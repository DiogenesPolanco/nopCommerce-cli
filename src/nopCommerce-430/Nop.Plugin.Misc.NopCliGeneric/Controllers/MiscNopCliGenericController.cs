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
        private void PrepareModel(ConfigurationModel model)
        {
            //load settings for active store scope
            var storeId = _storeContext.ActiveStoreScopeConfiguration;
            var NopCliGenericSettings = _settingService.LoadSetting<NopCliGenericSettings>(storeId);
 
            model.NopCliGenericToName = NopCliGenericSettings.NopCliGenericToName; 
        }

        #endregion 
        
        #region Methods

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        public IActionResult Configure()
        {
            var model = new ConfigurationModel();
            PrepareModel(model);
            return View("~/Plugins/Misc.NopCliGeneric/Views/Configure.cshtml", model);
        }

        [AuthorizeAdmin]
        [Area(AreaNames.Admin)]
        [HttpPost, ActionName("Configure")]
        [FormValueRequired("save")]
        public IActionResult Configure(ConfigurationModel model)
        {
            if (!ModelState.IsValid)
                return Configure();

            var storeId = _storeContext.ActiveStoreScopeConfiguration;
            var NopCliGenericSettings = _settingService.LoadSetting<NopCliGenericSettings>(storeId); 
            NopCliGenericSettings.NopCliGenericToName = model.NopCliGenericToName; 

            _settingService.SaveSetting(NopCliGenericSettings); 
            _settingService.ClearCache();

            _notificationService.SuccessNotification(_localizationService.GetResource("Admin.Plugins.Saved"));

            return Configure();
        }

        #endregion
    }
}