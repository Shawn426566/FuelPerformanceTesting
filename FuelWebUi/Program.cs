using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using FuelWebUi;
using Microsoft.Kiota.Abstractions.Authentication;
using Microsoft.Kiota.Http.HttpClientLibrary;
using FuelWebUi.ApiClient;
using Microsoft.Extensions.Logging;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Logging.SetMinimumLevel(LogLevel.Information);

// Configure HttpClient for API
var apiBaseUrl = "http://localhost:5114";

// Register Kiota dependencies
builder.Services.AddScoped<IAuthenticationProvider, AnonymousAuthenticationProvider>();
builder.Services.AddScoped(sp => 
{
    var authProvider = sp.GetRequiredService<IAuthenticationProvider>();
    var httpClient = new HttpClient();
    var adapter = new HttpClientRequestAdapter(authProvider, httpClient: httpClient)
    {
        BaseUrl = apiBaseUrl
    };
    return new FuelApiClient(adapter);
});

await builder.Build().RunAsync();
