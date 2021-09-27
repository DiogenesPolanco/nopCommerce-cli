using Microsoft.AspNetCore.Mvc; 
using Nop.Web.Framework.Components;

namespace Nop.Plugin.Misc.NopCliGeneric.Components
{
    /// <summary>
    /// Represents view component to embed tracking script on pages
    /// </summary>
    [ViewComponent(Name = NopCliGenericDefaults.TRACKING_VIEW_COMPONENT_NAME)]
    public class WidgetsNopCliGenericViewComponent : NopViewComponent
    {
        #region Fields

        #endregion

        #region Ctor

        public WidgetsNopCliGenericViewComponent()
        { 
        }

        #endregion

        #region Methods

        /// <summary>
        /// Invoke view component
        /// </summary>
        /// <param name="widgetZone">Widget zone name</param>
        /// <param name="additionalData">Additional data</param>
        /// <returns>View component result</returns>
        public IViewComponentResult Invoke(string widgetZone, object additionalData)
        { 
            return View("~/Plugins/Misc.NopCliGeneric/Views/PublicInfo.cshtml");
        }

        #endregion
    }
}