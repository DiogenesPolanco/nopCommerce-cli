namespace Nop.Plugin.Payments.NopCliGeneric.Models
{
    public class PaymentResponseDto
    {
        public string AuthorizationCode { get; set; }
        public string OrderId { get; set; } 
        public string ErrorDescription { get; set; } 
        public string ResponseMessage { get; set; }
    }
}