using BlazorPlayground.Components.Pages;
using Bunit;
using Microsoft.AspNetCore.Components.Web;
using NUnit.Framework;

namespace BlazorPlayground.Components.Tests.Pages;

public static class ChartingTests
{
	[Test]
	public static async Task CreateSequenceWithEmulatorAsync()
	{
		using var context = new BUnitTestContext();
		context.JSInterop.Mode = JSRuntimeMode.Strict;

		var chartingInterop = context.JSInterop.SetupModule(Constants.ChartingFileLocation);
		chartingInterop.Setup<object>(Constants.ChartingMethod, new InvocationMatcher(target =>
		{
			//Assert.That(target.Arguments[0] is ElementReference);
			return true;
		}))
		.SetResult(Task.CompletedTask);

		var charting = context.RenderComponent<Charting>();
		var chartingButton = charting.Find("button");
		var chartingInput = charting.Find("input");
		var chartingCurrentInput = charting.Find("#currentSequence");

		await Assert.MultipleAsync(async () =>
		{
			charting.Instance.Start = "44";
			await chartingButton.ClickAsync(new MouseEventArgs());
			Assert.That(chartingCurrentInput.ToMarkup(),
				Does.Contain("44,"));
		});
	}

	//[Test]
	//public static async Task CreateSequenceWithMockAsync()
	//{

	//}
}