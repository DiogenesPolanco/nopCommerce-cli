using Nop.Core;
using Nop.Web.Framework;
using Nop.Services.Messages;
using Nop.Services.Security;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using Nop.Services.Configuration;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Nop.Plugin.Widgets.NopCliGeneric.Models;

namespace Nop.Plugin.Widgets.NopCliGeneric.Controllers
{
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class WidgetsNopCliGenericController : BasePluginController
    {
        private readonly ILocalizationService _localizationService;
        private readonly INotificationService _notificationService;
        private readonly IPermissionService _permissionService; 
        private readonly ISettingService _settingService;
        private readonly IStoreContext _storeContext;

        public WidgetsNopCliGenericController(ILocalizationService localizationService,
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

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        // returns A task that represents the asynchronous operation
        public async Task<IActionResult> Configure()
        {
            if (! await _permissionService.AuthorizeAsync((StandardPermissionProvider.ManageWidgets)))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var widgetsNopCliGenericSettings = await _settingService.LoadSettingAsync<WidgetsNopCliGenericSettings>(storeScope);
            var model = new ConfigurationModel
            {
                Property = widgetsNopCliGenericSettings.Property,
                ActiveStoreScopeConfiguration = storeScope
            };

            if (storeScope > 0)
            {
                model.Property_OverrideForStore = await _settingService.SettingExistsAsync(widgetsNopCliGenericSettings, x => x.Property, storeScope);
            }

            return View("~/Plugins/Widgets.NopCliGeneric/Views/Configure.cshtml", model);
        }

        [HttpPost]
        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (! await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageWidgets))
                return AccessDeniedView();

            //load settings for a chosen store scope
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var widgetsNopCliGenericSettings = await _settingService.LoadSettingAsync<WidgetsNopCliGenericSettings>(storeScope);

            widgetsNopCliGenericSettings.Property = model.Property; 
            /* We do not clear cache after each setting update.
             * This behavior can increase performance because cached settings will not be cleared 
             * and loaded from database after each update */
            await _settingService.SaveSettingOverridablePerStoreAsync(widgetsNopCliGenericSettings, x => x.Property, model.Property_OverrideForStore, storeScope, false); 
            //now clear settings cache
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));
            return await Configure();
        }
    }
}