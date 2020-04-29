namespace Cloud.Core.Authentication.AzureAuth.Config
{
    using System.ComponentModel.DataAnnotations;
    using Validation;

    /// <summary>
    /// Service Principle Authentication model.
    /// </summary>
    public class ServicePrincipleAuth : AttributeValidator
    {
        /// <summary>
        /// Gets or sets the application identifier.
        /// </summary>
        /// <value>The application identifier.</value>
        [Required]
        public string AppId { get; set; }

        /// <summary>
        /// Gets or sets the application secret.
        /// </summary>
        /// <value>The application secret.</value>
        [Required]
        public string AppSecret { get; set; }

        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        /// <value>The tenant identifier.</value>
        [Required]
        public string TenantId { get; set; }
    }
}
