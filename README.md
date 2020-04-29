# **Cloud.Core.Authentication.AzureAuth** 
[![Build status](https://dev.azure.com/cloudcoreproject/CloudCore/_apis/build/status/Cloud.Core%20Packages/Cloud.Core.Authentication.AzureAuth_Package)](https://dev.azure.com/cloudcoreproject/CloudCore/_build/latest?definitionId=14)
![Code Coverage](https://cloud1core.blob.core.windows.net/codecoveragebadges/Cloud.Core.Authentication.AzureAuth-LineCoverage.png) 
[![Cloud.Core.Authentication.AzureAuth package in Cloud.Core feed in Azure Artifacts](https://feeds.dev.azure.com/cloudcoreproject/dfc5e3d0-a562-46fe-8070-7901ac8e64a0/_apis/public/Packaging/Feeds/8949198b-5c74-42af-9d30-e8c462acada6/Packages/7c02cd5a-b832-41e3-bbd3-c5ec7a3376a3/Badge)](https://dev.azure.com/cloudcoreproject/CloudCore/_packaging?_a=package&feed=8949198b-5c74-42af-9d30-e8c462acada6&package=7c02cd5a-b832-41e3-bbd3-c5ec7a3376a3&preferRelease=true)



<div id="description">

Azure specific implementation of `IAuthentication` interface.  Used for injecting Azure bearer authentication into supported packages.

</div>

## Usage

There are four authentication types available.

- Certificate
- User
- Service Principle (App Registration)
- Msi

A bearer token can be generated for each type.  A concrete instance of AzureAuthentication can be created as follows:

```csharp
IAuthentication auth = new AzureAuthentication("<appId>", "<appSecret>", "<tenantId>");
```

This can then be injected into services that expect the `IAuthentication` interface as a dependency, such as a web Api service which needs a bearer token sent with the header of it's requests: 

```csharp
public class ApiServiceSample
{
    private readonly IAuthentication _auth;
    private readonly HttpClient _httpClient;
    private readonly ILogger<ApiServiceSample> _logger;
    private const int MaxWaitTimeInSeconds = 5;
    private const int MaxRetryTimes = 3;

    public ApiServiceSample(HttpClient httpClient, ILogger<ApiServiceSample> logger, IAuthentication auth)
    {
        _httpClient = httpClient;
        _logger = logger;
        _auth = auth; // auth injected.
    }

    public async Task<List<CustomPattern>> GetPatternsAsync()
    {
        var token = _auth.AccessToken;

        // Add auth access token to the header.
        _httpClient.DefaultRequestHeaders.Add("Authorization", "bearer " + token.BearerToken);

        HttpResponseMessage response = null;

        Policy.Handle<TimeoutException>()
            .WaitAndRetryAsync(MaxRetryTimes, retryAttempt => TimeSpan.FromSeconds(Math.Pow(MaxWaitTimeInSeconds, retryAttempt))).ExecuteAsync(async () =>
            {
                response = await _httpClient.GetAsync("api/probe");
            }).GetAwaiter().GetResult();
        
        response.EnsureSuccessStatusCode();

        string content = await response.Content.ReadAsStringAsync();

        return JsonConvert.DeserializeObject<List<CustomPattern>>(content);
    }
}
```

## Test Coverage
A threshold will be added to this package to ensure the test coverage is above 80% for branches, functions and lines.  If it's not above the required threshold 
(threshold that will be implemented on ALL of the core repositories to gurantee a satisfactory level of testing), then the build will fail.

## Compatibility
This package has has been written in .net Standard and can be therefore be referenced from a .net Core or .net Framework application. The advantage of utilising from a .net Core application, 
is that it can be deployed and run on a number of host operating systems, such as Windows, Linux or OSX.  Unlike referencing from the a .net Framework application, which can only run on 
Windows (or Linux using Mono).
 
## Setup
This package is built using .net Standard 2.1 and requires the .net Core 3.1 SDK, it can be downloaded here: 
https://www.microsoft.com/net/download/dotnet-core/

IDE of Visual Studio or Visual Studio Code, can be downloaded here:
https://visualstudio.microsoft.com/downloads/

## How to access this package
All of the Cloud.Core.* packages are published to a internal NuGet feed.  To consume this on your local development machine, please add the following feed to your feed sources in Visual Studio:
https://pkgs.dev.azure.com/cloudcoreproject/CloudCore/_packaging/Cloud.Core/nuget/v3/index.json
 
For help setting up, follow this article: https://docs.microsoft.com/en-us/vsts/package/nuget/consume?view=vsts


<img src="https://cloud1core.blob.core.windows.net/icons/cloud_core_small.PNG" />

