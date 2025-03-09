using BlazorPlayground.Components.Pages;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using NUnit.Framework;
using System.Collections.Immutable;

namespace BlazorPlayground.Components.Tests.Pages;

public static class LocationTests
{
	[Test]
	public static void RenderWithEmulator()
	{
		const double latitude = 1.0d;
		const double longitude = 2.0d;
		const double accuracy = 3.0d;

		using var context = new BUnitTestContext();
		context.JSInterop.Mode = JSRuntimeMode.Strict;

		var locationInterop = context.JSInterop.SetupModule(Constants.LocationFileLocation);
		locationInterop.Setup<object>(Constants.LocationMethod, new InvocationMatcher(target =>
		{
			var reference = (DotNetObjectReference<Location>)target.Arguments[0]!;
			reference.Value.Change(latitude, longitude, accuracy);
			return true;
		}))
		.SetResult(Task.CompletedTask);

		var location = context.RenderComponent<Location>();
		var latitudeListItem = location.Find("#latitudeId");

		location.Render();

		Assert.That(latitudeListItem.ToMarkup(),
			Does.Contain("Latitude: 1"));
	}

	//[Test]
	//public static async Task RenderWithMockAsync()
	//{

	//}
}