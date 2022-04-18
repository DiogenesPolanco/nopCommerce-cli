using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Core;
using Nop.Core.Infrastructure;
using Nop.Plugin.Widgets.NopCliGeneric.Components;
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Media;
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.NopCliGeneric
{
    /// <summary>
    /// Plugin
    /// </summary>
    public class WidgetsNopCliGenericPlugin : BasePlugin, IWidgetPlugin
    {
        private readonly ILocalizationService _localizationService; 
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper; 

        public WidgetsNopCliGenericPlugin(ILocalizationService localizationService, 
            ISettingService settingService,
            IWebHelper webHelper )
        {
            _localizationService = localizationService; 
            _settingService = settingService;
            _webHelper = webHelper; 
        }

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public Task<IList<string>> GetWidgetZonesAsync()
        {
            return Task.FromResult<IList<string>>(new List<string> { PublicWidgetZones.HomepageTop });
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/Widgets.NopCliGeneric/Configure";
        }

        /// <summary>
        /// Gets a name of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component name</returns>
        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "WidgetsWidgetsNopCliGeneric";
        }

         /// <summary>
        /// Gets a name of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component name</returns>
        public Type GetWidgetViewComponent(string widgetZone)
        {
            return typeof(WidgetsNopCliGenericViewComponent);
        }


        /// <summary>
        /// Install plugin
        /// </summary>
        public override async Task InstallAsync()
        { 
            //settings
            var settings = new WidgetsNopCliGenericSettings()
            { 
                Property = "NopCliGeneric"
            };
            await _settingService.SaveSettingAsync(settings);

            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.Widgets.WidgetsNopCliGeneric.Property"] = "NopCliGeneric", 
            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override async Task UninstallAsync()
        {
            //settings
            await _settingService.DeleteSettingAsync<WidgetsNopCliGenericSettings>();

            //locales
            await _localizationService.DeleteLocaleResourceAsync("Plugins.Widgets.WidgetsNopCliGeneric");

            await base.UninstallAsync();
        }

        /// <summary>
        /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
        /// </summary>
        public bool HideInWidgetList => false;
    }
}
