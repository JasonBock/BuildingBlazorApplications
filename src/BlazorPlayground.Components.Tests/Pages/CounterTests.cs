using BlazorPlayground.Components.Pages;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;
using Rocks;

namespace BlazorPlayground.Components.Tests.Pages;

public static class CounterTests
{
	[Test]
	public static void IncrementViaClick()
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

		using (Assert.EnterMultipleScope())
		{
			Assert.That(counterRender.TextContent,
				Does.Contain(renderName));
			Assert.That(counterId.TextContent,
				Does.Contain(id.ToString()));
			Assert.That(counterCount.TextContent,
				Does.Contain("Current count: 0"));
			counterButton.Click();
			Assert.That(counterCount.TextContent,
				Does.Contain("Current count: 1"));
		}
	}
}