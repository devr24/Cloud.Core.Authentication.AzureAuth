namespace Cloud.Core.Authentication.AzureAuth.Config
{
    using System.ComponentModel.DataAnnotations;

    /// <summary>
    /// User Authentication model.
    /// </summary>
    public class UserAuth: Validation.AttributeValidator
    {
        /// <summary>
        /// Gets or sets the user's username
        /// </summary>
        /// <value>The user's username</value>
        [Required]
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the user's password.
        /// </summary>
        /// <value>The user's password.</value>
        [Required]
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the application id of the native app registration.
        /// </summary>
        /// <value>The application id of the native app registration</value>
        [Required]
        public string NativeAppId { get; set; }

        /// <summary>
        /// Gets or sets the application id of the target/resource app registration.
        /// </summary>
        /// <value>The application id of the target/resource app registration.</value>
        [Required]
        public string ResourceAppId { get; set; }

        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        /// <value>The tenant identifier.</value>
        [Required]
        public string TenantId { get; set; }
    }
}
