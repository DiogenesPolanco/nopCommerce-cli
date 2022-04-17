using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Web.Framework.Mvc.ModelBinding;

namespace Nop.Plugin.DiscountRules.NopCliGeneric.Models
{
    public class RequirementModel
    {
        public RequirementModel()
        {
            AvailableNopCliGeneric = new List<SelectListItem>();
        }

        [NopResourceDisplayName("Plugins.DiscountRules.NopCliGeneric.Fields.NopCliGeneric")]
        public int NopCliGenericId { get; set; }

        public int DiscountId { get; set; }

        public int RequirementId { get; set; }

        public IList<SelectListItem> AvailableNopCliGeneric { get; set; }
    }
}