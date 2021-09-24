using System.Collections.Generic;
using Nop.Core; 
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization; 
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.Generic
{
    /// <summary>
    /// PLugin
    /// </summary>
    public class WidgetsGenericPlugin : BasePlugin, IWidgetPlugin
    {
        private readonly ILocalizationService _localizationService; 
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper; 

        public WidgetsGenericPlugin(ILocalizationService localizationService, 
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
        public IList<string> GetWidgetZones()
        {
            return new List<string> { PublicWidgetZones.HomepageTop };
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return _webHelper.GetStoreLocation() + "Admin/WidgetsWidgetsGeneric/Configure";
        }

        /// <summary>
        /// Gets a name of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component name</returns>
        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "WidgetsWidgetsGeneric";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        { 
            //settings
            var settings = new WidgetsGenericSettings
            { 
                Property = "Generic"
            };
            _settingService.SaveSetting(settings);

            _localizationService.AddPluginLocaleResource(new Dictionary<string, string>
            {
                ["Plugins.Widgets.WidgetsGeneric.Property"] = "Generic", 
            });

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<WidgetsGenericSettings>();

            //locales
            _localizationService.DeletePluginLocaleResources("Plugins.Widgets.WidgetsGeneric");

            base.Uninstall();
        }

        /// <summary>
        /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
        /// </summary>
        public bool HideInWidgetList => false;
    }
}