using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Tasks;

namespace Nop.Plugin.Misc.NopCliGeneric.Services
{
    public interface INopCliGenericService
    {  
        Task SendNopCliGenericShipAsync(ShipmentSentEvent eventShipment);
        Task SendNopCliGenericPlacedAsync(OrderPlacedEvent orderPlacedEvent);
        Task SendNopCliGenericCancelledAsync(OrderCancelledEvent orderCancelledEvent);
    }
}