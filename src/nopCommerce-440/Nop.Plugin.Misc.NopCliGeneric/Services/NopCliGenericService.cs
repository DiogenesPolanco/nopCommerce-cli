using System;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Configuration;

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
        
        public Nop.Services.Tasks.Task SendNopCliGenericShipAsync(ShipmentSentEvent eventShipment)
        {
            throw new NotImplementedException();
        }

        public Nop.Services.Tasks.Task SendNopCliGenericPlacedAsync(OrderPlacedEvent orderPlacedEvent)
        {
            throw new NotImplementedException();
        }

        public Nop.Services.Tasks.Task SendNopCliGenericCancelledAsync(OrderCancelledEvent orderCancelledEvent)
        {
            throw new NotImplementedException();
        }
        #endregion

    }
}