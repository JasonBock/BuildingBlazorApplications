using BlazorPlayground.Components.Pages;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Rocks;

namespace BlazorPlayground.Components.Tests.Pages;

public sealed class CounterTests
{
	[Test]
	public async Task IncrementViaClickAsync()
	{
		var id = Guid.NewGuid();
		const string renderName = "Server";

		using var mockContext = new RockContext();
		var identifierExpectations = mockContext.Create<IIdentifierCreateExpectations>();
		identifierExpectations.Setups.Id.Gets()
			.ReturnValue(id)
			.ExpectedCallCount(2);

		using var context = new BunitContext();
		context.Services.AddSingleton(new ILoggerMakeExpectations<Counter>().Instance());
		context.Services.AddSingleton(identifierExpectations.Instance());
		context.Renderer.SetRendererInfo(
			new RendererInfo(renderName, true));

		var counter = context.Render<Counter>();
		var counterRender = counter.Find("#render");
		var counterId = counter.Find("#id");
		var counterCount = counter.Find("#count");
		var counterButton = counter.Find("button");

		using (Assert.Multiple())
		{
			await Assert.That(counterRender.TextContent).Contains(renderName);
			await Assert.That(counterId.TextContent).Contains(id.ToString());
			await Assert.That(counterCount.TextContent).Contains("Current count: 0");

			await counterButton.ClickAsync();

			await Assert.That(counterCount.TextContent).Contains("Current count: 1");
		}
	}
}