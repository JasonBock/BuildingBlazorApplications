using BlazorPlayground.Components.Pages;
using Bunit;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NUnit.Framework;

namespace BlazorPlayground.Components.Tests.Pages;

public static class CounterTests
{
	[Test]
	public static void IncrementViaClick()
	{
		var id = Guid.NewGuid();

		var identifierExpectations = new IIdentifierCreateExpectations();
		identifierExpectations.Properties.Getters.Id()
			.ReturnValue(id).ExpectedCallCount(2);

		using var context = new BUnitTestContext();
		context.Services.AddSingleton(new ILoggerMakeExpectations<Counter>().Instance());
		context.Services.AddSingleton(identifierExpectations.Instance());

		var counter = context.RenderComponent<Counter>();
		var counterId = counter.Find("#id");
		var counterCount = counter.Find("#count");
		var counterButton = counter.Find("button");

		Assert.Multiple(() =>
		{
			Assert.That(counterId.ToMarkup(),
				Does.Contain(id.ToString()));
			Assert.That(counterCount.ToMarkup(),
				Does.Contain("""<p id="count" role="status">Current count: 0</p>"""));
			counterButton.Click();
			Assert.That(counterCount.ToMarkup(),
				Does.Contain("""<p id="count" role="status">Current count: 1</p>"""));
		});

		identifierExpectations.Verify();
	}
}