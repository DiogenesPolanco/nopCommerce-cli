using System;
using Nop.Core;
using Nop.Core.Domain.Orders;
using Nop.Core.Domain.Shipping;
using Nop.Services.Configuration;

namespace Nop.Plugin.Misc.NopCliGeneric.Services
{
    public class NopCliGenericService : INopCliGenericService
    {
        #region Fields
 
        private readonly NopCliGenericSettings _NopCliGenericSettings; 

        #endregion

        #region Ctor

        public NopCliGenericService(ISettingService settingService, IStoreContext storeContext)
        { 
            _NopCliGenericSettings = settingService.LoadSetting<NopCliGenericSettings>(storeContext.ActiveStoreScopeConfiguration);
        }

        #endregion

        #region Methods
 
        public void SendNopCliGenericShip(ShipmentSentEvent eventShipment)
        {
            throw new NotImplementedException();
        }

        public void SendNopCliGenericPlaced(OrderPlacedEvent orderPlacedEvent)
        {
            throw new NotImplementedException();
        }

        public void SendNopCliGenericCancelled(OrderCancelledEvent orderCancelledEvent)
        {  
            throw new NotImplementedException();
        } 
        #endregion
    }
}