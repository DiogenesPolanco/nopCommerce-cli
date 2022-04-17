using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Core.Domain.ScheduleTasks;

namespace Nop.Plugin.Misc.NopCliGeneric.Services
{
    public interface INopCliGenericService
    {  
        ScheduleTask SendNopCliGenericShipAsync(ShipmentSentEvent eventShipment);
        ScheduleTask SendNopCliGenericPlacedAsync(OrderPlacedEvent orderPlacedEvent);
        ScheduleTask SendNopCliGenericCancelledAsync(OrderCancelledEvent orderCancelledEvent);
    }
}