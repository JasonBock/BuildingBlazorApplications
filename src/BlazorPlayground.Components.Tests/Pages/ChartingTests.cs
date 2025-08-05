using BlazorPlayground.Components.Extensions;
using BlazorPlayground.Components.Pages;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using NUnit.Framework;
using Rocks;
using System.Collections.Immutable;
using System.Net;

namespace BlazorPlayground.Components.Tests.Pages;

public static class ChartingTests
{
	[Test]
	public static async Task CreateSequenceWithEmulatorAsync()
	{
		using var context = new BUnitTestContext();
		context.Services.AddPlaygroundConfiguration();
		context.Services.AddSingleton(
			new IHttpClientFactoryMakeExpectations().Instance());
		context.JSInterop.Mode = JSRuntimeMode.Strict;

		var chartingInterop = context.JSInterop.SetupModule(
			Constants.ChartingFileLocation);
		chartingInterop.SetupVoid(Constants.ChartingMethod, new InvocationMatcher(target =>
		{
			Assert.That(target.Arguments[0] is ElementReference);

			var sequence = (ImmutableArray<int>)target.Arguments[1]!;
			Assert.That(sequence, Is.EquivalentTo([5, 8, 4, 2, 1]));

			var labels = (string[])target.Arguments[2]!;
			Assert.That(labels, Is.EquivalentTo(["1", "2", "3", "4", "5"]));

			return true;
		}))
		.SetVoidResult();

		var charting = context.RenderComponent<Charting>();
		var chartingButton = charting.Find("#start");
		var chartingInput = charting.Find("input");
		var chartingCurrentInput = charting.Find("#currentSequence");

		await Assert.MultipleAsync(async () =>
		{
			charting.Instance.Start = "5";
			await chartingButton.ClickAsync(new MouseEventArgs());
			Assert.That(chartingCurrentInput.ToMarkup(),
				Does.Contain("5, 8, 4, 2, 1"));
		});
	}

	[Test]
	public static async Task CreateSequenceWithMockAsync()
	{
		var collatzExpectations = new ICollatzCreateExpectations();
		collatzExpectations.Methods.Generate<int>(5)
			.ReturnValue([5, 8, 4, 2, 1]);

#pragma warning disable CA2012 // Use ValueTasks correctly
		var objectReferenceExpectations = new IJSObjectReferenceCreateExpectations();
		objectReferenceExpectations.Methods.InvokeAsync<IJSVoidResult>(
			Constants.ChartingMethod,
			Arg.Validate<object?[]?>(values =>
			{
				Assert.That(values![0] is ElementReference);

				var sequence = (ImmutableArray<int>)values[1]!;
				Assert.That(sequence, Is.EquivalentTo([5, 8, 4, 2, 1]));

				var labels = (string[])values[2]!;
				Assert.That(labels, Is.EquivalentTo(["1", "2", "3", "4", "5"]));

				return true;
			}))
			.ReturnValue(ValueTask.FromResult(new IJSVoidResultMakeExpectations().Instance()));

		var runtimeExpectations = new IJSRuntimeCreateExpectations();
		runtimeExpectations.Methods.InvokeAsync<IJSObjectReference>(
			 Constants.Import, new object?[] { Constants.ChartingFileLocation })
			 .ReturnValue(ValueTask.FromResult(objectReferenceExpectations.Instance()));
#pragma warning restore CA2012 // Use ValueTasks correctly

		using var context = new BUnitTestContext();
		context.Services.AddSingleton(collatzExpectations.Instance());
		context.Services.Add(new(typeof(IJSRuntime), runtimeExpectations.Instance()));
		context.Services.AddSingleton(new IHttpClientFactoryMakeExpectations().Instance());

		var charting = context.RenderComponent<Charting>();
		var chartingButton = charting.Find("#start");
		var chartingInput = charting.Find("input");
		var chartingCurrentInput = charting.Find("#currentSequence");

		await Assert.MultipleAsync(async () =>
		{
			charting.Instance.Start = "5";
			await chartingButton.ClickAsync(new MouseEventArgs());
			Assert.That(chartingCurrentInput.ToMarkup(),
				Does.Contain("5, 8, 4, 2, 1"));
		});

		objectReferenceExpectations.Verify();
		runtimeExpectations.Verify();
		collatzExpectations.Verify();
	}

	[Test]
	public static async Task CreateRandomStartValueWithMockAsync()
	{
		using var response = new HttpResponseMessage
		{
			StatusCode = HttpStatusCode.OK,
			Content = new StringContent("5")
		};

		var messageHandlerExpectations = new HttpMessageHandlerCreateExpectations();
		messageHandlerExpectations.Methods.SendAsync(
			Arg.Validate<HttpRequestMessage>(_ =>
			{
				Assert.That(_.RequestUri, 
					Is.EqualTo(new Uri("http://localhost:5128/random")));
				return true;
			}), Arg.Any<CancellationToken>())
			.ReturnValue(Task.FromResult(response));

		using var messageHandler = messageHandlerExpectations.Instance();
		using var client = new HttpClient(messageHandler);

		var clientFactoryExpectations = new IHttpClientFactoryCreateExpectations();
		clientFactoryExpectations.Methods.CreateClient(Arg.Any<string>())
			.ReturnValue(client);

		var collatzExpectations = new ICollatzCreateExpectations();
		collatzExpectations.Methods.Generate<int>(5)
			.ReturnValue([5, 8, 4, 2, 1]);

#pragma warning disable CA2012 // Use ValueTasks correctly
		var objectReferenceExpectations = new IJSObjectReferenceCreateExpectations();
		objectReferenceExpectations.Methods.InvokeAsync<IJSVoidResult>(
			Constants.ChartingMethod,
			Arg.Validate<object?[]?>(values =>
			{
				Assert.That(values![0] is ElementReference);

				var sequence = (ImmutableArray<int>)values[1]!;
				Assert.That(sequence, Is.EquivalentTo([5, 8, 4, 2, 1]));

				var labels = (string[])values[2]!;
				Assert.That(labels, Is.EquivalentTo(["1", "2", "3", "4", "5"]));

				return true;
			}))
			.ReturnValue(ValueTask.FromResult(new IJSVoidResultMakeExpectations().Instance()));

		var runtimeExpectations = new IJSRuntimeCreateExpectations();
		runtimeExpectations.Methods.InvokeAsync<IJSObjectReference>(
			 Constants.Import, new object?[] { Constants.ChartingFileLocation })
			 .ReturnValue(ValueTask.FromResult(objectReferenceExpectations.Instance()));
#pragma warning restore CA2012 // Use ValueTasks correctly

		using var context = new BUnitTestContext();
		context.Services.AddSingleton(collatzExpectations.Instance());
		context.Services.Add(new(typeof(IJSRuntime), runtimeExpectations.Instance()));
		context.Services.AddSingleton(clientFactoryExpectations.Instance());

		var charting = context.RenderComponent<Charting>();
		var chartingButton = charting.Find("#random");
		var chartingInput = charting.Find("input");
		var chartingCurrentInput = charting.Find("#currentSequence");

		await Assert.MultipleAsync(async () =>
		{
			await chartingButton.ClickAsync(new MouseEventArgs());
			Assert.That(charting.Instance.Start, Is.EqualTo("5"));
			Assert.That(chartingCurrentInput.ToMarkup(),
				Does.Contain("5, 8, 4, 2, 1"));
		});

		objectReferenceExpectations.Verify();
		runtimeExpectations.Verify();
		collatzExpectations.Verify();
		clientFactoryExpectations.Verify();
		messageHandlerExpectations.Verify();
	}
}