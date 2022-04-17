using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.NopCliGeneric.Components;
using Nop.Plugin.Payments.NopCliGeneric.Models;
using Nop.Plugin.Payments.NopCliGeneric.Validators;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Orders;
using Nop.Services.Payments;
using Nop.Services.Plugins;
using Nop.Services.Security;
using RestSharp;

namespace Nop.Plugin.Payments.NopCliGeneric
{
    /// <summary>
    /// NopCliGeneric payment processor
    /// </summary>
    public class NopCliGenericPaymentProcessor : BasePlugin, IPaymentMethod
    {
        #region Fields

        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly NopCliGenericPaymentSettings _nopCliGenericPaymentSettings;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IEncryptionService _encryptionService;
        private readonly IOrderTotalCalculationService _orderTotalCalculationService;

        #endregion

        #region Ctor

        public NopCliGenericPaymentProcessor(
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService,
            ISettingService settingService,
            IWebHelper webHelper,
            NopCliGenericPaymentSettings nopCliGenericPaymentSettings,
            IOrderProcessingService orderProcessingService,
            IEncryptionService encryptionService,
            IOrderTotalCalculationService orderTotalCalculationService)
        {
            _httpContextAccessor = httpContextAccessor;
            _localizationService = localizationService;
            _settingService = settingService;
            _webHelper = webHelper;
            _nopCliGenericPaymentSettings = nopCliGenericPaymentSettings;
            _orderProcessingService = orderProcessingService;
            _encryptionService = encryptionService;
            _orderTotalCalculationService = orderTotalCalculationService;

        }

        #endregion

        #region Utilities
 
        /// <summary>
        /// Gets PDT details
        /// </summary>
        /// <param name="tx">TX</param>
        /// <param name="values">Values</param>
        /// <param name="response">Response</param>
        /// <returns>Result</returns>
        public Dictionary<string, string> GetPdtDetails(string response)
        {
           var values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            var firstLine = true;
            foreach (var l in response.Split('\n'))
            {
                var line = l.Trim();
                if (firstLine)
                { 
                    firstLine = false;
                }
                else
                {
                    var equalPox = line.IndexOf('=');
                    if (equalPox >= 0)
                        values.Add(line[..equalPox], line[(equalPox + 1)..]);
                }
            }

            return values;
        }

        /// <summary>
        /// Verifies IPN
        /// </summary>
        /// <param name="formString">Form string</param>
        /// <param name="values">Values</param>
        /// <returns>Result</returns>
        public async Task<bool> VerifyIpnAsync(string formString, Dictionary<string, string> values)
        {
            return await Task.Run(() =>
            {
                var success = formString.Trim().ToLower().Contains("ResponseMessage".ToLower());

                if (!success)
                    success = formString.Trim().ToLower().Contains("ResponseCode".ToLower());

                values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
                foreach (var l in formString.Split('&'))
                {
                    var line = l.Trim();
                    var equalPox = line.IndexOf('=');
                    if (equalPox >= 0 && !values.Any(v =>
                        v.Key.Equals(line[..equalPox], StringComparison.OrdinalIgnoreCase)))
                    {
                        values.Add(line[..equalPox], line[(equalPox + 1)..]);
                    }
                }

                return success;
            });
        }

        #endregion

        #region Methods

        /// <summary>
        /// Process a payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessPayment(ProcessPaymentRequest processPaymentRequest)
        {
            return new ProcessPaymentResult()
            {
                AllowStoringCreditCardNumber = _nopCliGenericPaymentSettings.IsStandard == false
            };
        }

        public Task<ProcessPaymentResult> ProcessPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public async Task PostProcessPaymentAsync(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            if (_nopCliGenericPaymentSettings.IsStandard)
            {
                var url = $"{_webHelper.GetStoreLocation()}PaymentNopCliGeneric/Authorize";
                if (_httpContextAccessor.HttpContext != null) _httpContextAccessor.HttpContext.Response.Redirect(url);
            }
            else
            {
                await ProcessPaymentAsync(postProcessPaymentRequest);
            }
        }

