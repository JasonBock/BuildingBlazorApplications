using BlazorPlayground.Components.Pages;
using Bunit;
using NUnit.Framework;

namespace BlazorPlayground.Components.Tests.Pages;

public static class CounterTests
{
	[Test]
	public static void IncrementViaClick()
	{
		using var context = new Bunit.TestContext();
		var counter = context.RenderComponent<Counter>();
		counter.Find("button").Click();
		counter.Find("p").MarkupMatches("""<p role="status">Current count: 1</p>""");
	}
}