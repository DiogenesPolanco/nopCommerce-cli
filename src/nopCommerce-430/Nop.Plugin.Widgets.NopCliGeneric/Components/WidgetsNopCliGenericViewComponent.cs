using Nop.Core;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using Nop.Web.Framework.Components;
using Nop.Plugin.Widgets.NopCliGeneric.Models;

namespace Nop.Plugin.Widgets.NopCliGeneric.Components
{
    [ViewComponent(Name = "WidgetsNopCliGeneric")]
    public class WidgetsNopCliGenericViewComponent : NopViewComponent
    {
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;

        public WidgetsNopCliGenericViewComponent(IStoreContext storeContext, ISettingService settingService)
        {
            _storeContext = storeContext;
            _settingService = settingService;
        }

        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        {
            var WidgetsNopCliGenericSettings =
                _settingService.LoadSetting<WidgetsNopCliGenericSettings>(_storeContext.CurrentStore.Id);

            var model = new PublicInfoModel
            {
                Property = WidgetsNopCliGenericSettings.Property,
            };

            return View("~/Plugins/Widgets.NopCliGeneric/Views/PublicInfo.cshtml", model);
        }
    }
}