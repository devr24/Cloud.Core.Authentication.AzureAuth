namespace Cloud.Core.Authentication.AzureAuth.ServiceBus
{
    using System;
    using System.Linq;
    using System.Threading.Tasks;
    using Config;

    /// <summary>
    /// Class ServiceBusConnectionBuilder.
    /// Implements the <see cref="Cloud.Core.Authentication.AzureAuth.IConnectionStringBuilder" />
    /// </summary>
    /// <seealso cref="Cloud.Core.Authentication.AzureAuth.IConnectionStringBuilder" />
    public class ServiceBusConnectionBuilder : IConnectionStringBuilder
    {
        private readonly AzureTokenAuthentication _azureAuth;
        private readonly ServiceBusConfig _config;

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBusConnectionBuilder"/> class.
        /// </summary>
        /// <param name="msiAuth">The msi authentication.</param>
        /// <param name="config">The configuration.</param>
        public ServiceBusConnectionBuilder(MsiAuth msiAuth, ServiceBusConfig config)
        {
            _config = config;
            _azureAuth = new AzureTokenAuthentication(msiAuth);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ServiceBusConnectionBuilder" /> class.
        /// </summary>
        /// <param name="spAuth">The sp authentication.</param>
        /// <param name="config">The configuration.</param>
        public ServiceBusConnectionBuilder(ServicePrincipleAuth spAuth, ServiceBusConfig config)
        {
            _config = config;
            _azureAuth = new AzureTokenAuthentication(spAuth);
        }

        /// <summary>
        /// Builds a connection string to the service using the authentication mechanism passed.
        /// Generates an access token automatically.
        /// </summary>
        /// <returns>Task{System.String} connection string.</returns>
        /// <exception cref="InvalidOperationException">Could not find the a service bus instance  {InstanceName} in the subscription with ID {SubscriptionId}</exception>
        public async Task<string> BuildConnectionString()
        {
            // Ensure setup correct.
            _config.Validate();

            var azureManagement = _azureAuth.GetAzureAuthenticated(_config.SubscriptionId);
            var sbNamespace = (await azureManagement.ServiceBusNamespaces.ListAsync()).FirstOrDefault(s => s.Name == _config.InstanceName);

            // If the namespace is not found, throw an exception.
            if (sbNamespace == null)
                throw new InvalidOperationException($"Could not find the a service bus instance  {_config.InstanceName} in the subscription with ID {_config.SubscriptionId}");

            string connString;
            
            // Connection string built up in different ways depending on the shared access policy level. Uses "global" (top service level) when isServiceLevelSAP [true].
            if (_config.IsServiceLevelSharedAccessPolicy)
                connString = sbNamespace.AuthorizationRules.GetByName(_config.SharedAccessPolicyName).GetKeys().PrimaryConnectionString;
            else
            {
                // Topic or queue level connection string.
                connString = _config.IsTopic
                    ? sbNamespace.Topics.GetByName(_config.EntityPath).AuthorizationRules.GetByName(_config.SharedAccessPolicyName).GetKeys().PrimaryConnectionString
                    : sbNamespace.Queues.GetByName(_config.EntityPath).AuthorizationRules.GetByName(_config.SharedAccessPolicyName).GetKeys().PrimaryConnectionString;
            }

            // Remove the addition "EntityPath" text.
            return connString.Replace($";EntityPath={_config.EntityPath}", string.Empty);
        }
    }
}
