using BlazorPlayground.Components.Pages;
using Bunit;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using NUnit.Framework;
using Rocks;

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
		locationInterop.SetupVoid(Constants.LocationMethod, new InvocationMatcher(target =>
		{
			var reference = (DotNetObjectReference<Location>)target.Arguments[0]!;
			reference.Value.Change(latitude, longitude, accuracy);
			return true;
		}))
		.SetVoidResult();

		var location = context.RenderComponent<Location>();
		location.Render();

		Assert.Multiple(() =>
		{
			var locationInstance = location.Instance;

			Assert.That(locationInstance.Latitude, Is.EqualTo(latitude));
			Assert.That(locationInstance.Longitude, Is.EqualTo(longitude));
			Assert.That(locationInstance.Accuracy, Is.EqualTo(accuracy));
			Assert.That(locationInstance.BingMainUrl,
				Is.EqualTo("https://www.bing.com/maps/embed?h=400&w=500&cp=1~2&lvl=11&typ=d&sty=r&src=SHELL&FORM=MBEDV8"));
			Assert.That(locationInstance.BingLargeMapUrl,
				Is.EqualTo("https://www.bing.com/maps?cp=1~2&amp;sty=r&amp;lvl=11&amp;FORM=MBEDLD"));
			Assert.That(locationInstance.BingDirectionsUrl,
				Is.EqualTo("https://www.bing.com/maps/directions?cp=1~-2&amp;sty=r&amp;lvl=11&amp;rtp=~pos.1_2____&amp;FORM=MBEDLD"));

			Assert.That(location.Find("#latitudeId").ToMarkup(), Does.Contain("Latitude: 1"));
			Assert.That(location.Find("#longitudeId").ToMarkup(), Does.Contain("Longitude: 2"));
			Assert.That(location.Find("#accuracyId").ToMarkup(), Does.Contain("Accuracy: 3"));
			Assert.That(location.Find("#map").Attributes["src"]!.Value, Is.EqualTo(locationInstance.BingMainUrl));
			Assert.That(location.Find("#largeMapLink").Attributes["href"]!.Value, Is.EqualTo(locationInstance.BingLargeMapUrl));
			Assert.That(location.Find("#dirMapLink").Attributes["href"]!.Value, Is.EqualTo(locationInstance.BingDirectionsUrl));
		});
	}

	[Test]
	public static void RenderWithMock()
	{
		const double latitude = 1.0d;
		const double longitude = 2.0d;
		const double accuracy = 3.0d;

#pragma warning disable CA2012 // Use ValueTasks correctly
		var objectReferenceExpectations = new IJSObjectReferenceCreateExpectations();
		objectReferenceExpectations.Methods.InvokeAsync<IJSVoidResult>(
			Constants.LocationMethod,
			Arg.Validate<object?[]?>(values =>
			{
				var reference = (DotNetObjectReference<Location>)values![0]!;
				reference.Value.Change(latitude, longitude, accuracy);
				return true;
			}))
			.ReturnValue(ValueTask.FromResult(new IJSVoidResultMakeExpectations().Instance()));

		var runtimeExpectations = new IJSRuntimeCreateExpectations();
		runtimeExpectations.Methods.InvokeAsync<IJSObjectReference>(
			 Constants.Import, new object?[] { Constants.LocationFileLocation })
			 .ReturnValue(ValueTask.FromResult(objectReferenceExpectations.Instance()));
#pragma warning restore CA2012 // Use ValueTasks correctly

		using var context = new BUnitTestContext();
		context.Services.Add(new(typeof(IJSRuntime), runtimeExpectations.Instance()));
		var location = context.RenderComponent<Location>();
		location.Render();

		Assert.Multiple(() =>
		{
			var locationInstance = location.Instance;

			Assert.That(locationInstance.Latitude, Is.EqualTo(latitude));
			Assert.That(locationInstance.Longitude, Is.EqualTo(longitude));
			Assert.That(locationInstance.Accuracy, Is.EqualTo(accuracy));
			Assert.That(locationInstance.BingMainUrl,
				Is.EqualTo("https://www.bing.com/maps/embed?h=400&w=500&cp=1~2&lvl=11&typ=d&sty=r&src=SHELL&FORM=MBEDV8"));
			Assert.That(locationInstance.BingLargeMapUrl,
				Is.EqualTo("https://www.bing.com/maps?cp=1~2&amp;sty=r&amp;lvl=11&amp;FORM=MBEDLD"));
			Assert.That(locationInstance.BingDirectionsUrl,
				Is.EqualTo("https://www.bing.com/maps/directions?cp=1~-2&amp;sty=r&amp;lvl=11&amp;rtp=~pos.1_2____&amp;FORM=MBEDLD"));

			Assert.That(location.Find("#latitudeId").ToMarkup(), Does.Contain("Latitude: 1"));
			Assert.That(location.Find("#longitudeId").ToMarkup(), Does.Contain("Longitude: 2"));
			Assert.That(location.Find("#accuracyId").ToMarkup(), Does.Contain("Accuracy: 3"));
			Assert.That(location.Find("#map").Attributes["src"]!.Value, Is.EqualTo(locationInstance.BingMainUrl));
			Assert.That(location.Find("#largeMapLink").Attributes["href"]!.Value, Is.EqualTo(locationInstance.BingLargeMapUrl));
			Assert.That(location.Find("#dirMapLink").Attributes["href"]!.Value, Is.EqualTo(locationInstance.BingDirectionsUrl));
		});

		objectReferenceExpectations.Verify();
		runtimeExpectations.Verify();
	}
}