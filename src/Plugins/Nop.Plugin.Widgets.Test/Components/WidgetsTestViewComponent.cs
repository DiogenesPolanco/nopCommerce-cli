using Nop.Core;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Web.Framework.Components;
using Nop.Plugin.Widgets.Test.Models;

namespace Nop.Plugin.Widgets.Test.Components
{
    [ViewComponent(Name = "WidgetsTest")]
    public class WidgetsTestViewComponent : NopViewComponent
    {
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;

        public WidgetsTestViewComponent(IStoreContext storeContext, ISettingService settingService)
        {
            _storeContext = storeContext;
            _settingService = settingService;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            var WidgetsTestSettings =
                _settingService.LoadSetting<WidgetsTestSettings>(_storeContext.CurrentStore.Id);

            var model = new PublicInfoModel
            {
                Property = WidgetsTestSettings.Property,
            };

            return View("~/Plugins/Widgets.Test/Views/PublicInfo.cshtml", model);
        }
    }
}