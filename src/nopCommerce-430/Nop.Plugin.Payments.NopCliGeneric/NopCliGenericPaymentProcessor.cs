using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Plugin.Payments.NopCliGeneric.Models;
using Nop.Plugin.Payments.NopCliGeneric.Validators;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Customers;
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
        private readonly IPaymentService _paymentService;
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper;
        private readonly NopCliGenericPaymentSettings _NopCliGenericPaymentSettings;
        private readonly ICustomerAttributeService _customerAttributeService;
        private readonly ICustomerAttributeParser _customerAttributeParser;
        private readonly IGenericAttributeService _genericAttributeService;
        private readonly IWorkContext _workContext;
        private readonly IOrderProcessingService _orderProcessingService;
        private readonly IEncryptionService _encryptionService;

        #endregion

        #region Ctor

        public NopCliGenericPaymentProcessor(
            IHttpContextAccessor httpContextAccessor,
            ILocalizationService localizationService,
            IPaymentService paymentService,
            ISettingService settingService,
            IWebHelper webHelper,
            NopCliGenericPaymentSettings NopCliGenericPaymentSettings,
            ICustomerAttributeService customerAttributeService,
            ICustomerAttributeParser customerAttributeParser,
            IGenericAttributeService genericAttributeService,
            IWorkContext workContext,
            IOrderProcessingService orderProcessingService,
            IEncryptionService encryptionService)
        {
            _httpContextAccessor = httpContextAccessor;
            _localizationService = localizationService;
            _paymentService = paymentService;
            _settingService = settingService;
            _webHelper = webHelper;
            _NopCliGenericPaymentSettings = NopCliGenericPaymentSettings;
            _customerAttributeService = customerAttributeService;
            _customerAttributeParser = customerAttributeParser;
            _genericAttributeService = genericAttributeService;
            _workContext = workContext;
            _orderProcessingService = orderProcessingService;
            _encryptionService = encryptionService;
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
        public bool GetPdtDetails(string response, out Dictionary<string, string> values)
        {
            values = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            bool firstLine = true, success = false;
            foreach (var l in response.Split('\n'))
            {
                var line = l.Trim();
                if (firstLine)
                {
                    success = line.Contains("ResponseMessage=APROBADA", StringComparison.OrdinalIgnoreCase);

                    if (!success)
                        success = line.Contains("ResponseCode=APROBADA", StringComparison.OrdinalIgnoreCase);

                    firstLine = false;
                }
                else
                {
                    var equalPox = line.IndexOf('=');
                    if (equalPox >= 0)
                        values.Add(line[..equalPox], line[(equalPox + 1)..]);
                }
            }

            return success;
        }

        /// <summary>
        /// Verifies IPN
        /// </summary>
        /// <param name="formString">Form string</param>
        /// <param name="values">Values</param>
        /// <returns>Result</returns>
        public bool VerifyIpn(string formString, out Dictionary<string, string> values)
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
                AllowStoringCreditCardNumber = _NopCliGenericPaymentSettings.IsStandard == false
            };
        }

        /// <summary>
        /// Post process payment (used by payment gateways that require redirecting to a third-party URL)
        /// </summary>
        /// <param name="postProcessPaymentRequest">Payment info required for an order processing</param>
        public void PostProcessPayment(PostProcessPaymentRequest postProcessPaymentRequest)
        {
            if (_NopCliGenericPaymentSettings.IsStandard)
            {
                var url = $"{_webHelper.GetStoreLocation()}PaymentNopCliGeneric/Authorize";
                _httpContextAccessor.HttpContext.Response.Redirect(url);
            }
            else
            {
                ProcessPayment(postProcessPaymentRequest);
            }
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
        public decimal GetAdditionalHandlingFee(IList<ShoppingCartItem> cart)
        {
            return _paymentService.CalculateAdditionalFee(cart,
                _NopCliGenericPaymentSettings.AdditionalFee, _NopCliGenericPaymentSettings.AdditionalFeePercentage);
        }

        /// <summary>
        /// Captures payment
        /// </summary>
        /// <param name="capturePaymentRequest">Capture payment request</param>
        /// <returns>Capture payment result</returns>
        public CapturePaymentResult Capture(CapturePaymentRequest capturePaymentRequest)
        {
            return new CapturePaymentResult { Errors = new[] { "Capture method not supported" } };
        }

        /// <summary>
        /// Refunds a payment
        /// </summary>
        /// <param name="refundPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public RefundPaymentResult Refund(RefundPaymentRequest refundPaymentRequest)
        {
            return new RefundPaymentResult { Errors = new[] { "Refund method not supported" } };
        }

        /// <summary>
        /// Voids a payment
        /// </summary>
        /// <param name="voidPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public VoidPaymentResult Void(VoidPaymentRequest voidPaymentRequest)
        {
            return new VoidPaymentResult { Errors = new[] { "Void method not supported" } };
        }

        /// <summary>
        /// Process recurring payment
        /// </summary>
        /// <param name="processPaymentRequest">Payment info required for an order processing</param>
        /// <returns>Process payment result</returns>
        public ProcessPaymentResult ProcessRecurringPayment(ProcessPaymentRequest processPaymentRequest)
        {
            return new ProcessPaymentResult { Errors = new[] { "Recurring payment not supported" } };
        }

        /// <summary>
        /// Cancels a recurring payment
        /// </summary>
        /// <param name="cancelPaymentRequest">Request</param>
        /// <returns>Result</returns>
        public CancelRecurringPaymentResult CancelRecurringPayment(CancelRecurringPaymentRequest cancelPaymentRequest)
        {
            return new CancelRecurringPaymentResult { Errors = new[] { "Recurring payment not supported" } };
        }

        /// <summary>
        /// Gets a value indicating whether customers can complete a payment after order is placed but not completed (for redirection payment methods)
        /// </summary>
        /// <param name="order">Order</param>
        /// <returns>Result</returns>
        public bool CanRePostProcessPayment(Order order)
        {
            if (order == null)
                throw new ArgumentNullException(nameof(order));

            //let's ensure that at least 5 seconds passed after order is placed
            //P.S. there's no any particular reason for that. we just do it
            return !((DateTime.UtcNow - order.CreatedOnUtc).TotalSeconds < 5);
        }

        /// <summary>
        /// Validate payment form
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>List of validating errors</returns>
        public IList<string> ValidatePaymentForm(IFormCollection form)
        {
            var warnings = new List<string>();
            if (_NopCliGenericPaymentSettings.IsStandard)
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
        } 
        internal PaymentResponseDto CreateBaseRequest(object data)
        {
            var NopCliGenericPaymentSettings = _settingService.LoadSetting<NopCliGenericPaymentSettings>(0);
 
            var restClient = new RestClient(NopCliGenericPaymentSettings.ApiUrl){ };

            var restRequest = new RestRequest("payment", Method.POST);
            restRequest.AddJsonBody(data);

            restRequest.AddHeader("Cache-Control", "no-cache");
            restRequest.AddHeader("Accept", "application/json");
            restRequest.AddHeader("Content-Type", "application/json");  

            var response = restClient.Execute(restRequest);

            return JsonConvert.DeserializeObject<PaymentResponseDto>(response.Content);
        }

        public void ProcessPayment(PostProcessPaymentRequest paymentRequest)
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
                Itbis =  paymentRequest.Order.OrderTax,    
                OrderId = paymentRequest.Order.Id
            });

            if (string.IsNullOrEmpty(response.ResponseMessage) || !response.ResponseMessage.Equals("APROBADA")) return;

            if (!string.IsNullOrEmpty(response.AuthorizationCode))
            {
                paymentRequest.Order.AuthorizationTransactionCode = response.AuthorizationCode;
                _orderProcessingService.MarkAsAuthorized(paymentRequest.Order);
            }

            paymentRequest.Order.AuthorizationTransactionId = response.OrderId;
            _orderProcessingService.MarkOrderAsPaid(paymentRequest.Order);
        }

        /// <summary>
        /// Get payment information
        /// </summary>
        /// <param name="form">The parsed form values</param>
        /// <returns>Payment info holder</returns>
        public ProcessPaymentRequest GetPaymentInfo(IFormCollection form)
        { 
            if (_NopCliGenericPaymentSettings.IsStandard)
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

        /// <summary>
        /// Gets a name of a view component for displaying plugin in public store ("payment info" checkout step)
        /// </summary>
        /// <returns>View component name</returns>
        public void GetPublicViewComponent(out string viewComponentName)
        {
            viewComponentName = "PaymentNopCliGeneric";
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        {
            //settings
            _settingService.SaveSetting(new NopCliGenericPaymentSettings { UseDev = true });

            //locales
            _localizationService.AddPluginLocaleResource(new Dictionary<string, string>
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

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<NopCliGenericPaymentSettings>();

            //locales
            _localizationService.DeletePluginLocaleResources("Plugins.Payments.NopCliGeneric");

            base.Uninstall();
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
        public string PaymentMethodDescription =>
            //return description of this payment method to be display on "payment method" checkout step. good practice is to make it localizable
            //for example, for a redirection payment method, description may be like this: "You will be redirected to NopCliGeneric site to complete the payment"
            _localizationService.GetResource("Plugins.Payments.NopCliGeneric.PaymentMethodDescription");

        #endregion
    }
}