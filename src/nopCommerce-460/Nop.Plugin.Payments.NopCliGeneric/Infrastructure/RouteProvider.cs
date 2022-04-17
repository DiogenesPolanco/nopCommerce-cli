using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Payments.NopCliGeneric.Infrastructure
{
     public partial class RouteProvider : IRouteProvider
     {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
          public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
          {
            //Auth 
          endpointRouteBuilder.MapControllerRoute("Plugin.Payments.NopCliGeneric.Authorize", "PaymentNopCliGeneric/Authorize/{orderId:min(0)}",
               new { controller = "PaymentNopCliGeneric", action = "Authorize" });
            //PDT
          endpointRouteBuilder.MapControllerRoute("Plugin.Payments.NopCliGeneric.PDTHandler", "Plugins/PaymentNopCliGeneric/PDTHandler",
               new { controller = "PaymentNopCliGeneric", action = "PDTHandler" });

            //IPN
          endpointRouteBuilder.MapControllerRoute("Plugin.Payments.NopCliGeneric.IPNHandler", "Plugins/PaymentNopCliGeneric/IPNHandler",
               new { controller = "PaymentNopCliGeneric", action = "IPNHandler" });

            //Cancel
          endpointRouteBuilder.MapControllerRoute("Plugin.Payments.NopCliGeneric.CancelOrder", "Plugins/PaymentNopCliGeneric/CancelOrder",
               new { controller = "PaymentNopCliGeneric", action = "CancelOrder" });
          }

        /// <summary>
        /// Gets a priority of route provider
        /// </summary>
          public int Priority => -1;
     }
}
