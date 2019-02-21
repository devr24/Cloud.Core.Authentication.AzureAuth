namespace Cloud.Core.Authentication.AzureAuth.Config
{
    using System;

    /// <summary>
    /// Msi authentication model.
    /// </summary>
    public class MsiAuth
    {
        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        /// <value>The tenant identifier.</value>
        public string TenantId { get; set; }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <exception cref="ArgumentException">TenantId must be set for Msi authentication</exception>
        public void Validate()
        {
            if (string.IsNullOrEmpty(TenantId))
                throw new ArgumentException("TenantId must be set for Msi authentication");
        }
    }
}
