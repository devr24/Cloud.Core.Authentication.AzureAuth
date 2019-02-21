namespace Cloud.Core.Authentication.AzureAuth.Tests.Integration
{
    using System;
    using Storage;
    using System.Threading.Tasks;
    using FluentAssertions;
    using Config;
    using Testing;
    using Microsoft.Extensions.Configuration;
    using Xunit;
    using ServiceBus;

    public class ConnectionBuilderTest
    {
        [Fact, IsIntegration]
        public async Task Test_ServiceBusConnectionBuilder_SP_BuildConnection()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
            var connectionBuilder = new ServiceBusConnectionBuilder(
                new ServicePrincipleAuth {
                    AppSecret = config.GetValue<string>("AppSecret"),
                    TenantId = config.GetValue<string>("TenantId"),
                    AppId = config.GetValue<string>("AppId")
                }, new ServiceBusConfig
                {
                    InstanceName = config.GetValue<string>("ServiceBusInstanceName"),
                    EntityPath = string.Empty,
                    IsServiceLevelSharedAccessPolicy = true,
                    IsTopic = true,
                    SharedAccessPolicyName = config.GetValue<string>("ServiceBusRootAccessPolicy"),
                    SubscriptionId = config.GetValue<string>("SubscriptionId")
                });

            var conn = await connectionBuilder.BuildConnectionString();
            conn.Should().NotBe(null);
        }

        [Fact, IsIntegration]
        public async Task Test_ServiceBusConnectionBuilder_SP_BuildConnectionQueue()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
            var connectionBuilder = new ServiceBusConnectionBuilder(
                new ServicePrincipleAuth
                {
                    AppSecret = config.GetValue<string>("AppSecret"),
                    TenantId = config.GetValue<string>("TenantId"),
                    AppId = config.GetValue<string>("AppId")
                }, new ServiceBusConfig
                {
                    InstanceName = config.GetValue<string>("ServiceBusInstanceName"),
                    EntityPath = config.GetValue<string>("ServiceBusEntityPath"),
                    IsServiceLevelSharedAccessPolicy = false,
                    IsTopic = true,
                    SharedAccessPolicyName = config.GetValue<string>("ServiceBusEntityAccessPolicy"),
                    SubscriptionId = config.GetValue<string>("SubscriptionId")
                });

            var conn = await connectionBuilder.BuildConnectionString();
            conn.Should().NotBe(null);
        }

        [Fact, IsIntegration]
        public void Test_ServiceBusConnectionBuilder_SP_BuildConnectionWrong()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
            var connectionBuilder = new ServiceBusConnectionBuilder(
                new ServicePrincipleAuth
                {
                    AppSecret = config.GetValue<string>("AppSecret"),
                    TenantId = config.GetValue<string>("TenantId"),
                    AppId = config.GetValue<string>("AppId")
                }, new ServiceBusConfig
                {
                    InstanceName = "wrong",
                    EntityPath = string.Empty,
                    IsServiceLevelSharedAccessPolicy = true,
                    IsTopic = true,
                    SharedAccessPolicyName = config.GetValue<string>("ServiceBusRootAccessPolicy"),
                    SubscriptionId = config.GetValue<string>("SubscriptionId")
                });

            Assert.Throws<InvalidOperationException>(() => connectionBuilder.BuildConnectionString().GetAwaiter().GetResult());
        }

        [Fact, IsIntegration]
        public async Task Test_ServiceBusConnectionBuilder_MSI_BuildConnection()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
            var connectionBuilder = new ServiceBusConnectionBuilder(
                new MsiAuth
                {
                    TenantId = config.GetValue<string>("TenantId")
                }, new ServiceBusConfig
                {
                    InstanceName = config.GetValue<string>("ServiceBusInstanceName"),
                    EntityPath = string.Empty,
                    IsServiceLevelSharedAccessPolicy = true,
                    IsTopic = true,
                    SharedAccessPolicyName = config.GetValue<string>("ServiceBusRootAccessPolicy"),
                    SubscriptionId = config.GetValue<string>("SubscriptionId")
                });

            var conn = await connectionBuilder.BuildConnectionString();
            conn.Should().NotBe(null);
        }

        [Fact, IsIntegration]
        public async Task Test_StorageConnectionBuilder_SP_BuildConnection()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
            var connectionBuilder = new StorageConnectionBuilder(
                new ServicePrincipleAuth
                {
                    AppSecret = config.GetValue<string>("AppSecret"),
                    TenantId = config.GetValue<string>("TenantId"),
                    AppId = config.GetValue<string>("AppId")
                }, new StorageConfig
                {
                    InstanceName = config.GetValue<string>("StorageInstanceName"),
                    SubscriptionId = config.GetValue<string>("SubscriptionId")
                });

            var conn = await connectionBuilder.BuildConnectionString();
            conn.Should().NotBe(null);
        }

        [Fact, IsIntegration]
        public async Task Test_StorageConnectionBuilder_MSI_BuildConnection()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
            var connectionBuilder = new StorageConnectionBuilder(
                new MsiAuth
                {
                    TenantId = config.GetValue<string>("TenantId")
                }, new StorageConfig
                {
                    InstanceName = config.GetValue<string>("StorageInstanceName"),
                    SubscriptionId = config.GetValue<string>("SubscriptionId")
                });

            var conn = await connectionBuilder.BuildConnectionString();
            conn.Should().NotBe(null);
        }

        [Fact, IsIntegration]
        public void Test_StorageConnectionBuilder_MSI_BuildConnectionWrong()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
            var connectionBuilder = new StorageConnectionBuilder(
                new ServicePrincipleAuth
                {
                    AppSecret = config.GetValue<string>("AppSecret"),
                    TenantId = config.GetValue<string>("TenantId"),
                    AppId = config.GetValue<string>("AppId")
                }, new StorageConfig
                {
                    InstanceName = "wrong",
                    SubscriptionId = config.GetValue<string>("SubscriptionId")
                });

            Assert.Throws<InvalidOperationException>(() => connectionBuilder.BuildConnectionString().GetAwaiter().GetResult());
        }
    }
}
