using Nop.Core;
using System.Linq;
using Nop.Core.Domain.Cms;
using Nop.Services.Common;
using Nop.Services.Plugins;
using Nop.Services.Messages;
using System.Threading.Tasks;
using Nop.Web.Framework.Menu;
using Nop.Services.Localization;
using Nop.Services.Configuration;
using System.Collections.Generic;
using Microsoft.AspNetCore.Routing;

namespace Nop.Plugin.Misc.NopCliGeneric
{
    /// <summary>
    /// Represents the NopCliGeneric plugin
    /// </summary>
    public class MiscNopCliGenericPlugin : BasePlugin, IMiscPlugin, IAdminMenuPlugin
    {
        #region Fields

        private readonly ILocalizationService _localizationService; 
        private readonly ISettingService _settingService; 
        private readonly IWebHelper _webHelper;
        private readonly IMessageTemplateService _messageTemplateService;
        private readonly WidgetSettings _widgetSettings;

        #endregion

        #region Ctor

        public MiscNopCliGenericPlugin( 
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
        public override async Task InstallAsync()
        { 
            //settings
            await _settingService.SaveSettingAsync(new NopCliGenericSettings { });

            if (!_widgetSettings.ActiveWidgetSystemNames.Contains(NopCliGenericDefaults.SystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Add(NopCliGenericDefaults.SystemName);
                await _settingService.SaveSettingAsync(_widgetSettings);
            }

            //locales
            await _localizationService.AddLocaleResourceAsync(new Dictionary<string, string>
            { 
                ["plugins.misc.NopCliGeneric"] = "NopCliGeneric store settings", 
                ["plugins.Misc.NopCliGeneric.Fields.NopCliGenericToName"] = "To Name"                 
            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override async Task UninstallAsync()
        {
            //settings
            if (_widgetSettings.ActiveWidgetSystemNames.Contains(NopCliGenericDefaults.SystemName))
            {
                _widgetSettings.ActiveWidgetSystemNames.Remove(NopCliGenericDefaults.SystemName);
                await _settingService.SaveSettingAsync(_widgetSettings);
            }
            await _settingService.DeleteSettingAsync<NopCliGenericSettings>();

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.Misc.NopCliGeneric");

            await base.UninstallAsync();
        }

        /// <summary>
        /// Manage sitemap. You can use "SystemName" of menu items to manage existing sitemap or add a new menu item.
        /// </summary>
        /// <param name="rootNode">Root node of the sitemap.</param>
        /// <returns>A task that represents the asynchronous operation</returns>
        public Task ManageSiteMapAsync(SiteMapNode rootNode)
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

            return Task.CompletedTask;
        }
        #endregion
    }
}