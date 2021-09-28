using System;
using Nop.Core;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Nop.Services.Configuration;
using System.Collections.Generic;
using Nop.Web.Framework.Components;
using Microsoft.AspNetCore.Mvc.Rendering;
using Nop.Plugin.Payments.NopCliGeneric.Models;

namespace Nop.Plugin.Payments.NopCliGeneric.Components
{
    [ViewComponent(Name = "PaymentNopCliGeneric")]
    public class PaymentNopCliGenericViewComponent : NopViewComponent
    {
        private readonly IStoreContext _storeContext;
        private readonly ISettingService _settingService;

        public PaymentNopCliGenericViewComponent(IStoreContext storeContext, ISettingService settingService)
        {
            _storeContext = storeContext;
            _settingService = settingService;
        } 
        
        public async Task<IViewComponentResult> InvokeAsync()
        {
            var storeScope = await _storeContext.GetActiveStoreScopeConfigurationAsync();
            var nopCliGenericPaymentSettings = await _settingService.LoadSettingAsync<NopCliGenericPaymentSettings>(storeScope);

            var result = new PaymentInfoModel()
            {
                IsStandard = nopCliGenericPaymentSettings.IsStandard,
                CreditCardTypes = nopCliGenericPaymentSettings.IsStandard
                    ? new List<SelectListItem>()
                    : new List<SelectListItem>
                    {
                        new SelectListItem { Text = "Visa", Value = "visa" },
                        new SelectListItem { Text = "Master card", Value = "MasterCard" },
                        new SelectListItem { Text = "Discover", Value = "Discover" },
                        new SelectListItem { Text = "Amex", Value = "Amex" },
                    }
            };

            if (nopCliGenericPaymentSettings.IsStandard)
                return View("~/Plugins/Payments.NopCliGeneric/Views/PaymentInfo.cshtml", result);

            //years
            for (var i = 0; i < 15; i++)
            {
                var year = (DateTime.Now.Year + i).ToString();
                result.ExpireYears.Add(new SelectListItem { Text = year, Value = year, });
            }

            //months
            for (var i = 1; i <= 12; i++)
            {
                result.ExpireMonths.Add(new SelectListItem { Text = i.ToString("D2"), Value = i.ToString(), });
            }

            return View("~/Plugins/Payments.NopCliGeneric/Views/PaymentInfo.cshtml", result);
        }
    }
}