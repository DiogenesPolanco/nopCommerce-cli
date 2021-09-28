using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        public async Task<IActionResult> Configure()
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var nopCliGenericPaymentSettings =
                await _settingService.LoadSettingAsync<NopCliGenericPaymentSettings>(storeScope);

            var model = new ConfigurationModel
            {
                IsStandard = nopCliGenericPaymentSettings.IsStandard,
                Url = nopCliGenericPaymentSettings.ApiUrl,
                UseDev = nopCliGenericPaymentSettings.UseDev,
                AuthKey = nopCliGenericPaymentSettings.AuthKey,
                AdditionalFeePercentage = nopCliGenericPaymentSettings.AdditionalFeePercentage,
                AdditionalFee = nopCliGenericPaymentSettings.AdditionalFee,
                ActiveStoreScopeConfiguration = storeScope
            };
            if (storeScope <= 0) return View("~/Plugins/Payments.NopCliGeneric/Views/Configure.cshtml", model);

            model.AuthKeyOverrideForStore =
                await _settingService.SettingExistsAsync(nopCliGenericPaymentSettings, x => x.AuthKey, storeScope);
            model.UrlOverrideForStore =
                await _settingService.SettingExistsAsync(nopCliGenericPaymentSettings, x => x.ApiUrl, storeScope);
            model.IsStandardOverrideForStore =
                await _settingService.SettingExistsAsync(nopCliGenericPaymentSettings, x => x.IsStandard, storeScope);
            model.UseDevOverrideForStore =
                await _settingService.SettingExistsAsync(nopCliGenericPaymentSettings, x => x.UseDev, storeScope);
            model.AdditionalFeePercentageOverrideForStore =
                await _settingService.SettingExistsAsync(nopCliGenericPaymentSettings, x => x.AdditionalFeePercentage,
                    storeScope);
            model.AdditionalFeeOverrideForStore = await _settingService.SettingExistsAsync(nopCliGenericPaymentSettings,
                x => x.AdditionalFee, storeScope);

            return View("~/Plugins/Payments.NopCliGeneric/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            if (!ModelState.IsValid)
                return await Configure();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var nopCliGenericPaymentSettings =
                await _settingService.LoadSettingAsync<NopCliGenericPaymentSettings>(storeScope);

            //save settings
            nopCliGenericPaymentSettings.ApiUrl = nopCliGenericPaymentSettings.ApiUrl;
            nopCliGenericPaymentSettings.UseDev = nopCliGenericPaymentSettings.UseDev;
            nopCliGenericPaymentSettings.IsStandard = nopCliGenericPaymentSettings.IsStandard;
            nopCliGenericPaymentSettings.AuthKey = nopCliGenericPaymentSettings.AuthKey;
            nopCliGenericPaymentSettings.AdditionalFeePercentage = model.AdditionalFeePercentage;
            nopCliGenericPaymentSettings.AdditionalFee = model.AdditionalFee;

            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            await _settingService.SaveSettingOverridablePerStoreAsync(nopCliGenericPaymentSettings, x => x.ApiUrl,
                model.UrlOverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(nopCliGenericPaymentSettings, x => x.IsStandard,
                model.IsStandardOverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(nopCliGenericPaymentSettings, x => x.AuthKey,
                model.AuthKeyOverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(nopCliGenericPaymentSettings, x => x.UseDev,
                model.UseDevOverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(nopCliGenericPaymentSettings,
                x => x.AdditionalFeePercentage,
                model.AdditionalFeePercentageOverrideForStore, storeScope, false);
            await _settingService.SaveSettingOverridablePerStoreAsync(nopCliGenericPaymentSettings,
                x => x.AdditionalFee,
                model.AdditionalFeeOverrideForStore, storeScope, false);

            //now clear settings cache
            await _settingService.ClearCacheAsync();
            _notificationService.SuccessNotification(
                await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        //action displaying notification (warning) to a store owner about inaccurate  rounding
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> RoundingWarning(bool passProductNamesAndTotals)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManagePaymentMethods))
                return AccessDeniedView();

            //prices and total aren't rounded, so display warning
            if (passProductNamesAndTotals && !_shoppingCartSettings.RoundPricesDuringCalculation)
                return Json(new
                {
                    Result = await _localizationService.GetResourceAsync(
                        "Plugins.Payments.NopCliGeneric.RoundingWarning")
                });

            return Json(new { Result = string.Empty });
        }

        public async Task<IActionResult> Authorize(int? orderId)
        {
            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var nopCliGenericPaymentSettings =
                await _settingService.LoadSettingAsync<NopCliGenericPaymentSettings>(storeScope);

            var model = new ConfigurationModel
            {
                Url = nopCliGenericPaymentSettings.ApiUrl,
                UseDev = nopCliGenericPaymentSettings.UseDev,
                AdditionalFeePercentage = nopCliGenericPaymentSettings.AdditionalFeePercentage,
                AdditionalFee = nopCliGenericPaymentSettings.AdditionalFee,
                ActiveStoreScopeConfiguration = storeScope,
                ApprovedUrl = $"{_webHelper.GetStoreLocation()}PaymentNopCliGeneric/PdtHandler",
                CancelUrl = $"{_webHelper.GetStoreLocation()}PaymentNopCliGeneric/CancelHandler",
                DeclinedUrl = $"{_webHelper.GetStoreLocation()}PaymentNopCliGeneric/DeclinedHandler",
                AuthKey = nopCliGenericPaymentSettings.AuthKey,
            };

            var orders = await _orderService.SearchOrdersAsync(
                customerId: _workContext.GetCurrentCustomerAsync().Id,
                paymentMethodSystemName: "Payments.NopCliGeneric",
                psIds: new List<int>() { (int)PaymentStatus.Pending }
            );
            var order = orders.FirstOrDefault(o => !orderId.HasValue || o.Id == orderId);

            if (order == null) return RedirectToRoute("HomePage");

            return View("~/Plugins/Payments.NopCliGeneric/Views/Authorize.cshtml", model);
        }

        public async Task<IActionResult> PdtHandler()
        {
            if (await _paymentPluginManager.LoadPluginBySystemNameAsync("Payments.NopCliGeneric") is
                not NopCliGenericPaymentProcessor
                processor || !_paymentPluginManager.IsPluginActive(processor))
                throw new NopException(" Standard module cannot be loaded");

            var values = processor.GetPdtDetails(HttpContext.Request.QueryString.Value);
            if (!values.Any())
                return RedirectToAction("Index", "Home", new { area = "" });

            var orderNumber = _webHelper.QueryString<string>("OrderNumber");
            if (!int.TryParse(orderNumber, out var orderId))
                return RedirectToRoute("CheckoutCompleted", new { orderId });

            var order = await _orderService.GetOrderByIdAsync(orderId);
            if (order == null || order.PaymentStatus == PaymentStatus.Paid)
                return RedirectToRoute("OrderDetails", new { orderId });

            var authorizationCode = _webHelper.QueryString<string>("AuthorizationCode");
            var OrderId = _webHelper.QueryString<string>("OrderId");
            var responseMessage = _webHelper.QueryString<string>("ResponseMessage");
            var newPaymentStatus = NopCliGenericHelper.GetPaymentStatus(responseMessage);

            order.AuthorizationTransactionCode = authorizationCode;
            order.AuthorizationTransactionId = OrderId;
            await _orderService.UpdateOrderAsync(order);

            #region Standard payment

            {
                switch (newPaymentStatus)
                {
                    case PaymentStatus.Authorized:
                    {
                        //valid
                        if (_orderProcessingService.CanMarkOrderAsAuthorized(order))
                        {
                            await _orderProcessingService.MarkAsAuthorizedAsync(order);
                        }
                    }
                        break;
                    case PaymentStatus.Paid:
                    {
                        //valid
                        if (_orderProcessingService.CanMarkOrderAsPaid(order))
                        {
                            order.AuthorizationTransactionId = authorizationCode;
                            await _orderService.UpdateOrderAsync(order);

                            await _orderProcessingService.MarkOrderAsPaidAsync(order);
                        }
                    }
                        break;
                    case PaymentStatus.Refunded:
                    {
                        //refund
                        if (_orderProcessingService.CanRefundOffline(order))
                        {
                            await _orderProcessingService.RefundOfflineAsync(order);
                        }
                    }
                        break;
                    case PaymentStatus.Voided:
                    {
                        if (_orderProcessingService.CanVoidOffline(order))
                        {
                            await _orderProcessingService.VoidOfflineAsync(order);
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

        public async Task<IActionResult> DeclinedHandler()
        {
            if (await _paymentPluginManager.LoadPluginBySystemNameAsync("Payments.NopCliGeneric") is
                not NopCliGenericPaymentProcessor
                processor || !_paymentPluginManager.IsPluginActive(processor))
                throw new NopException(" Standard module cannot be loaded");

            var values = processor.GetPdtDetails(HttpContext.Request.QueryString.Value);
            if (!values.Any()) 
                return RedirectToAction("Index", "Home", new { area = "" });

            var orderNumber = _webHelper.QueryString<string>("OrderNumber");
            if (!int.TryParse(orderNumber, out var orderId))
                return RedirectToRoute("CheckoutCompleted", new { orderId });

            var order =await _orderService.GetOrderByIdAsync(orderId);
            if (order == null || order.PaymentStatus == PaymentStatus.Paid)
                return RedirectToRoute("OrderDetails", new { orderId });

            var authorizationCode = _webHelper.QueryString<string>("AuthorizationCode");
            var OrderId = _webHelper.QueryString<string>("OrderId");
            var errorDescription = _webHelper.QueryString<string>("ErrorDescription");
            var responseMessage = _webHelper.QueryString<string>("ResponseMessage");

            await _orderService.InsertOrderNoteAsync((new OrderNote()
            {
                OrderId = orderId,
                Note = $"OrderId: {OrderId}: Response:{responseMessage} Description:{errorDescription}"
            }));

            if (_orderProcessingService.CanVoidOffline(order))
            {
                await _orderProcessingService.VoidOfflineAsync(order);
            }

            return RedirectToRoute("OrderDetails", new { orderId });
        }

        public async Task<IActionResult> CancelHandler()
        {
            var order = await _orderService.SearchOrdersAsync(_storeContext.GetCurrentStoreAsync().Id,
                customerId: _workContext.GetCurrentCustomerAsync().Id, pageSize: 1);
            return order != null
                ? RedirectToRoute("OrderDetails", new { orderId = order.FirstOrDefault()?.Id })
                : RedirectToRoute("Homepage");
        }

        #endregion
    }
}