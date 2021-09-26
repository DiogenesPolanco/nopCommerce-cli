using Nop.Core;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Web.Framework.Components;
using Nop.Plugin.Widgets.NopcliGeneric.Models;

namespace Nop.Plugin.Widgets.NopcliGeneric.Components
{
    [ViewComponent(Name = "WidgetsNopcliGeneric")]
    public class WidgetsNopcliGenericViewComponent : NopViewComponent
    {
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;

        public WidgetsNopcliGenericViewComponent(IStoreContext storeContext, ISettingService settingService)
        {
            _storeContext = storeContext;
            _settingService = settingService;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            var WidgetsNopcliGenericSettings =
                _settingService.LoadSetting<WidgetsNopcliGenericSettings>(_storeContext.CurrentStore.Id);

            var model = new PublicInfoModel
            {
                Property = WidgetsNopcliGenericSettings.Property,
            };

            return View("~/Plugins/Widgets.NopcliGeneric/Views/PublicInfo.cshtml", model);
        }
    }
}