using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Events;

namespace Nop.Plugin.Misc.NopCliGeneric.Services
{
    /// <summary>
    /// Represents event consumer
    /// </summary>
    public class EventConsumer : IConsumer<ShipmentSentEvent>, IConsumer<OrderPlacedEvent>, IConsumer<OrderCancelledEvent>
    {
        #region Fields

        private readonly INopCliGenericService _NopCliGenericService;

        #endregion

        #region Ctor

        public EventConsumer(INopCliGenericService NopCliGenericService)
        {
            _NopCliGenericService = NopCliGenericService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Handle the Shipment Sent Event
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        public void HandleEvent(ShipmentSentEvent eventMessage)
        {
            //handle event 
            _NopCliGenericService.SendNopCliGenericShip(eventMessage);
        }

        /// <summary>
        /// Handle the Order Placed Event
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        public void HandleEvent(OrderPlacedEvent eventMessage)
        {
            //handle event 
            _NopCliGenericService.SendNopCliGenericPlaced(eventMessage);
        }

        /// <summary>
        /// Handle the Order Cancelled Event
        /// </summary>
        /// <param name="eventMessage">The event message.</param>
        public void HandleEvent(OrderCancelledEvent eventMessage)
        {
            //handle event 
            _NopCliGenericService.SendNopCliGenericCancelled(eventMessage);
        }

        #endregion
    }
}