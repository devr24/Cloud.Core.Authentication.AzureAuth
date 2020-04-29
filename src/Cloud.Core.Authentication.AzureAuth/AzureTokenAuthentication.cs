using System.Diagnostics.CodeAnalysis;

namespace Cloud.Core.Authentication.AzureAuth
{
    using System;
    using System.Collections.Generic;
    using System.IdentityModel.Tokens.Jwt;
    using System.Net.Http;
    using System.Threading.Tasks;
    using Config;
    using Microsoft.Azure.Management.Fluent;
    using Microsoft.Azure.Management.ResourceManager.Fluent;
    using Microsoft.Azure.Management.ResourceManager.Fluent.Authentication;
    using Microsoft.Azure.Management.ResourceManager.Fluent.Core;
    using Microsoft.Azure.Services.AppAuthentication;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Microsoft.Rest;
    using Microsoft.Rest.TransientFaultHandling;
    using Newtonsoft.Json;

    /// <summary>
    /// Authentication class used to generate an Azure bearer token in an Msi or Service Principle context.
    /// Implements the <see cref="IAuthentication" />
    /// </summary>
    /// <seealso cref="IAuthentication" />
    public class AzureTokenAuthentication : IAuthentication
    {
        private readonly ServicePrincipleAuth _spAuth;
        private readonly MsiAuth _msiAuth;
        private readonly UserAuth _userAuth;
        private readonly CertAuth _certAuth;
        private readonly string _azureAppRegistrationAuthority;
        private IAccessToken _token;

        private const string AzureManagementAuthority = "https://management.azure.com/";
        private const string WindowsLoginAuthority = "https://login.windows.net/";

