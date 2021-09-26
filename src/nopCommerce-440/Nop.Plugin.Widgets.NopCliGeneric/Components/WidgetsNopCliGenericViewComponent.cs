using System;
using System.Net;
using System.Linq;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Nop.Web.Framework.Components;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Widgets.NopCliGeneric.Models;

namespace Nop.Plugin.Widgets.NopCliGeneric.Components
{
    [ViewComponent(Name = "WidgetsNopCliGeneric")]
    public class WidgetsNopCliGenericViewComponent : NopViewComponent
    {
        public IViewComponentResult Invoke()
        {
            string model = "change the model you want";
            
            return View("~/Plugins/Widgets.NopCliGeneric/Views/PublishInfo.cshtml", model);
        }
    }
}
