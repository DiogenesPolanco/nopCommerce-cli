using Nop.Core.Configuration;

namespace Nop.Plugin.Payments.NopCliGeneric
{
    /// <summary>
    /// Represents settings of the Azul Standard payment plugin
    /// </summary>
    public class NopCliGenericPaymentSettings : ISettings
    { 
        public string ApiUrl { get; set; } 
        public bool UseDev { get; set; }   
        public bool PassProductNamesAndTotals { get; set; }
        public bool AdditionalFeePercentage { get; set; }
        public decimal AdditionalFee { get; set; }      
        public bool IsStandard { get; set; } 
        public string AuthKey { get; set; } 
    }
}
