using System;
using System.Linq;
using Nop.Web.Framework;
using Nop.Services.Security;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using Nop.Core.Domain.Discounts;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Services.Configuration;
using Nop.Web.Framework.Controllers;
using Nop.Web.Framework.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Nop.Plugin.DiscountRules.NopCliGeneric.Models;

namespace Nop.Plugin.DiscountRules.NopCliGeneric.Controllers
{
    [AuthorizeAdmin]
    [Area(AreaNames.Admin)]
    [AutoValidateAntiforgeryToken]
    public class DiscountRulesNopCliGenericController : BasePluginController
    {
        #region Fields

        private readonly ICustomerService _customerService;
        private readonly IDiscountService _discountService;
        private readonly ILocalizationService _localizationService;
        private readonly IPermissionService _permissionService;
        private readonly ISettingService _settingService;

        #endregion

        #region Ctor

        public DiscountRulesNopCliGenericController(ICustomerService customerService,
            IDiscountService discountService,
            ILocalizationService localizationService,
            IPermissionService permissionService,
            ISettingService settingService)
        {
            _customerService = customerService;
            _discountService = discountService;
            _localizationService = localizationService;
            _permissionService = permissionService;
            _settingService = settingService;
        }

        #endregion

        #region Methods

        public async Task<IActionResult> Configure(int discountId, int? discountRequirementId)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
                return Content("Access denied");

            //load the discount
            var discount = await _discountService.GetDiscountByIdAsync(discountId);
            if (discount == null)
                throw new ArgumentException("Discount could not be loaded");

            //check whether the discount requirement exists
            if (discountRequirementId.HasValue && _discountService.GetDiscountRequirementByIdAsync(discountRequirementId.Value) is null)
                return Content("Failed to load requirement.");

            //try to get previously saved restricted customer role identifier
            var restrictedRoleId = await _settingService.GetSettingByKeyAsync<int>(string.Format(DiscountRequirementDefaults.SettingsKey, discountRequirementId ?? 0));

            var model = new RequirementModel
            {
                RequirementId = discountRequirementId ?? 0,
                DiscountId = discountId,
                NopCliGenericId = restrictedRoleId
            };

            //set available customer roles
            model.AvailableNopCliGeneric = (await _customerService.GetAllCustomerRolesAsync(true)).Select(role => new SelectListItem
            {
                Text = role.Name,
                Value = role.Id.ToString(),
                Selected = role.Id == restrictedRoleId
            }).ToList();
            model.AvailableNopCliGeneric.Insert(0, new SelectListItem
            {
                Text = await _localizationService.GetResourceAsync("Plugins.DiscountRules.NopCliGeneric.Fields.CustomerRole.Select"),
                Value = "0"
            });

            //set the HTML field prefix
            ViewData.TemplateInfo.HtmlFieldPrefix = string.Format(DiscountRequirementDefaults.HtmlFieldPrefix, discountRequirementId ?? 0);

            return View("~/Plugins/DiscountRules.NopCliGeneric/Views/Configure.cshtml", model);
        }

        [HttpPost]        
        public async Task<IActionResult> Configure(RequirementModel model)
        {
            if (!await _permissionService.AuthorizeAsync(StandardPermissionProvider.ManageDiscounts))
                return Content("Access denied");

            if (ModelState.IsValid)
            {
                //load the discount
                var discount = await _discountService.GetDiscountByIdAsync(model.DiscountId);
                if (discount == null)
                    return NotFound(new { Errors = new[] { "Discount could not be loaded" } });

                //get the discount requirement
                var discountRequirement = await _discountService.GetDiscountRequirementByIdAsync(model.RequirementId);

                //the discount requirement does not exist, so create a new one
                if (discountRequirement == null)
                {
                    discountRequirement = new DiscountRequirement
                    {
                        DiscountId = discount.Id,
                        DiscountRequirementRuleSystemName = DiscountRequirementDefaults.SystemName
                    };

                    await _discountService.InsertDiscountRequirementAsync(discountRequirement);
                }

                //save restricted customer role identifier
                await _settingService.SetSettingAsync(string.Format(DiscountRequirementDefaults.SettingsKey, discountRequirement.Id), model.NopCliGenericId);

                return Ok(new { NewRequirementId = discountRequirement.Id });
            }

            return BadRequest(new { Errors = GetErrorsFromModelState(ModelState) });
        }

        #endregion

        #region Utilities

        private IEnumerable<string> GetErrorsFromModelState(ModelStateDictionary modelState)
        {
            return ModelState.Values.SelectMany(v => v.Errors.Select(e => e.ErrorMessage));
        }

        #endregion
    }
}