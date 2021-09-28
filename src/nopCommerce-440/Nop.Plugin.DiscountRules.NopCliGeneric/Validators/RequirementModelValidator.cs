using FluentValidation;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;
using Nop.Plugin.DiscountRules.NopCliGeneric.Models;

namespace Nop.Plugin.DiscountRules.NopCliGeneric.Validators
{
    /// <summary>
    /// Represents an <see cref="RequirementModel"/> validator.
    /// </summary>
    public class RequirementModelValidator : BaseNopValidator<RequirementModel>
    {
        public RequirementModelValidator(ILocalizationService localizationService)
        {
            RuleFor(model => model.DiscountId)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.DiscountRules.NopCliGeneric.Fields.DiscountId.Required"));
            RuleFor(model => model.NopCliGenericId)
                .NotEmpty()
                .WithMessageAwait(localizationService.GetResourceAsync("Plugins.DiscountRules.NopCliGeneric.Fields.NopCliGenericId.Required"));
        }
    }
}
