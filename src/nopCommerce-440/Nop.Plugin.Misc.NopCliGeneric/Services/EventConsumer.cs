using Nop.Services.Events;
using System.Threading.Tasks;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;

namespace Nop.Plugin.Misc.NopCliGeneric.Services
{
    /// <summary>
    /// Represents event consumer
    /// </summary>
    public class EventConsumer : IConsumer<ShipmentSentEvent>, IConsumer<OrderPlacedEvent>, IConsumer<OrderCancelledEvent>
    {
        #region Fields

        private readonly INopCliGenericService _nopCliGenericService;

        #endregion

        #region Ctor

        public EventConsumer(INopCliGenericService nopCliGenericService)
        {
            _nopCliGenericService = nopCliGenericService;
        }

        #endregion

        #region Methods

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(ShipmentSentEvent eventMessage)
        {
            //handle event 
            await _nopCliGenericService.SendNopCliGenericShipAsync(eventMessage).ExecuteAsync();
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
        {
            //handle event 
            await _nopCliGenericService.SendNopCliGenericPlacedAsync(eventMessage).ExecuteAsync();
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(OrderCancelledEvent eventMessage)
        {
            //handle event 
            await _nopCliGenericService.SendNopCliGenericCancelledAsync(eventMessage).ExecuteAsync();
        }

        #endregion
    }
}