using Nop.Core;

namespace Nop.Plugin.Misc.NopCliGeneric
{
    /// <summary>
    /// Represents plugin constants
    /// </summary>
    public static class NopCliGenericDefaults
    {
        /// <summary>
        /// Gets a name of the view component to embed tracking script on pages
        /// </summary>
        public const string TRACKING_VIEW_COMPONENT_NAME = "WidgetsNopCliGeneric"; 

        /// <summary>
        /// Gets a plugin system name
        /// </summary>
        public static string SystemName => "Misc.NopCliGeneric";

        /// <summary>
        /// Gets a plugin partner name
        /// </summary>
        public static string PartnerName => "NOPCOMMERCE";

        /// <summary>
        /// Gets a user agent used to request NopCliGeneric services
        /// </summary>
        public static string UserAgent => $"nopCommerce-{NopVersion.CurrentVersion}";
 
    }
}