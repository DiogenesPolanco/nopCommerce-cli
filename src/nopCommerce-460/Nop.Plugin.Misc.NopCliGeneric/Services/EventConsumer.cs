using Nop.Services.Events;
using System.Threading.Tasks;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services.ScheduleTasks;

namespace Nop.Plugin.Misc.NopCliGeneric.Services
{
    /// <summary>
    /// Represents event consumer
    /// </summary>
    public class EventConsumer : IConsumer<ShipmentSentEvent>, IConsumer<OrderPlacedEvent>, IConsumer<OrderCancelledEvent>
    {
        #region Fields

        private readonly INopCliGenericService _nopCliGenericService;
        private readonly IScheduleTaskRunner _taskRunner;

        #endregion

        #region Ctor

        public EventConsumer(INopCliGenericService nopCliGenericService, IScheduleTaskRunner taskRunner)
        {
            _nopCliGenericService = nopCliGenericService;
            _taskRunner = taskRunner;
        }

        #endregion

        #region Methods

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(ShipmentSentEvent eventMessage)
        {
            //handle event 
            await _taskRunner.ExecuteAsync( _nopCliGenericService.SendNopCliGenericShipAsync(eventMessage), true);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(OrderPlacedEvent eventMessage)
        {
            //handle event 
            await _taskRunner.ExecuteAsync( _nopCliGenericService.SendNopCliGenericPlacedAsync(eventMessage), true);
        }

        /// <returns>A task that represents the asynchronous operation</returns>
        public async Task HandleEventAsync(OrderCancelledEvent eventMessage)
        {
            //handle event 
            await _taskRunner.ExecuteAsync( _nopCliGenericService.SendNopCliGenericCancelledAsync(eventMessage), true);
        }

        #endregion
    }
}