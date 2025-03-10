using BlazorPlayground.Components.Extensions;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.Services.AddPlaygroundConfiguration();
builder.Services.AddHttpClient(
	"Playground", 
	client => client.BaseAddress = new Uri("http://localhost:5128"));

await builder.Build().RunAsync();