namespace Cloud.Core.Authentication.AzureAuth.Tests.Integration
{
    using System;
    using System.Security.Cryptography.X509Certificates;
    using Config;
    using Core.Storage.AzureBlobStorage;
    using Core.Storage.AzureBlobStorage.Config;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using Testing;
    using Xunit;

    [IsIntegration]
    public class AzureAuthenticationTest
    {
        [Fact]
        public void Test_Authenticates_MSI_Credentials()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
            var auth = new AzureTokenAuthentication(new MsiAuth
            {
                TenantId = config.GetValue<string>("TenantId")
            });
            auth.AccessToken.Should().NotBe(null);
        }

        [Fact]
        public void Test_Authenticates_SP_MixedCredentials()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
            var auth = new AzureTokenAuthentication(new ServicePrincipleAuth
            {
                AppSecret = config.GetValue<string>("AppSecret"),
                TenantId = config.GetValue<string>("TenantId"),
                AppId = config.GetValue<string>("AppId")
            });

            auth.AccessToken.Should().NotBe(null);
            auth.AccessToken.BearerToken.Should().NotBe(null);
            auth.AccessToken.BearerToken.Length.Should().BeGreaterThan(0);
            auth.AccessToken.HasExpired.Should().BeFalse();
            auth.AccessToken.Expires.Should().BeMoreThan(DateTime.Now.TimeOfDay);
        }

        [Fact]
        public void Test_Authenticates_UserCredentials()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
            var auth = new AzureTokenAuthentication(new UserAuth
            {
                Username = config.GetValue<string>("Username"),
                Password = config.GetValue<string>("Password"),
                NativeAppId = config.GetValue<string>("NativeAppId"),
                TenantId = config.GetValue<string>("TenantId"),
                ResourceAppId = config.GetValue<string>("ResourceAppId")
            });

            auth.AccessToken.Should().NotBe(null);
            auth.AccessToken.BearerToken.Should().NotBe(null);
            auth.AccessToken.BearerToken.Length.Should().BeGreaterThan(0);
            auth.AccessToken.HasExpired.Should().BeFalse();
            auth.AccessToken.Expires.Should().BeMoreThan(DateTime.Now.TimeOfDay);
        }

        [Fact]
        public void Test_Authenticates_CertificateAuthentication()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();

            //Load Certificate from blob storage
            var blobClient = new BlobStorage(new ConnectionConfig(config.GetValue<string>("StorageAccountConnectionString")));
            var certfile = blobClient.DownloadBlob("certs/AzureAuthIntTest.pfx").GetAwaiter().GetResult();

            //Azure AD Certificate Based Authentication
            var certificateBytes = new byte[certfile.Length];
            certfile.Read(certificateBytes, 0, (int)certfile.Length);
            var cert = new X509Certificate2(
                certificateBytes,
                config.GetValue<string>("CertificatePassword"),
                X509KeyStorageFlags.Exportable |
                X509KeyStorageFlags.MachineKeySet |
                X509KeyStorageFlags.PersistKeySet);

            var auth = new AzureTokenAuthentication(new CertAuth
            {
                AppId = config.GetValue<string>("CertificateAppId"),
                TenantName = config.GetValue<string>("TenantName"),
                TargetUri = "https://management.core.windows.net/",
                Certificate = cert
            });

            auth.AccessToken.Should().NotBe(null);
            auth.AccessToken.BearerToken.Should().NotBe(null);
            auth.AccessToken.BearerToken.Length.Should().BeGreaterThan(0);
            auth.AccessToken.HasExpired.Should().BeFalse();
            auth.AccessToken.Expires.Should().BeMoreThan(DateTime.Now.TimeOfDay);
        }

        [InlineData("incorrect")]
        [Theory, IsIntegration]
        public void Test_Authenticates_IncorrectUserCredentials_ThrowsException(string password)
        {
            var config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();

            Assert.Throws<InvalidOperationException>(() => new AzureTokenAuthentication(new UserAuth
            {
                Username = "incorrect",
                Password = password,
                NativeAppId = "incorrect",
                TenantId = "incorrect",
                ResourceAppId = "incorrect"
            }).AccessToken);
        }
    }
}
