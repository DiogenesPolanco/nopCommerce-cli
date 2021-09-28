using System.Linq;
using Nop.Core.Domain.Payments;
using Nop.Plugin.Payments.NopCliGeneric.Models;
using System.Text;
using System.Security.Cryptography;

namespace Nop.Plugin.Payments.NopCliGeneric
{
    /// <summary>
    /// Represents NopCliGeneric helper
    /// </summary>
    public class NopCliGenericHelper
    {
        #region Methods

        public static string Hash(ConfigurationModel model)
        {  
            using var hmac = new HMACSHA512(Encoding.UTF8.GetBytes(model.AuthKey));
            var hashResult = hmac.ComputeHash(Encoding.UTF8.GetBytes(model.AuthKey));
            return hashResult.Aggregate(string.Empty, (current, b) => current + $"{b:x2}");
        }

        /// <summary>
        /// Gets a payment status
        /// </summary> 
        /// <param name="responseMessage"></param>
        /// <returns>Payment status</returns>
        public static PaymentStatus GetPaymentStatus(string responseMessage)
        {
            return responseMessage.Contains("APROBADA") ? PaymentStatus.Paid : PaymentStatus.Pending;
        }

        #endregion
    }
}