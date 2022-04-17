using Nop.Core.Configuration;

namespace Nop.Plugin.Misc.NopCliGeneric
{
    /// <summary>
    /// Represents a plugin settings
    /// </summary>
    public class NopCliGenericSettings : ISettings
    {
        public string  NopCliGenericToName { get; set; }
        public bool NotifyNopCliGenericTeam { get; set; }
        public string  NopCliGenericTeamEmails { get; set; }
        public bool NotifyCourierTeam { get; set; }
        public string  CourierTeamEmails { get; set; }
        public bool  CountryLimitedToStores { get; set; }
        public bool  CurrencyLimitedToStores { get; set; }
        public bool NotifyNopCliGenericTeamNewOrder { get; set; }
        public string  NopCliGenericTeamNewOrderEmails { get; set; }
        public bool NotifyNopCliGenericTeamCancelledOrder { get; set; }
        public string  NopCliGenericTeamCancelledOrderEmails { get; set; }
    }
}