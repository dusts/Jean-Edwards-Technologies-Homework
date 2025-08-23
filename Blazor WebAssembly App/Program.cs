using Blazor_WebAssembly_App;
using Blazor_WebAssembly_App.ApiServices;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

internal class Program
{
    private static async Task Main(string[] args)
    {
        var builder = WebAssemblyHostBuilder.CreateDefault(args);
        builder.RootComponents.Add<App>("#app");
        builder.RootComponents.Add<HeadOutlet>("head::after");

        // Read BackendApiUrl from configuration
        var blazorBackendApiUrl = builder.Configuration["BlazorBackendApiUrl-https"]
            ?? throw new InvalidOperationException("BlazorBackendApiUrl-https is missing in configuration.");
        Console.WriteLine($"Backend API URL: {blazorBackendApiUrl}");

        // Ensure trailing slash for BaseAddress
        if (!blazorBackendApiUrl.EndsWith("/"))
            blazorBackendApiUrl += "/";

        // Register named HttpClient
        builder.Services.AddHttpClient("BlazorBackendApi", client =>
        {
            client.BaseAddress = new Uri(blazorBackendApiUrl);
        });

        builder.Services.AddScoped<BlazorBackendApiServices>();

        await builder.Build().RunAsync();
    }
}