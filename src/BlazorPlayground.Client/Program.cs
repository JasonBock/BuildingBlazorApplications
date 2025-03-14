using BlazorPlayground.Components.Extensions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddPlaygroundConfiguration();

await builder.Build().RunAsync();