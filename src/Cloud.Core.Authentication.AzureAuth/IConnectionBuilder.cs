namespace Cloud.Core.Authentication.AzureAuth
{
    using System.Threading.Tasks;

    /// <summary>
    /// Interface for classes that build connection strings.
    /// </summary>
    public interface IConnectionStringBuilder
    {
        /// <summary>
        /// Builds a connection string to the service using the authentication mechanism passed.
        /// Generates an access token automatically.
        /// </summary>
        /// <returns>Task{System.String} connection string.</returns>
        Task<string> BuildConnectionString();
    }
}
