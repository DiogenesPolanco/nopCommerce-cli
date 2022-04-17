using Nop.Web.Framework.Models;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.Misc.NopCliGeneric.Models
{
    /// <summary>
    /// Represents a configuration model
    /// </summary>
    public record ConfigurationModel : BaseNopModel
    {
        #region Ctor

        public ConfigurationModel()
        { 
            
        }

        #endregion

        #region Properties

        [NopResourceDisplayName("plugins.Misc.NopCliGeneric.Fields.NopCliGenericToName")]
        public string  NopCliGenericToName { get; set; } 
        #endregion
    }
}