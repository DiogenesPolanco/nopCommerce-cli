using System;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Configuration;
using Nop.Core.Domain.ScheduleTasks;

namespace Nop.Plugin.Misc.NopCliGeneric.Services
{
    public class NopCliGenericService : INopCliGenericService
    {
        #region Fields

        private NopCliGenericSettings _nopCliGenericService; 

        #endregion

        #region Ctor

        public async Task NopCliGenericServiceAsync(ISettingService settingService, IStoreContext storeContext)
        { 
            _nopCliGenericService = await settingService.LoadSettingAsync<NopCliGenericSettings>(await storeContext.GetActiveStoreScopeConfigurationAsync());
        }

        #endregion

        #region Methods
        
        public ScheduleTask SendNopCliGenericShipAsync(ShipmentSentEvent eventShipment)
        {
            throw new NotImplementedException();
        }

        public ScheduleTask SendNopCliGenericPlacedAsync(OrderPlacedEvent orderPlacedEvent)
        {
            throw new NotImplementedException();
        }

        public ScheduleTask SendNopCliGenericCancelledAsync(OrderCancelledEvent orderCancelledEvent)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}