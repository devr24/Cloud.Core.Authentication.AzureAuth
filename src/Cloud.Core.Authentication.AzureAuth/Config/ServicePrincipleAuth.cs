namespace Cloud.Core.Authentication.AzureAuth.Config
{
    using System;

    /// <summary>
    /// Service Principle Authentication model.
    /// </summary>
    public class ServicePrincipleAuth
    {
        /// <summary>
        /// Gets or sets the application identifier.
        /// </summary>
        /// <value>The application identifier.</value>
        public string AppId { get; set; }

        /// <summary>
        /// Gets or sets the application secret.
        /// </summary>
        /// <value>The application secret.</value>
        public string AppSecret { get; set; }

        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        /// <value>The tenant identifier.</value>
        public string TenantId { get; set; }

        /// <summary>
        /// Validates this instance has all mandatory fields.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// AppId must be set for Service Principle authentication
        /// or
        /// AppSecret must be set for Service Principle authentication
        /// or
        /// TenantId must be set for Service Principle authentication
        /// </exception>
        public void Validate()
        {
            if (string.IsNullOrEmpty(AppId))
                throw new ArgumentException("AppId must be set for Service Principle authentication");
            if (string.IsNullOrEmpty(AppSecret))
                throw new ArgumentException("AppSecret must be set for Service Principle authentication");
            if (string.IsNullOrEmpty(TenantId))
                throw new ArgumentException("TenantId must be set for Service Principle authentication");
        }
    }
}
