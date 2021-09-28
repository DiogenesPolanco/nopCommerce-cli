using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Routing;
using Nop.Web.Framework.Mvc.Routing;

namespace Nop.Plugin.Shipping.NopCliGeneric
{
    public partial class RouteProvider : IRouteProvider
    {
        /// <summary>
        /// Register routes
        /// </summary>
        /// <param name="endpointRouteBuilder">Route builder</param>
        public void RegisterRoutes(IEndpointRouteBuilder endpointRouteBuilder)
        {
            //Webhook
            endpointRouteBuilder.MapControllerRoute("Plugin.Payments.Shipping.WebhookHandler", "Plugins/ShippingNopCliGeneric/Webhook",
                new { controller = "ShippingNopCliGeneric", action = "Webhook" });
        }

        public int Priority => 0;
    }
}
