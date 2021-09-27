using System.Collections.Generic;
using System.Linq;
using Microsoft.AspNetCore.Routing;
using Nop.Core;
using Nop.Core.Domain.Cms;
using Nop.Services.Cms;
using Nop.Services.Common;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Plugins;
using Nop.Web.Framework.Menu;

namespace Nop.Plugin.Misc.NopCliGeneric
{
    /// <summary>
    /// Represents the NopCliGeneric plugin
    /// </summary>
    public class NopCliGenericPlugin : BasePlugin, IMiscPlugin, IWidgetPlugin, IAdminMenuPlugin
    {
        #region Fields
 
        private readonly ILocalizationService _localizationService; 
        private readonly ISettingService _settingService; 
        private readonly IWebHelper _webHelper;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly WidgetSettings _widgetSettings;

        #endregion

        #region Ctor

        public NopCliGenericPlugin( 
            ILocalizationService localizationService, 
            ISettingService settingService, 
            IWebHelper webHelper,
            IMessageTemplateService messageTemplateService,
            WidgetSettings widgetSettings)
        { 
            _localizationService = localizationService; 
            _settingService = settingService; 
            _webHelper = webHelper;
            _widgetSettings = widgetSettings;
            _messageTemplateService = messageTemplateService;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Gets widget zones where this widget should be rendered
        /// </summary>
        /// <returns>Widget zones</returns>
        public IList<string> GetWidgetZones()
        {
            return new List<string> {   };
        }

        /// <summary>
        /// Gets a name of a view component for displaying widget
        /// </summary>
        /// <param name="widgetZone">Name of the widget zone</param>
        /// <returns>View component name</returns>
        public string GetWidgetViewComponentName(string widgetZone)
        {
            return NopCliGenericDefaults.TRACKING_VIEW_COMPONENT_NAME;
        }

        /// <summary>
        /// Gets a configuration page URL
        /// </summary>
        public override string GetConfigurationPageUrl()
        {
            return $"{_webHelper.GetStoreLocation()}Admin/NopCliGeneric/Configure";
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override void Install()
        { 
            //settings
            _settingService.SaveSetting(new NopCliGenericSettings { });

            if (!_widgetSettings.ActiveWidgetSystemNames.Contains(NopCliGenericDefaults.SystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Add(NopCliGenericDefaults.SystemName);
                _settingService.SaveSetting(_widgetSettings);
            }
 
            //locales
            _localizationService.AddPluginLocaleResource(new Dictionary<string, string>
            { 
                ["plugins.misc.NopCliGeneric"] = "NopCliGeneric store settings", 
                ["plugins.Misc.NopCliGeneric.Fields.NopCliGenericToName"] = "To Name"                 
            });

            base.Install();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override void Uninstall()
        {
           
            //settings
            if (_widgetSettings.ActiveWidgetSystemNames.Contains(NopCliGenericDefaults.SystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Remove(NopCliGenericDefaults.SystemName);
                _settingService.SaveSetting(_widgetSettings);
            }
            _settingService.DeleteSetting<NopCliGenericSettings>();
 
 
            //locales
            _localizationService.DeletePluginLocaleResources("Plugins.Misc.NopCliGeneric");

            base.Uninstall();
        }

        #endregion
        public void ManageSiteMap(SiteMapNode rootNode)
        {
            var menuItem = new SiteMapNode()
            {
                SystemName = "Misc.NopCliGeneric",
                Title = "NopCliGeneric Settings",
                ControllerName = "NopCliGeneric",
                ActionName = "Configure",
                IconClass = "fa-dot-circle-o",
                Visible = true,
                RouteValues = new RouteValueDictionary() { { "area", "Admin" } },
            };
            var pluginNode = rootNode.ChildNodes.FirstOrDefault(x => x.SystemName == "Configuration");
            if(pluginNode != null)
                pluginNode.ChildNodes.Add(menuItem);
            else
                rootNode.ChildNodes.Add(menuItem); 
        }
        /// <summary>
        /// Gets a value indicating whether to hide this plugin on the widget list page in the admin area
        /// </summary>
        public bool HideInWidgetList => true;
    }
}