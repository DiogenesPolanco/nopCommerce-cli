using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Misc.NopCliGeneric.Models;
using Nop.Services.Configuration;
using Nop.Services.Localization;
using Nop.Services.Messages;
using Nop.Web.Framework;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;

namespace Nop.Plugin.Misc.NopCliGeneric.Controllers
{
    [AutoValidateAntiforgeryToken]
    public class MiscNopCliGenericController : BasePluginController
    {
        #region Fields

        private readonly ILocalizationService _localizationService; 
        private readonly INotificationService _notificationService;
        private readonly ISettingService _settingService; 
        private readonly IStoreContext _storeContext; 

        #endregion

        #region Ctor

        public MiscNopCliGenericController( 
            ILocalizationService localizationService, 
            INotificationService notificationService,
            ISettingService settingService, 
            IStoreContext storeContext)
        { 
            _localizationService = localizationService; 
            _notificationService = notificationService;
            _settingService = settingService; 
            _storeContext = storeContext; 
        }

        #endregion

        #region Utilities

        /// <summary>
        /// Prepare SendinBlueModel
        /// </summary>
        /// <param name="model">Model</param>
        private async Task PrepareModelAsync(ConfigurationModel model)
        {
            //load settings for active store scope
            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var nopCliGenericSettings = await _settingService.LoadSettingAsync<NopCliGenericSettings>(storeId);

            model.NopCliGenericToName = nopCliGenericSettings.NopCliGenericToName; 
        }

        #endregion 
        
        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public async Task<IActionResult> Configure()
        {
            var model = new ConfigurationModel();
            await PrepareModelAsync(model);
            return View("~/Plugins/Misc.NopCliGeneric/Views/Configure.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        public async Task<IActionResult> Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return await Configure();

            var storeId = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var nopCliGenericSettings = await _settingService.LoadSettingAsync<NopCliGenericSettings>(storeId); 
            nopCliGenericSettings.NopCliGenericToName = model.NopCliGenericToName; 

            await _settingService.SaveSettingAsync(nopCliGenericSettings); 
            await _settingService.ClearCacheAsync();

            _notificationService.SuccessNotification(await _localizationService.GetResourceAsync("Admin.Plugins.Saved"));

            return await Configure();
        }

        #endregion
    }
}