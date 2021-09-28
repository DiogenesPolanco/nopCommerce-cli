using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.NopCliGeneric.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Payments.NopCliGeneric.Controllers
{
    public class PaymentNopCliGenericController : BasePaymentController
    {
        #region Fields

        private readonly IOrderService _orderService;
        private readonly IPermissionService _permissionService;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;
        private readonly IWorkContext _workContext;
        private readonly INotificationService _notificationService;
        private readonly IWebHelper _webHelper;
        private readonly IPaymentPluginManager _paymentPluginManager;
        private readonly IOrderProcessingService _orderProcessingService; 
        private readonly ShoppingCartSettings _shoppingCartSettings;

        #endregion

        #region Ctor

        public PaymentNopCliGenericController(
            IOrderService orderService,
            IPermissionService permissionService,
            ILocalizationService localizationService,
            ISettingService settingService,
            IStoreContext storeContext,
            IWorkContext workContext,
            INotificationService notificationService,
            IWebHelper webHelper,
            IPaymentPluginManager paymentPluginManager,
            IOrderProcessingService orderProcessingService, 
            ShoppingCartSettings shoppingCartSettings)
        {
            _orderService = orderService;
            _permissionService = permissionService;
            _localizationService = localizationService;
            _settingService = settingService;
            _shoppingCartSettings = shoppingCartSettings;
            _storeContext = storeContext;
            _workContext = workContext;
            _paymentPluginManager = paymentPluginManager;
            _notificationService = notificationService;
            _orderProcessingService = orderProcessingService;
            _webHelper = webHelper; 
        }

        #endregion

        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var NopCliGenericPaymentSettings = _settingService.LoadSetting<NopCliGenericPaymentSettings>(storeScope);

            var model = new ConfigurationModel
            {
                IsStandard = NopCliGenericPaymentSettings.IsStandard,
                Url = NopCliGenericPaymentSettings.ApiUrl,
                UseDev = NopCliGenericPaymentSettings.UseDev, 
                AuthKey = NopCliGenericPaymentSettings.AuthKey,               
                AdditionalFeePercentage = NopCliGenericPaymentSettings.AdditionalFeePercentage,
                AdditionalFee = NopCliGenericPaymentSettings.AdditionalFee,
                ActiveStoreScopeConfiguration = storeScope              
            };
            if (storeScope <= 0) return View("~/Plugins/Payments.NopCliGeneric/Views/Configure.cshtml", model);

            model.AuthKeyOverrideForStore =
                _settingService.SettingExists(NopCliGenericPaymentSettings, x => x.AuthKey, storeScope);
            model.UrlOverrideForStore =
                _settingService.SettingExists(NopCliGenericPaymentSettings, x => x.ApiUrl, storeScope);
            model.IsStandardOverrideForStore =
                _settingService.SettingExists(NopCliGenericPaymentSettings, x => x.IsStandard, storeScope);         
            model.UseDevOverrideForStore =
                _settingService.SettingExists(NopCliGenericPaymentSettings, x => x.UseDev, storeScope);         
            model.AdditionalFeePercentageOverrideForStore =
                _settingService.SettingExists(NopCliGenericPaymentSettings, x => x.AdditionalFeePercentage,
                    storeScope);
            model.AdditionalFeeOverrideForStore = _settingService.SettingExists(NopCliGenericPaymentSettings,
                x => x.AdditionalFee, storeScope);

            return View("~/Plugins/Payments.NopCliGeneric/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return Configure();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var NopCliGenericPaymentSettings = _settingService.LoadSetting<NopCliGenericPaymentSettings>(storeScope);

            //save settings
            NopCliGenericPaymentSettings.ApiUrl = NopCliGenericPaymentSettings.ApiUrl;
            NopCliGenericPaymentSettings.UseDev = NopCliGenericPaymentSettings.UseDev;
            NopCliGenericPaymentSettings.IsStandard = NopCliGenericPaymentSettings.IsStandard; 
            NopCliGenericPaymentSettings.AuthKey = NopCliGenericPaymentSettings.AuthKey; 
            NopCliGenericPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
            NopCliGenericPaymentSettings.AdditionalFee = model.AdditionalFee;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(NopCliGenericPaymentSettings, x => x.ApiUrl,
                model.UrlOverrideForStore, storeScope, false);         
            _settingService.SaveSettingOverridablePerStore(NopCliGenericPaymentSettings, x => x.IsStandard,
                model.IsStandardOverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(NopCliGenericPaymentSettings, x => x.AuthKey,
                model.AuthKeyOverrideForStore, storeScope, false);    
            _settingService.SaveSettingOverridablePerStore(NopCliGenericPaymentSettings, x => x.UseDev,
                model.UseDevOverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(NopCliGenericPaymentSettings, x => x.AdditionalFeePercentage,
                model.AdditionalFeePercentageOverrideForStore, storeScope, false);
            _settingService.SaveSettingOverridablePerStore(NopCliGenericPaymentSettings, x => x.AdditionalFee,
                model.AdditionalFeeOverrideForStore, storeScope, false);

            //now clear settings cache
            _settingService.ClearCache();
            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        //action displaying notification (warning) to a store owner about inaccurate  rounding
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult RoundingWarning(bool passProductNamesAndTotals)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //prices and total aren't rounded, so display warning
            if (passProductNamesAndTotals && !_shoppingCartSettings.RoundPricesDuringCalculation)
                return Json(new
                {
                    Result = _localizationService.GetResource("Plugins.Payments.NopCliGeneric.RoundingWarning")
                });

            return Json(new { Result = string.Empty });
        }

        public IActionResult Authorize(int? orderId)
        {
            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var NopCliGenericPaymentSettings = _settingService.LoadSetting<NopCliGenericPaymentSettings>(storeScope);

            var model = new ConfigurationModel
            {
                Url = NopCliGenericPaymentSettings.ApiUrl,
                UseDev = NopCliGenericPaymentSettings.UseDev,                
                AdditionalFeePercentage = NopCliGenericPaymentSettings.AdditionalFeePercentage,
                AdditionalFee = NopCliGenericPaymentSettings.AdditionalFee,
                ActiveStoreScopeConfiguration = storeScope,              
                ApprovedUrl = $"{_webHelper.GetStoreLocation()}PaymentNopCliGeneric/PdtHandler",
                CancelUrl = $"{_webHelper.GetStoreLocation()}PaymentNopCliGeneric/CancelHandler",
                DeclinedUrl = $"{_webHelper.GetStoreLocation()}PaymentNopCliGeneric/DeclinedHandler",
                AuthKey = NopCliGenericPaymentSettings.AuthKey,
            };

            var orders = _orderService.SearchOrders(
                customerId: _workContext.CurrentCustomer.Id,
                paymentMethodSystemName: "Payments.NopCliGeneric",
                psIds: new List<int>() { (int)PaymentStatus.Pending }
            );
            var order = orders.FirstOrDefault(o => !orderId.HasValue || o.Id == orderId);

            if (order == null) return RedirectToRoute("HomePage"); 
            
            return View("~/Plugins/Payments.NopCliGeneric/Views/Authorize.cshtml", model);
        }

        public IActionResult PdtHandler()
        {
            if (!(_paymentPluginManager.LoadPluginBySystemName("Payments.NopCliGeneric") is NopCliGenericPaymentProcessor
                processor) || !_paymentPluginManager.IsPluginActive(processor))
                throw new NopException(" Standard module cannot be loaded");

            if (!processor.GetPdtDetails(HttpContext.Request.QueryString.Value, out var values) && values.Any())
                return RedirectToAction("Index", "Home", new { area = "" });

            var orderNumber = _webHelper.QueryString<string>("OrderNumber");
            if (!int.TryParse(orderNumber, out var orderId))
                return RedirectToRoute("CheckoutCompleted", new { orderId });

            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.PaymentStatus == PaymentStatus.Paid)
                return RedirectToRoute("OrderDetails", new { orderId });

            var authorizationCode = _webHelper.QueryString<string>("AuthorizationCode");
            var OrderId = _webHelper.QueryString<string>("OrderId");
            var responseMessage = _webHelper.QueryString<string>("ResponseMessage");
            var newPaymentStatus = NopCliGenericHelper.GetPaymentStatus(responseMessage);

            order.AuthorizationTransactionCode = authorizationCode;
            order.AuthorizationTransactionId = OrderId;
            _orderService.UpdateOrder(order);

            #region Standard payment

            {
                switch (newPaymentStatus)
                {
                    case PaymentStatus.Authorized:
                    {
                        //valid
                        if (_orderProcessingService.CanMarkOrderAsAuthorized(order))
                        {
                            _orderProcessingService.MarkAsAuthorized(order);
                        }
                    }
                        break;
                    case PaymentStatus.Paid:
                    {
                        //valid
                        if (_orderProcessingService.CanMarkOrderAsPaid(order))
                        {
                            order.AuthorizationTransactionId = authorizationCode;
                            _orderService.UpdateOrder(order);

                            _orderProcessingService.MarkOrderAsPaid(order);
                        }
                    }
                        break;
                    case PaymentStatus.Refunded:
                    {
                        //refund
                        if (_orderProcessingService.CanRefundOffline(order))
                        {
                            _orderProcessingService.RefundOffline(order);
                        }
                    }
                        break;
                    case PaymentStatus.Voided:
                    {
                        if (_orderProcessingService.CanVoidOffline(order))
                        {
                            _orderProcessingService.VoidOffline(order);
                        }
                    }
                        break;
                    case PaymentStatus.Pending:
                    case PaymentStatus.PartiallyRefunded:
                        break;
                    default:
                        throw new ArgumentOutOfRangeException();
                }

                return RedirectToRoute("OrderDetails", new { orderId });
            }

            #endregion
        }

        public IActionResult DeclinedHandler()
        {
            if (!(_paymentPluginManager.LoadPluginBySystemName("Payments.NopCliGeneric") is NopCliGenericPaymentProcessor
                processor) || !_paymentPluginManager.IsPluginActive(processor))
                throw new NopException(" Standard module cannot be loaded");

            if (!processor.GetPdtDetails(HttpContext.Request.QueryString.Value, out var values) && values.Any())
                return RedirectToAction("Index", "Home", new { area = "" });

            var orderNumber = _webHelper.QueryString<string>("OrderNumber");
            if (!int.TryParse(orderNumber, out var orderId))
                return RedirectToRoute("CheckoutCompleted", new { orderId });

            var order = _orderService.GetOrderById(orderId);
            if (order == null || order.PaymentStatus == PaymentStatus.Paid)
                return RedirectToRoute("OrderDetails", new { orderId });

            var authorizationCode = _webHelper.QueryString<string>("AuthorizationCode");
            var OrderId = _webHelper.QueryString<string>("OrderId");
            var errorDescription = _webHelper.QueryString<string>("ErrorDescription");
            var responseMessage = _webHelper.QueryString<string>("ResponseMessage");

            _orderService.InsertOrderNote((new OrderNote()
            {
                OrderId = orderId,
                Note = $"OrderId: {OrderId}: Response:{responseMessage} Description:{errorDescription}"
            }));

            if (_orderProcessingService.CanVoidOffline(order))
            {
                _orderProcessingService.VoidOffline(order);
            }

            return RedirectToRoute("OrderDetails", new { orderId });
        }

        public IActionResult CancelHandler()
        {
            var order = _orderService.SearchOrders(_storeContext.CurrentStore.Id,
                customerId: _workContext.CurrentCustomer.Id, pageSize: 1).FirstOrDefault();

            if (order != null)
                return RedirectToRoute("OrderDetails", new { orderId = order.Id });

            return RedirectToRoute("Homepage");
            ;
        }

        #endregion
    }
}