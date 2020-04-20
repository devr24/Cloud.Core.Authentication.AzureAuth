namespace Cloud.Core.Authentication.AzureAuth.Config
{
    using System.ComponentModel.DataAnnotations;
    using System.Security.Cryptography.X509Certificates;
    using Validation;

    /// <summary>
    /// Certificate Authentication Model
    /// </summary>
    public class CertAuth : AttributeValidator
    {
        /// <summary>
        /// Gets or sets the application identifier.
        /// </summary>
        /// <value>The application identifier.</value>
        [Required]
        public string AppId { get; set; }

        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        /// <value>The tenant identifier.</value>
        [Required]
        public string TenantName { get; set; }

        /// <summary>
        /// Gets or sets the certificate to use for Authentication
        /// </summary>
        /// <value>The certificate to use in combination with the AppId to authenticate with Azure.</value>
        [Required]
        public X509Certificate2 Certificate { get; set; }

        /// <summary>
        /// Gets or sets the Target URI for the Authentication Request
        /// </summary>
        /// <value>The Target URI for the Authentication Request</value>
        [Required]
        public string TargetUri { get; set; }
    }
}
