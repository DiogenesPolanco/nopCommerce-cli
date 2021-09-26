using System.Collections.Generic;
using Nop.Core; 
using Nop.Services.Cms;
using Nop.Services.Configuration;
using Nop.Services.Localization; 
using Nop.Services.Plugins;
using Nop.Web.Framework.Infrastructure;

namespace Nop.Plugin.Widgets.Test
{
    /// <summary>
    /// PLugin
    /// </summary>
    public class WidgetsTestPlugin : BasePlugin, IWidgetPlugin
    {
        private readonly ILocalizationService _localizationService; 
        private readonly ISettingService _settingService;
        private readonly IWebHelper _webHelper; 

        public WidgetsTestPlugin(ILocalizationService localizationService, 
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
            return _webHelper.GetStoreLocation() + "Admin/WidgetsWidgetsTest/Configure";
        }

        /// <summary>
        /// Gets a name of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component name</returns>
        public string GetWidgetViewComponentName(string widgetZone)
        {
            return "WidgetsWidgetsTest";
        }

        /// <summary>
        /// Install plugin
        /// </summary>
        public override void Install()
        { 
            //settings
            var settings = new WidgetsTestSettings
            { 
                Property = "Test"
            };
            _settingService.SaveSetting(settings);

            _localizationService.AddPluginLocaleResource(new Dictionary<string, string>
            {
                ["Plugins.Widgets.WidgetsTest.Property"] = "Test", 
            });

            base.Install();
        }

        /// <summary>
        /// Uninstall plugin
        /// </summary>
        public override void Uninstall()
        {
            //settings
            _settingService.DeleteSetting<WidgetsTestSettings>();

            //locales
            _localizationService.DeletePluginLocaleResources("Plugins.Widgets.WidgetsTest");

            base.Uninstall();
        }

        /// <summary>
        /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
        /// </summary>
        public bool HideInWidgetList => false;
    }
}