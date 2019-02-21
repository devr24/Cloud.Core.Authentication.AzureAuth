namespace Cloud.Core.Authentication.AzureAuth.Tests.Unit
{
    using System;
    using Config;
    using Testing;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using Xunit;

    public class AzureAuthenticationTest
    {
        [Theory, IsUnit]
        [InlineData("")]
        [InlineData(null)]
        public void Test_Authenticates_MSI_EmptyCredentials(string tenantId)
        {
            Assert.Throws<ArgumentException>(() => new AzureTokenAuthentication(new MsiAuth
            {
                TenantId = tenantId
            }).AccessToken);
        }

        [Theory, IsUnit]
        [InlineData("", "", "")]
        [InlineData(null, null, null)]
        [InlineData("test", "", "")]
        [InlineData("test", "test", "")]
        public void Test_Authenticates_SP_EmptyCredentials(string appId, string appSecret, string tenantId)
        {
            Assert.Throws<ArgumentException>(() => new AzureTokenAuthentication(new ServicePrincipleAuth
            {
                AppSecret = appSecret,
                AppId = appId,
                TenantId = tenantId,
            }).AccessToken);
        }

        [Fact, IsUnit]
        public void Test_Authenticates_SP_MixedCredentials()
        {
            var auth = new AzureTokenAuthentication(new ServicePrincipleAuth
            {
                AppSecret = "test1",
                TenantId = "test1",
                AppId = "test1"
            });
            Assert.Throws<AdalServiceException>(() => auth.AccessToken);
        }
    }
}
