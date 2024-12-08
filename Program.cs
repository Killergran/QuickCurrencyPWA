using Microsoft.AspNetCore.Components.Web;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Blazored.LocalStorage;
using QuickCurrencyPWA;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");
builder.RootComponents.Add<HeadOutlet>("head::after");

builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });
builder.Services.AddScoped(sp => new CurrencyService(new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) }));

builder.Services.AddBlazoredLocalStorage();

await builder.Build().RunAsync();
