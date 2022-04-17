﻿using System;
using Nop.Core;
using System.Linq;
using Nop.Services.Plugins;
using Nop.Services.Customers;
using Nop.Services.Discounts;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Localization;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nop.Services.Configuration;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.AspNetCore.Mvc.Infrastructure;

namespace Nop.Plugin.DiscountRules.NopCliGeneric
{
    public partial class CustomerRoleDiscountRequirementRule : BasePlugin, IDiscountRequirementRule
    {
        #region Fields

        private readonly IActionContextAccessor _actionContextAccessor;
        private readonly ICustomerService _customerService;
        private readonly IDiscountService _discountService;
        private readonly ILocalizationService _localizationService;
        private readonly ISettingService _settingService;
        private readonly IUrlHelperFactory _urlHelperFactory;
        private readonly IWebHelper _webHelper;

        #endregion

        #region Ctor

        public CustomerRoleDiscountRequirementRule(IActionContextAccessor actionContextAccessor,
            IDiscountService discountService,
            ICustomerService customerService,
            ILocalizationService localizationService,
            ISettingService settingService,
            IUrlHelperFactory urlHelperFactory,
            IWebHelper webHelper)
        {
            _actionContextAccessor = actionContextAccessor;
            _customerService = customerService;
            _discountService = discountService;
            _localizationService = localizationService;
            _settingService = settingService;
            _urlHelperFactory = urlHelperFactory;
            _webHelper = webHelper;
        }

        #endregion

        #region Methods

        /// <summary>
        /// Check discount requirement
        /// </summary>
        /// <param name="request">Object that contains all information required to check the requirement (Current customer, discount, etc)</param>
        /// <returns>Result</returns>
        public async Task<DiscountRequirementValidationResult> CheckRequirementAsync(DiscountRequirementValidationRequest request)
        {
            if (request == null)
                throw new ArgumentNullException(nameof(request));

            //invalid by default
            var result = new DiscountRequirementValidationResult();

            if (request.Customer == null)
                return result;

            //try to get saved restricted customer role identifier
            var restrictedRoleId = await _settingService.GetSettingByKeyAsync<int>(string.Format(DiscountRequirementDefaults.SettingsKey, request.DiscountRequirementId));
            if (restrictedRoleId == 0)
                return result;

            //result is valid if the customer belongs to the restricted role
            result.IsValid = (await _customerService.GetCustomerRolesAsync(request.Customer)).Any(role => role.Id == restrictedRoleId);

            return result;
        }

        /// <summary>
        /// Get URL for rule configuration
        /// </summary>
        /// <param name="discountId">Discount identifier</param>
        /// <param name="discountRequirementId">Discount requirement identifier (if editing)</param>
        /// <returns>URL</returns>
        public string GetConfigurationUrl(int discountId, int? discountRequirementId)
        {
            
            var urlHelper = _urlHelperFactory.GetUrlHelper(_actionContextAccessor.ActionContext);

            return urlHelper.Action("Configure", "DiscountRulesNopCliGeneric",
                new { discountId = discountId, discountRequirementId = discountRequirementId }, _webHelper.GetCurrentRequestProtocol());
        }

        /// <summary>
        /// Install the plugin
        /// </summary>
        public override async Task InstallAsync()
        {
            //locales
            await _localizationService.AddOrUpdateLocaleResourceAsync(new Dictionary<string, string>
            {
                ["Plugins.DiscountRules.NopCliGeneric.Fields.CustomerRole"] = "Required customer role",
                ["Plugins.DiscountRules.NopCliGeneric.Fields.CustomerRole.Hint"] = "Discount will be applied if customer is in the selected customer role.",
                ["Plugins.DiscountRules.NopCliGeneric.Fields.CustomerRole.Select"] = "Select customer role",
                ["Plugins.DiscountRules.NopCliGeneric.Fields.CustomerRoleId.Required"] = "Customer role is required",
                ["Plugins.DiscountRules.NopCliGeneric.Fields.DiscountId.Required"] = "Discount is required"
            });

            await base.InstallAsync();
        }

        /// <summary>
        /// Uninstall the plugin
        /// </summary>
        public override async Task UninstallAsync()
        {
            //discount requirements
            var discountRequirements = (await _discountService.GetAllDiscountRequirementsAsync())
                .Where(discountRequirement => discountRequirement.DiscountRequirementRuleSystemName == DiscountRequirementDefaults.SystemName);
            foreach (var discountRequirement in discountRequirements)
            {
                await _discountService.DeleteDiscountRequirementAsync(discountRequirement, false);
            }

            //locales
            await _localizationService.DeleteLocaleResourcesAsync("Plugins.DiscountRules.NopCliGeneric");

            await base.UninstallAsync();
        }

        #endregion
    }
}