using BlazorPlayground.Components;
using BlazorPlayground.Components.Extensions;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.AddRazorComponents()
	 .AddInteractiveServerComponents()
	 .AddInteractiveWebAssemblyComponents();

builder.Services.AddPlaygroundConfiguration();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseWebAssemblyDebugging();
}
else
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
}


app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode()
    .AddInteractiveWebAssemblyRenderMode()
    .AddAdditionalAssemblies(
		typeof(BlazorPlayground.Client._Imports).Assembly,
		typeof(BlazorPlayground.Components.Pages.Counter).Assembly);

app.Run();
