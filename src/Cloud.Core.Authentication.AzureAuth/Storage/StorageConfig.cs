namespace Cloud.Core.Authentication.AzureAuth.Storage
{
    using System;

    /// <summary>
    /// Storage specific configuration.
    /// </summary>
    public class StorageConfig
    {
        /// <summary>
        /// Gets or sets the subscription identifier.
        /// </summary>
        /// <value>The subscription identifier.</value>
        public string SubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets the name of the instance.
        /// </summary>
        /// <value>The name of the instance.</value>
        public string InstanceName { get; set; }

        /// <summary>
        /// Validates this instance.
        /// </summary>
        /// <exception cref="ArgumentException">
        /// SubscriptionId must be set for Storage connection
        /// or InstanceName must be set for Storage connection
        /// </exception>
        public void Validate()
        {
            if (string.IsNullOrEmpty(SubscriptionId))
                throw new ArgumentException("SubscriptionId must be set for Storage connection");
            if (string.IsNullOrEmpty(InstanceName))
                throw new ArgumentException("InstanceName must be set for Storage connection");
        }
    }
}