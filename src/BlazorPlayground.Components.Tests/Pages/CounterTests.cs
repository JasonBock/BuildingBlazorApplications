using BlazorPlayground.Components.Pages;
using Bunit;
using NUnit.Framework;

namespace BlazorPlayground.Components.Tests.Pages;

public static class CounterTests
{
	[Test]
	public static void IncrementViaClick()
	{
		using var context = new BUnitTestContext();
		var counter = context.RenderComponent<Counter>();
		var counterCount = counter.Find("p");
		var counterButton = counter.Find("button");

		Assert.Multiple(() =>
		{
			Assert.That(counterCount.ToMarkup(),
				Does.Contain("""<p role="status">Current count: 0</p>"""));
			counterButton.Click();
			Assert.That(counterCount.ToMarkup(),
				Does.Contain("""<p role="status">Current count: 1</p>"""));
		});
	}
}