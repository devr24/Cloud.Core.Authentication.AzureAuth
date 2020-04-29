namespace Cloud.Core.Authentication.AzureAuth.Config
{
    using System.ComponentModel.DataAnnotations;
    using Validation;

    /// <summary>
    /// Msi authentication model.
    /// </summary>
    public class MsiAuth : AttributeValidator
    {
        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        /// <value>The tenant identifier.</value>
        [Required]
        public string TenantId { get; set; }
    }
}
