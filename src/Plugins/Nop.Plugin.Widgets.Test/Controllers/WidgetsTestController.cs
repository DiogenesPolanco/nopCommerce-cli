using Nop.Core;
using Nop.Web.Framework;
using Nop.Services.Messages;
using Nop.Services.Security;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using Nop.Services.Configuration;
using Nop.Web.Framework.Controllers;
using Nop.Plugin.Widgets.Test.Models;

namespace Nop.Plugin.Widgets.Test.Controllers
{
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class WidgetsTestController : BasePluginController
    {
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService; 
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        public WidgetsTestController(ILocalizationService localizationService,
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
            var WidgetsTestSettings = _settingService.LoadSetting<WidgetsTestSettings>(storeScope);
            var model = new ConfigurationModel
            {
                Property = WidgetsTestSettings.Property,
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                model.Property_OverrideForStore = _settingService.SettingExists(WidgetsTestSettings, x => x.Property, storeScope);
            }

            return View("~/Plugins/Widgets.Test/Views/Configure.cshtml", model);
        }

        [HttpPost]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!_permissionService.Authorize(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = _storeContext.ActiveStoreScopeConfiguration;
            var WidgetsTestSettings = _settingService.LoadSetting<WidgetsTestSettings>(storeScope);

            WidgetsTestSettings.Property = model.Property; 
            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            _settingService.SaveSettingOverridablePerStore(WidgetsTestSettings, x => x.Property, model.Property_OverrideForStore, storeScope, false); 
            //now clear settings cache
            _settingService.ClearCache();

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));
            return Configure();
        }
    }
}