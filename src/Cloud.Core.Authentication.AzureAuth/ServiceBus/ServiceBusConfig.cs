namespace Cloud.Core.Authentication.AzureAuth.ServiceBus
{
    using System;

    /// <summary>
    /// Service Bus specific configuration.
    /// </summary>
    public class ServiceBusConfig
    {
        /// <summary>
        /// Gets or sets the subscription identifier.
        /// </summary>
        /// <value>The subscription identifier.</value>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the name of the shared access policy.
        /// </summary>
        /// <value>The name of the shared access policy.</value>
        public string SharedAccessPolicyName { get; set; }

        /// <summary>
        /// Gets or sets the name of the instance.
        /// </summary>
        /// <value>The name of the instance.</value>
        public string InstanceName { get; set; }

        /// <summary>
        /// Gets or sets the entity path.
        /// </summary>
        /// <value>The entity path.</value>
        public string EntityPath { get; set; }

        /// <summary>
        /// Gets or sets a value indicating whether this instance is service level shared access policy.
        /// </summary>
        /// <value><c>true</c> if this instance is service level shared access policy; otherwise, <c>false</c>.</value>
        public bool IsServiceLevelSharedAccessPolicy { get; set; }

        /// <summary>
        /// States whether the connection instance will be for a topic or queue.  
        /// </summary>
        /// <value><c>true</c> if this instance is topic; otherwise, <c>false</c> for queue.</value>
        public bool IsTopic { get; set; }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// InstanceName must be set for Service Bus connection or
        /// SubscriptionId must be set for Service Bus connection or
        /// SharedAccessPolicyName must be set for Service Bus connection or
        /// When you are NOT using a Service Level Shared Access Policy [IsServiceLevelSharedAccessPolicy=false],
        /// then the EntityPath must be set for Service Bus connection
        /// </exception>
        public void Validate()
        {
            if (string.IsNullOrEmpty(InstanceName))
            {
                throw new ArgumentException("InstanceName must be set for Service Bus connection");
            }

            if (string.IsNullOrEmpty(SubscriptionId))
            {
                throw new ArgumentException("SubscriptionId must be set for Service Bus connection");
            }

            if (string.IsNullOrEmpty(SharedAccessPolicyName))
            {
                throw new ArgumentException("SharedAccessPolicyName must be set for Service Bus connection");
            }

            if (!IsServiceLevelSharedAccessPolicy && string.IsNullOrEmpty(EntityPath))
            {
                throw new ArgumentException("When you are NOT using a Service Level Shared Access Policy " +
                                            "[IsServiceLevelSharedAccessPolicy=false], then the EntityPath must " +
                                            "be set for Service Bus connection");
            }
        }
    }
}