        public Task<bool> HidePaymentMethodAsync(IList<ShoppingCartItem> cart)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Returns a value indicating whether payment method should be hidden during checkout
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>true - hide; false - display.</returns>
        public bool HidePaymentMethod(IList<ShoppingCartItem> cart)
        {
            //you can put any logic here
            //for example, hide this payment method if all products in the cart are downloadable
            //or hide this payment method if current customer is from certain country
            return false;
        }

        /// <summary>
        /// Gets additional handling fee
        /// </summary>
        /// <param name="cart">Shopping cart</param>
        /// <returns>Additional handling fee</returns>
        public async Task<decimal> GetAdditionalHandlingFeeAsync(IList<ShoppingCartItem> cart)
        {
           return await _orderTotalCalculationService.CalculatePaymentAdditionalFeeAsync(cart,
                _nopCliGenericPaymentSettings.AdditionalFee, _nopCliGenericPaymentSettings.AdditionalFeePercentage);
        }

        public Task<ProcessPaymentResult> ProcessRecurringPaymentAsync(ProcessPaymentRequest processPaymentRequest)
        {
            return Task.FromResult(new ProcessPaymentResult { Errors = new[] { "Recurring payment not supported" } });
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public Task<CapturePaymentResult> CaptureAsync(CapturePaymentRequest capturePaymentRequest)
        {
            return Task.FromResult(new CapturePaymentResult { Errors = new[] { "Capture method not supported" } });
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public Task<RefundPaymentResult> RefundAsync(RefundPaymentRequest refundPaymentRequest)
        {
            return Task.FromResult(new RefundPaymentResult { Errors = new[] { "Capture method not supported" } });
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public Task<VoidPaymentResult> VoidAsync(VoidPaymentRequest voidPaymentRequest)
        {
            return Task.FromResult(new VoidPaymentResult { Errors = new[] { "Capture method not supported" } });
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public Task<ProcessPaymentResult> ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            return Task.FromResult(new ProcessPaymentResult { Errors = new[] { "Capture method not supported" } });
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public Task<CancelRecurringPaymentResult> CancelRecurringPaymentAsync(
            CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            return Task.FromResult(
                new CancelRecurringPaymentResult { Errors = new[] { "Recurring payment not supported" } });
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public async Task<bool> CanRePostProcessPaymentAsync(Order order)
        {
            return await Task.Run(() =>
            {
                if (order == null)
                    throw new ArgumentNullException(nameof(order));

                //let's ensure that at least 5 seconds passed after order is placed
                //P.S. there's no any particular reason for that. we just do it
                return !((DateTime.UtcNow - order.CreatedOnUtc).TotalSeconds < 5);
            });
        }

        /// <summary>
        /// Validate payment form
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>List of validating errors</returns>
        public async Task<IList<string>> ValidatePaymentFormAsync(IFormCollection form)
        {
            return await Task.Run(() =>
            {
                var warnings = new List<string>();
                if (_nopCliGenericPaymentSettings.IsStandard)
                {
                    return warnings;
                }

                //validate
                var validator = new PaymentInfoValidator(_localizationService);
                var model = new PaymentInfoModel
                {
                    CardholderName = form["CardholderName"],
                    CardNumber = form["CardNumber"],
                    CardCode = form["CardCode"],
                    ExpireMonth = form["ExpireMonth"],
                    ExpireYear = form["ExpireYear"]
                };
                var validationResult = validator.Validate(model);
                if (!validationResult.IsValid)
                    warnings.AddRange(validationResult.Errors.Select(error => error.ErrorMessage));

                return warnings;
            });
        }

        internal PaymentResponseDto CreateBaseRequest(object data)
        {
            var nopCliGenericPaymentSettings = _settingService.LoadSettingAsync<NopCliGenericPaymentSettings>(0);

            var restClient = new RestClient(_nopCliGenericPaymentSettings.ApiUrl) { };

            var restRequest = new RestRequest("payment", Method.POST);
            restRequest.AddJsonBody(data);

            restRequest.AddHeader("Cache-Control", "no-cache");
            restRequest.AddHeader("Accept", "application/json");
            restRequest.AddHeader("Content-Type", "application/json");

            var response = restClient.Execute(restRequest);

            return JsonConvert.DeserializeObject<PaymentResponseDto>(response.Content);
        }

        public async Task ProcessPaymentAsync(PostProcessPaymentRequest paymentRequest)
        {
            var response = CreateBaseRequest(new
            {
                Store = paymentRequest.Order.StoreId,
                CardNumber = _encryptionService.DecryptText(paymentRequest.Order.CardNumber),
                Expiration =
                    _encryptionService.DecryptText(paymentRequest.Order.CardExpirationYear) +
                    _encryptionService.DecryptText(paymentRequest.Order.CardExpirationMonth),
                CVC = _encryptionService.DecryptText(paymentRequest.Order.CardCvv2),
                Amount = paymentRequest.Order.OrderTotal,
                Itbis = paymentRequest.Order.OrderTax,
                OrderId = paymentRequest.Order.Id
            });

            if (string.IsNullOrEmpty(response.ResponseMessage) || !response.ResponseMessage.Equals("APROBADA")) return;

            if (!string.IsNullOrEmpty(response.AuthorizationCode))
            {
                paymentRequest.Order.AuthorizationTransactionCode = response.AuthorizationCode;
                await _orderProcessingService.MarkAsAuthorizedAsync(paymentRequest.Order);
            }

            paymentRequest.Order.AuthorizationTransactionId = response.OrderId;
            await _orderProcessingService.MarkOrderAsPaidAsync(paymentRequest.Order);
        }

        /// <summary>
        /// Get payment information
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>Payment info holder</returns>
        public async Task<ProcessPaymentRequest> GetPaymentInfoAsync(IFormCollection form)
        {
            return await Task.Run(() =>
            {
                if (_nopCliGenericPaymentSettings.IsStandard)
                    return new ProcessPaymentRequest();

                return new ProcessPaymentRequest
                {
                    CreditCardType = form["CreditCardType"],
                    CreditCardName = form["CardholderName"],
                    CreditCardNumber = form["CardNumber"],
                    CreditCardExpireMonth = int.Parse(form["ExpireMonth"]),
                    CreditCardExpireYear = int.Parse(form["ExpireYear"]),
                    CreditCardCvv2 = form["CardCode"]
                };
            });
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/PaymentNopCliGeneric/Configure";
        }

        public string GetPublicViewComponentName()
        {
            return "PaymentNopCliGeneric";
        }

        public Task<string> GetPaymentMethodDescriptionAsync()
        {
            throw new NotImplementedException();
        }

      /// <summary>
        /// Gets a type of a view component for displaying plugin in public store ("payment info" checkout step)
        /// </summary>
        /// <returns>View component type</returns>
        public Type GetPublicViewComponent()
        {
            return typeof(PaymentNopCliGenericViewComponent);
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override async Task InstallAsync()
        {
            //settings
            await _settingService.SaveSettingAsync(new NopCliGenericPaymentSettings { UseDev = true });

            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Payments.NopCliGeneric.Fields.Url"] = "NopCliGeneric Url",
                ["Plugins.Payments.NopCliGeneric.Fields.UseDev"] = "Use Dev",
                ["Plugins.Payments.NopCliGeneric.Fields.AuthKey"] = "Auth Key",
                ["Plugins.Payments.NopCliGeneric.Fields.IsStandard"] = "Is Standard",
                ["Plugins.Payments.NopCliGeneric.Fields.AdditionalFee"] = "Additional fee",
                ["Plugins.Payments.NopCliGeneric.Fields.AdditionalFee.Hint"] =
                    "Enter additional fee to charge your customers.",
                ["Plugins.Payments.NopCliGeneric.Fields.AdditionalFeePercentage"] = "Additional fee. Use percentage",
                ["Plugins.Payments.NopCliGeneric.Fields.AdditionalFeePercentage.Hint"] =
                    "Determines whether to apply a percentage additional fee to the order total. If not enabled, a fixed value is used.",
                ["Plugins.Payments.NopCliGeneric.Fields.PassProductNamesAndTotals"] =
                    "Pass product names and order totals to NopCliGeneric",
                ["Plugins.Payments.NopCliGeneric.Fields.PassProductNamesAndTotals.Hint"] =
                    "Check if product names and order totals should be passed to NopCliGeneric.",
                ["Plugins.Payments.NopCliGeneric.Fields.RedirectionTip"] =
                    "You will be redirected to NopCliGeneric site to complete the order.",
                ["Plugins.Payments.NopCliGeneric.Instructions"] = @"
            <p>
	            <b>If you're using this gateway ensure that your primary store currency is supported by NopCliGeneric.</b>
	            <br />
	            <br />To use PDT, you must activate PDT and Auto Return in your NopCliGeneric account profile. You must also acquire a PDT identity token, which is used in all PDT communication you send to NopCliGeneric. Follow these steps to configure your account for PDT:<br />
	            <br />1. Log in to your NopCliGeneric account (click <a href=""https://www.NopCliGeneric.com"" target=""_blank"">here</a> to create your account).
	            <br />2. Click the Profile button.
	            <br />3. Click the Profile and Settings button.
	            <br />4. Select the My selling tools item on left panel.
	            <br />5. Click Website Preferences Update in the Selling online section.
	            <br />6. Under Auto Return for Website Payments, click the On radio button.
	            <br />7. For the Return URL, enter the URL on your site that will receive the transaction ID posted by NopCliGeneric after a customer payment ({0}).
                <br />8. Under Payment Data Transfer, click the On radio button and get your PDT identity token.
	            <br />9. Click Save.
	            <br />
            </p>",
                ["Plugins.Payments.NopCliGeneric.PaymentMethodDescription"] =
                    "You will be redirected to NopCliGeneric site to complete the payment",
                ["Plugins.Payments.NopCliGeneric.RoundingWarning"] =
                    "It looks like you have \"ShoppingCartSettings.RoundPricesDuringCalculation\" setting disabled. Keep in mind that this can lead to a discrepancy of the order total amount, as NopCliGeneric only rounds to two decimals."
            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<NopCliGenericPaymentSettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Payments.NopCliGeneric");

            await base.UninstallAsync();
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets a value indicating whether capture is supported
        /// </summary>
        public bool SupportCapture => false;

        /// <summary>
        /// Gets a value indicating whether partial refund is supported
        /// </summary>
        public bool SupportPartiallyRefund => false;

        /// <summary>
        /// Gets a value indicating whether refund is supported
        /// </summary>
        public bool SupportRefund => false;

        /// <summary>
        /// Gets a value indicating whether void is supported
        /// </summary>
        public bool SupportVoid => false;

        /// <summary>
        /// Gets a recurring payment type of payment method
        /// </summary>
        public RecurringPaymentType RecurringPaymentType => RecurringPaymentType.NotSupported;

        /// <summary>
        /// Gets a payment method type
        /// </summary>
        public PaymentMethodType PaymentMethodType => PaymentMethodType.Redirection;

        /// <summary>
        /// Gets a value indicating whether we should display a payment information page for this plugin
        /// </summary>
        public bool SkipPaymentInfo => false;

        /// <summary>
        /// Gets a payment method description that will be displayed on checkout pages in the public store
        /// </summary>
        public Task<string> PaymentMethodDescription =>
            //return description of this payment method to be display on "payment method" checkout step. good practice is to make it localizable
            //for example, for a redirection payment method, description may be like this: "You will be redirected to NopCliGeneric site to complete the payment"
            _localizationService.GetResourceAsync("Plugins.Payments.NopCliGeneric.PaymentMethodDescription");

        #endregion
    }
}