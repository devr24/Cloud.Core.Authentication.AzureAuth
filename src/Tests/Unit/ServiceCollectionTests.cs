namespace Cloud.Core.Authentication.AzureAuth.Tests.Unit
{
    using System.Linq;
    using Cloud.Core.Testing;
    using FluentAssertions;
    using Microsoft.Extensions.DependencyInjection;
    using Xunit;

    [IsUnit]
    public class ServiceCollectionTests
    {
        [Fact]
        public void Test_ServiceCollectionExt_MsiConfig()
        {
            var services = new ServiceCollection();

            // Add singleton with msi auth.
            services.Count.Should().Be(0);
            services.AddAzureMsiAuthSingleton("tenantId");
            services.Count.Should().Be(2);
            services.Any(x => x.ServiceType == typeof(IAuthentication)).Should().BeTrue();
            services.Any(x => x.ServiceType == typeof(NamedInstanceFactory<IAuthentication>)).Should().BeTrue();
            services.Clear();

            services.Count.Should().Be(0);
            services.AddAzureAuthSingleton(new Config.MsiAuth { TenantId = "tenantId" });
            services.Count.Should().Be(2);
            services.Any(x => x.ServiceType == typeof(IAuthentication)).Should().BeTrue();
            services.Any(x => x.ServiceType == typeof(NamedInstanceFactory<IAuthentication>)).Should().BeTrue();
        }

        [Fact]
        public void Test_ServiceCollectionExt_ServicePrincipleConfig()
        {
            var services = new ServiceCollection();

            // Add singleton with msi auth.
            services.Count.Should().Be(0);
            services.AddAzureServicePrincipleAuthSingleton("appId", "appSecret", "tenantId");
            services.Count.Should().Be(2);
            services.Any(x => x.ServiceType == typeof(IAuthentication)).Should().BeTrue();
            services.Any(x => x.ServiceType == typeof(NamedInstanceFactory<IAuthentication>)).Should().BeTrue();
            services.Clear();

            services.Count.Should().Be(0);
            services.AddAzureAuthSingleton(new Config.ServicePrincipleAuth { 
                AppId = "appId", 
                AppSecret = "appSecret", 
                TenantId = "tenantId" 
            });
            services.Count.Should().Be(2);
            services.Any(x => x.ServiceType == typeof(IAuthentication)).Should().BeTrue();
            services.Any(x => x.ServiceType == typeof(NamedInstanceFactory<IAuthentication>)).Should().BeTrue();
        }

        [Fact]
        public void Test_ServiceCollectionExt_UserConfig()
        {
            var services = new ServiceCollection();

            // Add singleton with msi auth.
            services.Count.Should().Be(0);
            services.AddAzureUserAuthSingleton("appId", "username", "password", "resourceAppId", "tenantId");
            services.Count.Should().Be(2);
            services.Any(x => x.ServiceType == typeof(IAuthentication)).Should().BeTrue();
            services.Any(x => x.ServiceType == typeof(NamedInstanceFactory<IAuthentication>)).Should().BeTrue();
            services.Clear();

            services.Count.Should().Be(0);
            services.AddAzureAuthSingleton(new Config.UserAuth
            {
                NativeAppId = "appId",
                Username = "username",
                Password = "password",
                ResourceAppId = "resourceAppId",
                TenantId = "tenantId"
            });
            services.Count.Should().Be(2);
            services.Any(x => x.ServiceType == typeof(IAuthentication)).Should().BeTrue();
            services.Any(x => x.ServiceType == typeof(NamedInstanceFactory<IAuthentication>)).Should().BeTrue();
        }

        [Fact]
        public void Test_ServiceCollectionExt_CertConfig()
        {
            var services = new ServiceCollection();

            // Add singleton with msi auth.
            services.Count.Should().Be(0);
            services.AddAzureAuthSingleton(new Config.CertAuth
            {
                AppId = "appId",
                TenantName = "tenantName",
                TargetUri = "uri",
                Certificate = new System.Security.Cryptography.X509Certificates.X509Certificate2()
            });
            services.Count.Should().Be(2);
            services.Any(x => x.ServiceType == typeof(IAuthentication)).Should().BeTrue();
            services.Any(x => x.ServiceType == typeof(NamedInstanceFactory<IAuthentication>)).Should().BeTrue();
        }
    }
}
