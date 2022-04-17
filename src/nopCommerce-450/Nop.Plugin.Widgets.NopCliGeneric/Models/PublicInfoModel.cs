using Nop.Web.Framework.Models;

namespace Nop.Plugin.Widgets.NopCliGeneric.Models
{
    public record PublicInfoModel : BaseNopModel
    {
        public string Property { get; set; } 
    }
}