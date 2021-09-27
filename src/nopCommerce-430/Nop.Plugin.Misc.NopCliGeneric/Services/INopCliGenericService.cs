using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping; 

namespace Nop.Plugin.Misc.NopCliGeneric.Services
{
    public interface INopCliGenericService
    {  
        void SendNopCliGenericShip(ShipmentSentEvent eventShipment); 
        void SendNopCliGenericPlaced(OrderPlacedEvent orderPlacedEvent); 
        void SendNopCliGenericCancelled(OrderCancelledEvent orderCancelledEvent); 
    }
}