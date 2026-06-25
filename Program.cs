using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using pRHosaApp1;
using Blazored.LocalStorage;
using pRHosaApp1.Services;
var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.RootComponents.Add<HeadOutlet>("head::after");
builder.Services.AddScoped<API>();
builder.Services.AddScoped<SessionService>();
builder.Services.AddScoped<NotificationService>();
builder.Services.AddBlazoredLocalStorage();

builder.Services.AddHttpClient("API", client =>
{
    client.BaseAddress = new Uri(builder.Configuration["APIServer:Url"]!);
    client.DefaultRequestHeaders.Add("Accept", "application/json");
});

await builder.Build().RunAsync();
