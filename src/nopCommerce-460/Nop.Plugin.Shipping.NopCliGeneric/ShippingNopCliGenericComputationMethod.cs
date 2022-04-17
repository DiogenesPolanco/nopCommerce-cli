using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Domain.Shipping;
using Nop.Plugin.Shipping.NopCliGeneric.Services;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Plugins;
using Nop.Services.Shipping;
using Nop.Services.Shipping.Tracking;

namespace Nop.Plugin.Shipping.NopCliGeneric
{
    /// <summary>
    /// Fixed rate or by weight shipping computation method 
    /// </summary>
    public class ShippingNopCliGenericComputationMethod : BasePlugin, IShippingRateComputationMethod, IMiscPlugin
    {
        #region Fields

        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IShippingNopCliGenericService _shippingNopCliGenericService;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public ShippingNopCliGenericComputationMethod(ILocalizationService localizationService,
            ISettingService settingService,
            IShippingNopCliGenericService shippingNopCliGenericService,
            IWebHelper webHelper)
        {
            _localizationService = localizationService;
            _settingService = settingService;
            _shippingNopCliGenericService = shippingNopCliGenericService;
            _webHelper = webHelper;
        }

        #endregion

        #region Utilities

        #endregion

        #region Methods

        /// <summary>
        /// Gets fixed shipping rate (if shipping rate computation method allows it and the rate can be calculated before checkout).
        /// </summary>
        /// <param name="getShippingOptionRequest">A request for getting shipping options</param>
        /// <returns>Fixed shipping rate; or null in case there's no fixed shipping rate</returns>
        public decimal? GetFixedRate(GetShippingOptionRequest getShippingOptionRequest)
        {
            if (getShippingOptionRequest == null)
                throw new ArgumentNullException(nameof(getShippingOptionRequest));

            return null;
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/ShippingNopCliGeneric/Configure";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override async Task InstallAsync()
        {
            //settings
            var settings = new ShippingNopCliGenericSettings
            {
                PackingPackageVolume = 5184
            };
            await _settingService.SaveSettingAsync(settings);

            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Enums.Nop.Plugin.Shipping.NopCliGeneric.PackingType.PackByDimensions"] = "Pack by dimensions",
                ["Enums.Nop.Plugin.Shipping.NopCliGeneric.PackingType.PackByVolume"] = "Pack by volume",
                ["Plugins.Shipping.NopCliGeneric.Fields.ApiKey.Hint"] = "Specify ShippingNopCliGeneric API key.",
                ["Plugins.Shipping.NopCliGeneric.Fields.ApiKey"] = "API key",
                ["Plugins.Shipping.NopCliGeneric.Fields.ApiSecret.Hint"] = "Specify ShippingNopCliGeneric API secret.",
                ["Plugins.Shipping.NopCliGeneric.Fields.ApiSecret"] = "API secret",
                ["Plugins.Shipping.NopCliGeneric.Fields.PackingPackageVolume.Hint"] = "Enter your package volume.",
                ["Plugins.Shipping.NopCliGeneric.Fields.PackingPackageVolume"] = "Package volume",
                ["Plugins.Shipping.NopCliGeneric.Fields.PackingType.Hint"] = "Choose preferred packing type.",
                ["Plugins.Shipping.NopCliGeneric.Fields.PackingType"] = "Packing type",
                ["Plugins.Shipping.NopCliGeneric.Fields.Password.Hint"] = "Specify ShippingNopCliGeneric password",
                ["Plugins.Shipping.NopCliGeneric.Fields.Password"] = "Password",
                ["Plugins.Shipping.NopCliGeneric.Fields.PassDimensions.Hint"] = "Check if need send dimensions to the ShippingNopCliGeneric server",
                ["Plugins.Shipping.NopCliGeneric.Fields.PassDimensions"] = "Pass dimensions",
                ["Plugins.Shipping.NopCliGeneric.Fields.UserName"] = "User name",
                ["Plugins.Shipping.NopCliGeneric.Fields.UserName.Hint"] = "Specify ShippingNopCliGeneric user name"
            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<ShippingNopCliGenericSettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Enums.Nop.Plugin.Shipping.NopCliGeneric");
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Shipping.NopCliGeneric");

            await base.UninstallAsync();
        }

        #endregion

        #region Properties

        public async Task<GetShippingOptionResponse> GetShippingOptionsAsync(GetShippingOptionRequest getShippingOptionRequest)
        {
            if (getShippingOptionRequest == null)
                throw new ArgumentNullException(nameof(getShippingOptionRequest));

            var response = new GetShippingOptionResponse();

            if (getShippingOptionRequest.Items == null)
                response.AddError("No shipment items");

            if (getShippingOptionRequest.ShippingAddress == null)
                response.AddError("Shipping address is not set");

            if ((getShippingOptionRequest.ShippingAddress?.CountryId ?? 0) == 0)
                response.AddError("Shipping country is not set");

            if (!response.Success)
                return response;

            try
            {
                foreach (var rate in await _shippingNopCliGenericService.GetAllRatesAsync(getShippingOptionRequest))
                {
                    response.ShippingOptions.Add(new ShippingOption
                    {
                        Description = rate.ServiceCode,
                        Name = rate.ServiceName,
                        Rate = rate.TotalCost
                    });
                }
            }
            catch (Exception e)
            {
                response.Errors.Add(e.Message);
            }

            return response;
        }

        public Task<decimal?> GetFixedRateAsync(GetShippingOptionRequest getShippingOptionRequest)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Gets a shipment tracker
        /// </summary>
        public IShipmentTracker ShipmentTracker => null;

        #endregion
    }
}