namespace Cloud.Core.Authentication.AzureAuth.Storage
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Config;

    /// <summary>
    /// Class StorageConnectionBuilder.
    /// Implements the <see cref="IConnectionStringBuilder" />
    /// </summary>
    /// <seealso cref="IConnectionStringBuilder" />
    public class StorageConnectionBuilder : IConnectionStringBuilder
    {
        private readonly AzureTokenAuthentication _azureAuth;
        private readonly StorageConfig _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageConnectionBuilder"/> class.
        /// </summary>
        /// <param name="msiAuth">The msi authentication.</param>
        /// <param name="config">The configuration.</param>
        public StorageConnectionBuilder(MsiAuth msiAuth, StorageConfig config)
        {
            _azureAuth = new AzureTokenAuthentication(msiAuth);
            _config = config;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="StorageConnectionBuilder"/> class.
        /// </summary>
        /// <param name="spAuth">The sp authentication.</param>
        /// <param name="config">The configuration.</param>
        public StorageConnectionBuilder(ServicePrincipleAuth spAuth, StorageConfig config)
        {
            _azureAuth = new AzureTokenAuthentication(spAuth);
            _config = config;
        }

        /// <summary>
        /// Builds the connection string.
        /// </summary>
        /// <returns>Task&lt;System.String&gt;.</returns>
        /// <exception cref="InvalidOperationException">
        /// Could not find the storage instance
        /// or
        /// Could not find access keys for the storage instance
        /// </exception>
        public async Task<string> BuildConnectionString()
        {
            var azureManagement = _azureAuth.GetAzureAuthenticated(_config.SubscriptionId);

            // Get the storage namespace for the passed in instance name.
            var storageNamespace = azureManagement.StorageAccounts.List().FirstOrDefault(n => n.Name == _config.InstanceName);

            // If we cant find that name, throw an exception.
            if (storageNamespace == null)
            {
                throw new InvalidOperationException($"Could not find the storage instance {_config.InstanceName} in the subscription Id specified");
            }

            // Storage accounts use access keys - this will be used to build a connection string.
            var accessKeys = await storageNamespace.GetKeysAsync();

            // If the access keys are not found (not configured for some reason), throw an exception.
            if (accessKeys == null)
            {
                throw new InvalidOperationException($"Could not find access keys for the storage instance {_config.InstanceName}");
            }

            // We just default to the first key.
            var key = accessKeys[0].Value;

            // Build and return the connection string.
            return $"DefaultEndpointsProtocol=https;AccountName={_config.InstanceName};AccountKey={key};EndpointSuffix=core.windows.net";
        }
    }
}
