using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Payments.NopCliGeneric.Models
{
    public class ConfigurationModel : BaseNopModel
    {
        public int ActiveStoreScopeConfiguration { get; set; }
        [NopResourceDisplayName("Plugins.Payments.NopCliGeneric.Fields.Url")]
        public string Url { get; set; }
        public bool UrlOverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Payments.NopCliGeneric.Fields.UseDev")]
        public bool UseDev { get; set; }
        public bool UseDevOverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Payments.NopCliGeneric.Fields.AdditionalFeePercentage")]
        public bool AdditionalFeePercentage { get; set; }
        public bool AdditionalFeePercentageOverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Payments.NopCliGeneric.Fields.AdditionalFee")]
        public decimal AdditionalFee { get; set; }
        public bool AdditionalFeeOverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Payments.NopCliGeneric.Fields.PassProductNamesAndTotals")]
        public bool PassProductNamesAndTotals { get; set; }
        public bool PassProductNamesAndTotalsOverrideForStore { get; set; }
        public bool IsStandardOverrideForStore { get; set; }
        [NopResourceDisplayName("Plugins.Payments.NopCliGeneric.Fields.IsStandard")]
        public bool IsStandard { get; set; }
        [NopResourceDisplayName("Plugins.Payments.NopCliGeneric.Fields.AuthKey")]
        public string AuthKey { get; set; } 
        public bool AuthKeyOverrideForStore { get; set; }
        public string ApprovedUrl { get; set; }
        public string CancelUrl { get; set; }
        public string DeclinedUrl { get; set; }
        public string GetUrl()
        {
            return UseDev
                ? "https://NopCliGeneric.NopCliGeneric.com"
                : string.IsNullOrEmpty(Url)? "https://payments.NopCliGeneric.com": Url;
        }
    }
}