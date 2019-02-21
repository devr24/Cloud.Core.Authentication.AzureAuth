namespace Cloud.Core.Authentication.AzureAuth
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Microsoft.Azure.Services.AppAuthentication;
    using Microsoft.Azure.Management.Fluent;
    using Microsoft.Azure.Management.ResourceManager.Fluent;
    using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
    using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
    using Microsoft.Rest;
    using Microsoft.Rest.TransientFaultHandling;
    using Config;

    /// <summary>
    /// Authentication class used to generate an Azure bearer token in an Msi or Service Principle context.
    /// Implements the <see cref="IAuthentication" />
    /// </summary>
    /// <seealso cref="Cloud.Core.IAuthentication" />
    public class AzureTokenAuthentication  : IAuthentication
    {
        private readonly ServicePrincipleAuth _spAuth;
        private readonly MsiAuth _msiAuth;
        private IAccessToken _token;

        private const string AzureManagementAuthority = "https://management.azure.com/";
        private const string WindowsLoginAuthority = "https://login.windows.net/";

        /// <summary>
        /// Get an AccessToken for authentication.
        /// </summary> <value>The access token.</value>
        public IAccessToken AccessToken
        {
            get {
                if (_token == null || _token.HasExpired)
                    _token = GetBearerToken().GetAwaiter().GetResult();

                return _token;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureTokenAuthentication"/> class.
        /// </summary>
        /// <param name="servicePrincipleAuth">The service principle authentication.</param>
        public AzureTokenAuthentication(ServicePrincipleAuth servicePrincipleAuth)
        {
            _spAuth = servicePrincipleAuth;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureTokenAuthentication"/> class.
        /// </summary>
        /// <param name="msiAuth">The msi authentication.</param>
        public AzureTokenAuthentication(MsiAuth msiAuth)
        {
            _msiAuth = msiAuth;
        }

        /// <summary>
        /// Gets the IAzure authenticated object for connecting to Azure namespaces.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier for Azure.</param>
        /// <returns>IAzure.</returns>
        internal IAzure GetAzureAuthenticated(string subscriptionId)
        {
            var accessToken =  AccessToken;

            // Set credentials and grab the authenticated REST client.
            var tokenCredentials = new TokenCredentials(accessToken.BearerToken);

            var client = RestClient.Configure()
                .WithEnvironment(AzureEnvironment.AzureGlobalCloud)
                .WithLogLevel(HttpLoggingDelegatingHandler.Level.None)
                .WithCredentials(new AzureCredentials(tokenCredentials, tokenCredentials, string.Empty, AzureEnvironment.AzureGlobalCloud))
                .WithRetryPolicy(new RetryPolicy(new HttpStatusCodeErrorDetectionStrategy(), new FixedIntervalRetryStrategy(3, TimeSpan.FromMilliseconds(500))))
                .Build();

            // Authenticate against the management layer.
            return Azure.Authenticate(client, string.Empty).WithSubscription(subscriptionId);
        }

        /// <summary>
        /// Generates an authentication bearer token.
        /// </summary>
        /// <returns>Task{AzureToken}.</returns>
        /// <exception cref="InvalidOperationException">
        /// Could not authenticate using Managed Service Identity, ensure the application is running in a secure context
        /// or
        /// Could not authenticate to using Service Principle authentication
        /// </exception>
        private async Task<AzureToken> GetBearerToken()
        {
            var authenticated = new AzureToken();

            if (_spAuth != null)
            {
                // Ensure setup correctly.
                _spAuth.Validate();

                // Service Principle Authentication.
                var clientCredential = new ClientCredential(_spAuth.AppId, _spAuth.AppSecret);
                AuthenticationContext context = new AuthenticationContext($"{WindowsLoginAuthority}{_spAuth.TenantId}", false);

                AuthenticationResult tokenResult = await context.AcquireTokenAsync(AzureManagementAuthority, clientCredential).ConfigureAwait(false);

                if (tokenResult == null)
                    throw new InvalidOperationException("Could not authenticate to using Service Principle authentication");

                // Set the return Azure Token information.
                authenticated.Expires = tokenResult.ExpiresOn;
                authenticated.BearerToken = tokenResult.AccessToken;
            }
            else
            { 
                // Ensure setup correctly.
                _msiAuth.Validate();

                // Msi Authentication.
                var provider = new AzureServiceTokenProvider();
                var token = await provider.GetAccessTokenAsync(AzureManagementAuthority, _msiAuth.TenantId)
                    .ConfigureAwait(false);

                if (string.IsNullOrEmpty(token))
                    throw new InvalidOperationException("Could not authenticate using Managed Service Identity, ensure the application is running in a secure context");

                // Set the return Azure Token information.
                authenticated.BearerToken = token;
                authenticated.Expires = DateTime.Now.AddDays(1);
            }
            
            return authenticated;
        }
        
    }
}
