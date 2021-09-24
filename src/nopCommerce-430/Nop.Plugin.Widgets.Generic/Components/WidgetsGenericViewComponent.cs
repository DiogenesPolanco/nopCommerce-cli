using Microsoft.AspNetCore.Mvc;
using Nop.Core;
using Nop.Plugin.Widgets.Generic.Models;
using Nop.Services.Configuration;
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Widgets.Generic.Components
{
    [ViewComponent(Name = "WidgetsGeneric")]
    public class WidgetsGenericViewComponent : NopViewComponent
    {
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;

        public WidgetsGenericViewComponent(IStoreContext storeContext, ISettingService settingService)
        {
            _storeContext = storeContext;
            _settingService = settingService;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            var WidgetsGenericSettings =
                _settingService.LoadSetting<WidgetsGenericSettings>(_storeContext.CurrentStore.Id);

            var model = new PublicInfoModel
            {
                Property = WidgetsGenericSettings.Property,
            };

            return View("~/Plugins/Widgets.Generic/Views/PublicInfo.cshtml", model);
        }
    }
}