using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Widgets.Generic.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Services.Security;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;

namespace Nop.Plugin.Widgets.Generic.Controllers
{
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class WidgetsGenericController : BasePluginController
    {
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService; 
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        public WidgetsGenericController(ILocalizationService localizationService,
            INotificationService notificationService,
            IPermissionService permissionService,  
            ISettingService settingService,
            IStoreContext storeContext)
        {
            _localizationService = localizationService;
            _notificationService = notificationService;
            _permissionService = permissionService; 
            _settingService = settingService;
            _storeContext = storeContext;
        }

        public IActionResult Configure()
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var WidgetsGenericSettings = _settingService.LoadSetting<WidgetsGenericSettings>(storeScope);
            var model = new ConfigurationModel
            {
                Property = WidgetsGenericSettings.Property,
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                model.Property_OverrideForStore = _settingService.SettingExists(WidgetsGenericSettings, x => x.Property, storeScope);
              
            }

            return View("~/Plugins/Widgets.Generic/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var WidgetsGenericSettings = _settingService.LoadSetting<WidgetsGenericSettings>(storeScope);
  
            WidgetsGenericSettings.Property = model.Property; 
            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(WidgetsGenericSettings, x => x.Property, model.Property_OverrideForStore, storeScope, false); 
            //now clear settings cache
            _settingService.ClearCache();
             
            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            return Configure();
        }
    }
}