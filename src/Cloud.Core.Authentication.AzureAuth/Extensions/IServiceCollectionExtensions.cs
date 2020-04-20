namespace Microsoft.Extensions.DependencyInjection
{
    using System.Linq;
    using Cloud.Core;
    using Cloud.Core.Authentication.AzureAuth;
    using Cloud.Core.Authentication.AzureAuth.Config;

    /// <summary>
    /// Class IServiceCollectionExtensions.
    /// </summary>
    public static class IServiceCollectionExtensions
    {
        /// <summary>
        /// Add an instance of Azure authentication as a singleton, using managed user config to setup.  Requires the TenantId to authenticate with and an optional app authority url.
        /// </summary>
        /// <param name="services">The services to extend.</param>
        /// <param name="tenantId">Tenant Id the instance lives in.</param>
        /// <param name="appRegistrationUrl">Optional parameter for an app registration url to authenticate against.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddAzureMsiAuthSingleton(this IServiceCollection services, string tenantId, string appRegistrationUrl = null)
        {
            services.AddAzureAuthSingleton(new MsiAuth { TenantId = tenantId }, appRegistrationUrl);
            return services;
        }

        /// <summary>
        /// Add an instance of IAuthentication for service principle authentication.
        /// </summary>
        /// <param name="services">The services to extend.</param>
        /// <param name="appId">Application Id to use for authentication.</param>
        /// <param name="appSecret">Application secret for the AppId.</param>
        /// <param name="tenantId">Tentant to authenticate with.</param>
        /// <param name="appRegistrationUrl">Optional parameter for an app registration url to authenticate against.</param>
        /// <returns>Service collection with the IAuthentication added.</returns>
        public static IServiceCollection AddAzureServicePrincipleAuthSingleton(this IServiceCollection services, string appId, string appSecret, string tenantId, string appRegistrationUrl = null)
        {
            return services.AddAzureAuthSingleton(new ServicePrincipleAuth { 
                AppId = appId,
                AppSecret = appSecret,
                TenantId = tenantId
            }, appRegistrationUrl);
        }

        /// <summary>
        /// Add an instance of IAuthentication for user authentication.
        /// </summary>
        /// <param name="services">The services to extend.</param>
        /// <param name="tenantId">Tentant to authenticate with.</param>
        /// <param name="username">Username for auth.</param>
        /// <param name="password">Password for auth.</param>
        /// <param name="nativeAppId">Native application Id.</param>
        /// <param name="resourceAppId">Resource app Id.</param>
        /// <returns>Service collection with the IAuthentication added.</returns>
        public static IServiceCollection AddAzureUserAuthSingleton(this IServiceCollection services, string nativeAppId, string username, string password, string resourceAppId, string tenantId)
        {
            return services.AddAzureAuthSingleton(new UserAuth { 
                NativeAppId = nativeAppId, 
                Username = username,
                Password = password,
                ResourceAppId = resourceAppId, 
                TenantId = tenantId });
        }

        /// <summary>
        /// Add an instance of IAuthentication for managed user identity authentication.
        /// </summary>
        /// <param name="services">The services to extend.</param>
        /// <param name="config">The managed identity authentication configuration to initialise with.</param>
        /// <param name="appRegistrationUrl">Optional parameter for an app registration url to authenticate against.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddAzureAuthSingleton(this IServiceCollection services, MsiAuth config, string appRegistrationUrl = null)
        {
            services.AddSingleton<IAuthentication>(new AzureTokenAuthentication(config, appRegistrationUrl));
            AddFactoryIfNotAdded(services);
            return services;
        }

        /// <summary>
        /// Add an instance of IAuthentication for certificate authentication.
        /// </summary>
        /// <param name="services">The services to extend.</param>
        /// <param name="certAuth">Certificate authentication.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddAzureAuthSingleton(this IServiceCollection services, CertAuth certAuth)
        {
            services.AddSingleton<IAuthentication>(new AzureTokenAuthentication(certAuth));
            AddFactoryIfNotAdded(services);
            return services;
        }

        /// <summary>
        /// Add an instance of IAuthentication for service principle authentication.
        /// </summary>
        /// <param name="services">The services to extend.</param>
        /// <param name="config">The service prinicple authentication configuration to initialise with.</param>
        /// <param name="appRegistrationUrl">Optional parameter for an app registration url to authenticate against.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddAzureAuthSingleton(this IServiceCollection services, ServicePrincipleAuth config, string appRegistrationUrl = null)
        {
            services.AddSingleton<IAuthentication>(new AzureTokenAuthentication(config, appRegistrationUrl));
            AddFactoryIfNotAdded(services);
            return services;
        }

        /// <summary>
        /// Add an instance of IAuthentication for user authentication.
        /// </summary>
        /// <param name="services">The services to extend.</param>
        /// <param name="config">The user authentication configuration to initialise with.</param>
        /// <returns>IServiceCollection.</returns>
        public static IServiceCollection AddAzureAuthSingleton(this IServiceCollection services, UserAuth config)
        {
            services.AddSingleton<IAuthentication>(new AzureTokenAuthentication(config));
            AddFactoryIfNotAdded(services);
            return services;
        }

        /// <summary>
        /// Add the generic service factory from Cloud.Core for the IAuthentication type.  This allows multiple named instances of the same instance.
        /// </summary>
        /// <param name="services">Service collection to extend.</param>
        private static void AddFactoryIfNotAdded(IServiceCollection services)
        {
            if (!services.Any(x => x.ServiceType == typeof(NamedInstanceFactory<IAuthentication>)))
            {
                // Service Factory doesn't exist, so we add it to ensure it's always available.
                services.AddSingleton<NamedInstanceFactory<IAuthentication>>();
            }
        }
    }
}
