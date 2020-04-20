namespace Cloud.Core.Authentication.AzureAuth.Config
{
    using System;

    /// <summary>
    /// User Authentication model.
    /// </summary>
    public class UserAuth
    {
        /// <summary>
        /// Gets or sets the user's username
        /// </summary>
        /// <value>The user's username</value>
        public string Username { get; set; }

        /// <summary>
        /// Gets or sets the user's password.
        /// </summary>
        /// <value>Ther user's password.</value>
        public string Password { get; set; }

        /// <summary>
        /// Gets or sets the application id of the native app registration.
        /// </summary>
        /// <value>The application id of the native app registration</value>
        public string NativeAppId { get; set; }

        /// <summary>
        /// Gets or sets the application id of the target/resource app registration.
        /// </summary>
        /// <value>The application id of the target/resource app registration.</value>
        public string ResourceAppId { get; set; }

        /// <summary>
        /// Gets or sets the tenant identifier.
        /// </summary>
        /// <value>The tenant identifier.</value>
        public string TenantId { get; set; }

        /// <summary>
        /// Validates this instance has all mandatory fields.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// NativeAppId must be set for user authentication
        /// or
        /// Username must be set for user authentication
        /// or
        /// Password must be set for user authentication
        /// or
        /// ResourceAppId must be set for user authentication
        /// or
        /// TenantId must be set for user authentication
        /// </exception>
        public void Validate()
        {
            if (string.IsNullOrEmpty(NativeAppId))
                throw new ArgumentException("NativeAppId must be set for User Credential authentication");
            if (string.IsNullOrEmpty(Username))
                throw new ArgumentException("Username must be set for User Credential authentication");
            if (string.IsNullOrEmpty(Password))
                throw new ArgumentException("Password must be set for User Credential authentication");
            if (string.IsNullOrEmpty(ResourceAppId))
                throw new ArgumentException("ResourceAppId must be set for User Credential authentication");
            if (string.IsNullOrEmpty(TenantId))
                throw new ArgumentException("TenantId must be set for User Credential authentication");
        }
    }
}
