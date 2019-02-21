namespace Cloud.Core.Authentication.AzureAuth.Tests.Integration
{
    using System;
    using Config;
    using Testing;
    using FluentAssertions;
    using Microsoft.Extensions.Configuration;
    using Xunit;

    public class AzureAuthenticationTest
    {
        [Fact, IsIntegration]
        public void Test_Authenticates_MSI_Credentials()
        {
            var config = new ConfigurationBuilder().AddJsonFile("appSettings.json").Build();
            var auth = new AzureTokenAuthentication(new MsiAuth
            {
                TenantId = config.GetValue<string>("TenantId")
            });
            auth.AccessToken.Should().NotBe(null);
        }

        [Fact, IsIntegration]
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
    }
}
