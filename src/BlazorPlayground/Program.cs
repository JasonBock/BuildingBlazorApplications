using BlazorPlayground.Components;
using BlazorPlayground.Components.Extensions;
using Microsoft.AspNetCore.Mvc;

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

app.UseStatusCodePagesWithReExecute("/not-found", createScopeForStatusCodePages: true);
app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
	.AddInteractiveServerRenderMode()
	.AddInteractiveWebAssemblyRenderMode()
	.AddAdditionalAssemblies(
		typeof(BlazorPlayground.Components.Pages.Counter).Assembly);

app.MapGet("/random", async ([FromServices] IHttpClientFactory httpClientFactory) =>
{
	using var client = httpClientFactory.CreateClient();

	// https://www.random.org/clients/http/api/
	// Let's get one number between 1 and 1_000_000,
	// 1 column, base 10, in plain text format.
	var response = await client.GetAsync(
		 new Uri("https://www.random.org/integers/?num=1&min=1&max=1000000&col=1&base=10&format=plain&rnd=new"));
	var content = await response.Content.ReadAsStringAsync();
	return Results.Ok(content);
});

app.Run();
