using FluentValidation;
using Nop.Plugin.DiscountRules.NopCliGeneric.Models;
using Nop.Services.Localization;
using Nop.Web.Framework.Validators;

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
                .WithMessage(localizationService.GetResource("Plugins.DiscountRules.NopCliGeneric.Fields.DiscountId.Required"));
            RuleFor(model => model.NopCliGenericId)
                .NotEmpty()
                .WithMessage(localizationService.GetResource("Plugins.DiscountRules.NopCliGeneric.Fields.NopCliGenericId.Required"));
        }
    }
}
