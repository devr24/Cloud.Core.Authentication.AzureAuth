namespace Cloud.Core.Authentication.AzureAuth
{
    using System;

    /// <summary>
    /// Class AzureAuthenticated.
    /// Implements the <see cref="IAccessToken" />
    /// </summary>
    /// <seealso cref="IAccessToken" />
    public class AzureToken : IAccessToken
    {
        /// <summary>
        /// Bearer token for authentication.
        /// </summary>
        /// <value>The bearer token.</value>
        public string BearerToken { get; set; }

        /// <summary>
        /// Gets the bearer token expiry.
        /// </summary>
        /// <value>The expires.</value>
        public DateTimeOffset Expires { get; set; }

        /// <summary>
        /// Gets a value indicating whether this instance has expired.
        /// </summary>
        /// <value><c>true</c> if this instance has expired; otherwise, <c>false</c>.</value>
        public bool HasExpired => BearerToken == null || Expires < DateTime.UtcNow;
    }
}
