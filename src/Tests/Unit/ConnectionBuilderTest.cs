namespace Cloud.Core.Authentication.AzureAuth.Tests.Unit
{
    using System;
    using Storage;
    using FluentAssertions;
    using Testing;
    using Xunit;
    using ServiceBus;

    public class ConnectionBuilderTest
    {
        [Theory, IsUnit]
        [InlineData("", "")]
        [InlineData(null, null)]
        [InlineData("something", "")]
        [InlineData("", "something")]
        [InlineData("something", "something")]
        public void Test_StorageConfig_Validate(string instanceName, string subscriptionId)
        {
            var config = new StorageConfig
            {
                InstanceName = instanceName,
                SubscriptionId = subscriptionId
            };

            if (!instanceName.IsNullOrEmpty() && !subscriptionId.IsNullOrEmpty())
                AssertExtensions.DoesNotThrow(() => config.Validate());
            else
                Assert.Throws<ArgumentException>(() => config.Validate());
        }

        [Theory, IsUnit]
        [InlineData("", "", "", true, null)]
        [InlineData(null, null, null, true, null)]
        [InlineData("something", "", "", true, null)]
        [InlineData("", "something", "", true, null)]
        [InlineData("", "", "something", true, null)]
        [InlineData("something", "something", "", true, null)]
        [InlineData("something", "something", "something", false, "something")]
        [InlineData("something", "something", "something", false,  null)]
        public void Test_ServiceBusConfig_Validate(string instanceName, string subscriptionId, string sharedPolicyName, bool isServiceLevelSharedAccessPolicy, string entityPath)
        {
            var config = new ServiceBusConfig
            {
                InstanceName = instanceName,
                EntityPath =entityPath,
                IsServiceLevelSharedAccessPolicy = isServiceLevelSharedAccessPolicy,
                IsTopic = isServiceLevelSharedAccessPolicy,
                SharedAccessPolicyName = sharedPolicyName,
                SubscriptionId = subscriptionId
            };

            config.IsTopic.Should().Be(isServiceLevelSharedAccessPolicy);

            if (instanceName.IsNullOrEmpty())
                Assert.Throws<ArgumentException>(() => config.Validate());

            if (subscriptionId.IsNullOrEmpty())
                Assert.Throws<ArgumentException>(() => config.Validate());

            if (sharedPolicyName.IsNullOrEmpty())
                Assert.Throws<ArgumentException>(() => config.Validate());

            if (!isServiceLevelSharedAccessPolicy && entityPath.IsNullOrEmpty())
                Assert.Throws<ArgumentException>(() => config.Validate());
            
            if (!instanceName.IsNullOrEmpty() && !subscriptionId.IsNullOrEmpty() && !sharedPolicyName.IsNullOrEmpty()
                & isServiceLevelSharedAccessPolicy == false && !entityPath.IsNullOrEmpty())
                AssertExtensions.DoesNotThrow(() => config.Validate());
        }
    }
}