        /// <summary>
        /// Name of the object instance.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Get an AccessToken for authentication.
        /// </summary> <value>The access token.</value>
        public IAccessToken AccessToken
        {
            get
            {
                if (_token == null || _token.HasExpired)
                {
                    _token = GetBearerToken().GetAwaiter().GetResult();
                }

                return _token;
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureTokenAuthentication"/> class.
        /// </summary>
        /// <param name="servicePrincipleAuth">The service principle authentication.</param>
        /// <param name="azureAppRegistrationAuthority">Optionally specify an Azure App Registration to authenticate against. Defaults to "https://management.azure.com/"</param>
        public AzureTokenAuthentication(ServicePrincipleAuth servicePrincipleAuth, string azureAppRegistrationAuthority = null)
        {
            // Validate the certificate.
            servicePrincipleAuth.ThrowIfInvalid();

            _spAuth = servicePrincipleAuth;
            _azureAppRegistrationAuthority = azureAppRegistrationAuthority;
            Name = servicePrincipleAuth.AppId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureTokenAuthentication"/> class.
        /// </summary>
        /// <param name="msiAuth">The msi authentication.</param>
        /// <param name="azureAppRegistrationAuthority">Optionally specify an Azure App Registration to authenticate against. Defaults to "https://management.azure.com/"</param>
        public AzureTokenAuthentication(MsiAuth msiAuth, string azureAppRegistrationAuthority = null)
        {
            // Validate the certificate.
            msiAuth.ThrowIfInvalid();

            _msiAuth = msiAuth;
            _azureAppRegistrationAuthority = azureAppRegistrationAuthority;
            Name = msiAuth.TenantId;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureTokenAuthentication"/> class.
        /// </summary>
        /// <param name="userAuth">The user authentication.</param>
        public AzureTokenAuthentication(UserAuth userAuth)
        {
            // Validate the certificate.
            userAuth.ThrowIfInvalid();

            _userAuth = userAuth;
            Name = userAuth.Username;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureTokenAuthentication"/> class.
        /// </summary>
        /// <param name="certAuth">The certificate authentication</param>
        public AzureTokenAuthentication(CertAuth certAuth)
        {
            // Validate the certificate.
            certAuth.ThrowIfInvalid();

            _certAuth = certAuth;
            Name = certAuth.TenantName;
        }

        /// <summary>
        /// Gets the IAzure authenticated object for connecting to Azure namespaces.
        /// </summary>
        /// <param name="subscriptionId">The subscription identifier for Azure.</param>
        /// <returns>IAzure.</returns>
        internal IAzure GetAzureAuthenticated(string subscriptionId)
        {
            var accessToken = AccessToken;

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
        /// or
        /// Could not authenticate using User Authentication
        /// </exception>
        [ExcludeFromCodeCoverage]
        private async Task<AzureToken> GetBearerToken()
        {
            var authenticated = new AzureToken();

            var authority = AzureManagementAuthority;
            if (!string.IsNullOrEmpty(_azureAppRegistrationAuthority))
            {
                authority = _azureAppRegistrationAuthority;
            }

            if (_spAuth != null)
            {
                // Service Principle Authentication.
                var clientCredential = new ClientCredential(_spAuth.AppId, _spAuth.AppSecret);
                var context = new AuthenticationContext($"{WindowsLoginAuthority}{_spAuth.TenantId}", false);

                var tokenResult = await context.AcquireTokenAsync(authority, clientCredential).ConfigureAwait(false);

                if (tokenResult == null)
                {
                    throw new InvalidOperationException("Could not authenticate using Service Principle authentication");
                }

                // Set the return Azure Token information.
                authenticated.Expires = tokenResult.ExpiresOn;
                authenticated.BearerToken = tokenResult.AccessToken;
            }
            else if (_msiAuth != null)
            {
                // Msi Authentication.
                var provider = new AzureServiceTokenProvider();
                var token = await provider.GetAccessTokenAsync(authority, _msiAuth.TenantId)
                    .ConfigureAwait(false);

                if (string.IsNullOrEmpty(token))
                {
                    throw new InvalidOperationException("Could not authenticate using Managed Service Identity, ensure the application is running in a secure context");
                }

                var tokenDecoder = new JwtSecurityTokenHandler();

                if (!(tokenDecoder.ReadToken(token) is JwtSecurityToken jwtSecurityToken))
                {
                    throw new InvalidOperationException("Could not authenticate using Managed Service Identity, ensure the application is running in a secure context");
                }

                // Set the return Azure Token information.
                authenticated.BearerToken = jwtSecurityToken.RawData;
                authenticated.Expires = jwtSecurityToken.ValidTo;
            }
            else if (_certAuth != null)
            {
                //Certificate Authentication
                var authenticationAuthority = $"{WindowsLoginAuthority}{_certAuth.TenantName}";
                var authContext = new AuthenticationContext(authenticationAuthority);
                var clientAssertionCertificate = new ClientAssertionCertificate(_certAuth.AppId, _certAuth.Certificate);
                var azureADAppOnlyAuthenticationToken = authContext.AcquireTokenAsync(AzureManagementAuthority, clientAssertionCertificate).GetAwaiter().GetResult();

                if (azureADAppOnlyAuthenticationToken == null)
                {
                    throw new InvalidOperationException("Could not authenticate using Certificate authentication");
                }

                // Set the return Azure Token information.
                authenticated.BearerToken = azureADAppOnlyAuthenticationToken.AccessToken;
                authenticated.Expires = azureADAppOnlyAuthenticationToken.ExpiresOn;
            }
            else
            {
                var jwtSecurityToken = await GetUserAuthToken().ConfigureAwait(false);

                if (jwtSecurityToken == null)
                {
                    throw new InvalidOperationException("Could not authenticate using Certificate authentication");
                }

                // Set the return Azure Token information.
                authenticated.BearerToken = jwtSecurityToken.RawData;
                authenticated.Expires = jwtSecurityToken.ValidTo;
            }

            return authenticated;
        }

        /// <summary>
        /// Generates an authentication bearer token using user authentication
        /// </summary>
        /// <returns>Task{JwtSecurityToken}.</returns>
        /// <exception cref="InvalidOperationException">
        /// Could not authenticate using Service Principle authentication
        /// </exception>
        [ExcludeFromCodeCoverage]
        private async Task<JwtSecurityToken> GetUserAuthToken()
        {
            using var httpClient = new HttpClient();

            // user credentials authentication
            var content = new FormUrlEncodedContent(new Dictionary<string, string>
            {
                {"grant_type", "password"},
                {"client_id", _userAuth.NativeAppId },
                {"password", _userAuth.Password },
                {"username", _userAuth.Username },
                {"resource", _userAuth.ResourceAppId }
            });

            var req = new HttpRequestMessage(HttpMethod.Post, $"https://login.microsoftonline.com/{_userAuth.TenantId}/oauth2/token") { Content = content };

            var resp = await httpClient.SendAsync(req).ConfigureAwait(false);

            var respBody = await resp.Content.ReadAsStringAsync().ConfigureAwait(false);

            var token = JsonConvert.DeserializeObject<AzureToken>(respBody);

            if (string.IsNullOrEmpty(token.BearerToken))
            {
                throw new InvalidOperationException("Could not authenticate using provided user credentials, ensure the user credentials provided are correct");
            }

            var tokenDecoder = new JwtSecurityTokenHandler();
            var jwtSecurityToken = tokenDecoder.ReadToken(token.BearerToken) as JwtSecurityToken;

            return jwtSecurityToken;
        }
    }
}
