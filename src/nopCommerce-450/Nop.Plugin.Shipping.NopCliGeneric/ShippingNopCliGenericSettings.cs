using Nop.Core.Configuration;

namespace Nop.Plugin.Shipping.NopCliGeneric
{
    public class ShippingNopCliGenericSettings : ISettings
    {
        /// <summary>
        /// API key
        /// </summary>
        public string ApiKey { get; set; }

        /// <summary>
        /// API secret
        /// </summary>
        public string ApiSecret { get; set; }

        /// <summary>
        /// Set to true if need pass dimensions to the ShippingNopCliGeneric server
        /// </summary>
        public bool PassDimensions { get; set; }

        /// <summary>
        /// Packing type
        /// </summary>
        public PackingType PackingType { get; set; }
        
        /// <summary>
        /// Package volume
        /// </summary>
        public int PackingPackageVolume { get; set; }

        /// <summary>
        /// ShippingNopCliGeneric user name
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// ShippingNopCliGeneric password
        /// </summary>
        public string Password { get; set; }
    }
}