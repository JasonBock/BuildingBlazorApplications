using BlazorPlayground.Components.Pages;
using Bunit;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Rocks;

namespace BlazorPlayground.Components.Tests.Pages;

public class LocationTests
{
	[Test]
	public async Task RenderWithEmulatorAsync()
	{
		const double latitude = 1.0d;
		const double longitude = 2.0d;
		const double accuracy = 3.0d;

		using var context = new BunitContext();
		context.JSInterop.Mode = JSRuntimeMode.Strict;

		var locationInterop = context.JSInterop.SetupModule(Constants.LocationFileLocation);
		locationInterop.SetupVoid(Constants.LocationMethod, new InvocationMatcher(target =>
		{
			var reference = (DotNetObjectReference<Location>)target.Arguments[0]!;
			reference.Value.Change(latitude, longitude, accuracy);
			return true;
		}))
		.SetVoidResult();

		var location = context.Render<Location>();
		location.Render();

		using (Assert.Multiple())
		{
			var locationInstance = location.Instance;

			await Assert.That(locationInstance.Latitude).IsEqualTo(latitude);
			await Assert.That(locationInstance.Longitude).IsEqualTo(longitude);
			await Assert.That(locationInstance.Accuracy).IsEqualTo(accuracy);
			await Assert.That(locationInstance.BingMainUrl)
				.IsEqualTo("https://www.bing.com/maps/embed?h=400&w=500&cp=1~2&lvl=11&typ=d&sty=r&src=SHELL&FORM=MBEDV8");
			await Assert.That(locationInstance.BingLargeMapUrl)
				.IsEqualTo("https://www.bing.com/maps?cp=1~2&amp;sty=r&amp;lvl=11&amp;FORM=MBEDLD");
			await Assert.That(locationInstance.BingDirectionsUrl)
				.IsEqualTo("https://www.bing.com/maps/directions?cp=1~-2&amp;sty=r&amp;lvl=11&amp;rtp=~pos.1_2____&amp;FORM=MBEDLD");

			await Assert.That(location.Find("#latitudeId").TextContent).Contains("Latitude: 1");
			await Assert.That(location.Find("#longitudeId").TextContent).Contains("Longitude: 2");
			await Assert.That(location.Find("#accuracyId").TextContent).Contains("Accuracy: 3");
			await Assert.That(location.Find("#map").Attributes["src"]!.Value).IsEqualTo(locationInstance.BingMainUrl);
			await Assert.That(location.Find("#largeMapLink").Attributes["href"]!.Value).IsEqualTo(locationInstance.BingLargeMapUrl);
			await Assert.That(location.Find("#dirMapLink").Attributes["href"]!.Value).IsEqualTo(locationInstance.BingDirectionsUrl);
		}
	}

	[Test]
	public async Task RenderWithMockAsync()
	{
		const double latitude = 1.0d;
		const double longitude = 2.0d;
		const double accuracy = 3.0d;

		using var mockContext = new RockContext();
		var objectReferenceExpectations = mockContext.Create<IJSObjectReferenceCreateExpectations>();
		objectReferenceExpectations.Setups.InvokeAsync<IJSVoidResult>(
			Constants.LocationMethod, Arg.Any<object?[]?>())
			.Callback((identifier, args) =>
			{
				var reference = (DotNetObjectReference<Location>)args![0]!;
				reference.Value.Change(latitude, longitude, accuracy);
				return ValueTask.FromResult(new IJSVoidResultMakeExpectations().Instance());
			});

		var runtimeExpectations = mockContext.Create<IJSRuntimeCreateExpectations>();
		runtimeExpectations.Setups.InvokeAsync<IJSObjectReference>(
			 Constants.Import, new object?[] { Constants.LocationFileLocation })
			 .ReturnValue(ValueTask.FromResult(objectReferenceExpectations.Instance()));

		using var context = new BunitContext();
		context.Services.Add(new(typeof(IJSRuntime), runtimeExpectations.Instance()));
		var location = context.Render<Location>();
		location.Render();

		using (Assert.Multiple())
		{
			var locationInstance = location.Instance;

			await Assert.That(locationInstance.Latitude).IsEqualTo(latitude);
			await Assert.That(locationInstance.Longitude).IsEqualTo(longitude);
			await Assert.That(locationInstance.Accuracy).IsEqualTo(accuracy);
			await Assert.That(locationInstance.BingMainUrl)
				.IsEqualTo("https://www.bing.com/maps/embed?h=400&w=500&cp=1~2&lvl=11&typ=d&sty=r&src=SHELL&FORM=MBEDV8");
			await Assert.That(locationInstance.BingLargeMapUrl)
				.IsEqualTo("https://www.bing.com/maps?cp=1~2&amp;sty=r&amp;lvl=11&amp;FORM=MBEDLD");
			await Assert.That(locationInstance.BingDirectionsUrl)
				.IsEqualTo("https://www.bing.com/maps/directions?cp=1~-2&amp;sty=r&amp;lvl=11&amp;rtp=~pos.1_2____&amp;FORM=MBEDLD");

			await Assert.That(location.Find("#latitudeId").TextContent).Contains("Latitude: 1");
			await Assert.That(location.Find("#longitudeId").TextContent).Contains("Longitude: 2");
			await Assert.That(location.Find("#accuracyId").TextContent).Contains("Accuracy: 3");
			await Assert.That(location.Find("#map").Attributes["src"]!.Value).IsEqualTo(locationInstance.BingMainUrl);
			await Assert.That(location.Find("#largeMapLink").Attributes["href"]!.Value).IsEqualTo(locationInstance.BingLargeMapUrl);
			await Assert.That(location.Find("#dirMapLink").Attributes["href"]!.Value).IsEqualTo(locationInstance.BingDirectionsUrl);
		}
	}
}