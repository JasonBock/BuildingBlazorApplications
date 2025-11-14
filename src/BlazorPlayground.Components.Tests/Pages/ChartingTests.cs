using BlazorPlayground.Components.Extensions;
using BlazorPlayground.Components.Pages;
using Bunit;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Newtonsoft.Json.Linq;
using Rocks;
using System.Collections.Immutable;
using System.Net;

namespace BlazorPlayground.Components.Tests.Pages;

public sealed class ChartingTests
{
	[Test]
	public async Task CreateSequenceWithEmulatorAsync()
	{
		using (Assert.Multiple())
		{
			using var context = new BunitContext();
			context.Services.AddPlaygroundConfiguration();
			context.Services.AddSingleton(
				new IHttpClientFactoryMakeExpectations().Instance());
			context.JSInterop.Mode = JSRuntimeMode.Strict;

			var chartingInterop = context.JSInterop.SetupModule(
				Constants.ChartingFileLocation);
			IReadOnlyList<object?>? arguments = null;

			chartingInterop.SetupVoid(Constants.ChartingMethod, new InvocationMatcher(target =>
			{
				arguments = target.Arguments;
				return true;
			}))
			.SetVoidResult();

			var charting = context.Render<Charting>();
			var chartingButton = charting.Find("#start");
			var chartingInput = charting.Find("input");
			var chartingCurrentInput = charting.Find("#currentSequence");

			charting.Instance.Start = "5";
			await chartingButton.ClickAsync(new MouseEventArgs());

			using (Assert.Multiple())
			{
				await Assert.That(chartingCurrentInput.TextContent).Contains("5, 8, 4, 2, 1");
				await Assert.That(arguments![0]).IsTypeOf<ElementReference>();
			
				var sequence = (ImmutableArray<int>)arguments[1]!;
				await Assert.That(sequence).IsEquivalentTo([5, 8, 4, 2, 1]);

				var labels = (string[])arguments[2]!;
				await Assert.That(labels).IsEquivalentTo(["1", "2", "3", "4", "5"]);
			}
		}
	}

	[Test]
	public async Task CreateSequenceWithMockAsync()
	{
		using (Assert.Multiple())
		{
			using var mockContext = new RockContext();
			var collatzExpectations = mockContext.Create<ICollatzCreateExpectations>();
			collatzExpectations.Setups.Generate<int>(5)
				.ReturnValue([5, 8, 4, 2, 1]);

			object?[]? arguments = null;

			var objectReferenceExpectations = mockContext.Create<IJSObjectReferenceCreateExpectations>();
			objectReferenceExpectations.Setups.InvokeAsync<IJSVoidResult>(
				Constants.ChartingMethod, Arg.Any<object?[]?>())
				.Callback((identifier, args) =>
				{
					arguments = args;				
					return ValueTask.FromResult(new IJSVoidResultMakeExpectations().Instance());
				});

			var runtimeExpectations = mockContext.Create<IJSRuntimeCreateExpectations>();
			runtimeExpectations.Setups.InvokeAsync<IJSObjectReference>(
				 Constants.Import, new object?[] { Constants.ChartingFileLocation })
				 .ReturnValue(ValueTask.FromResult(objectReferenceExpectations.Instance()));

			using var context = new BunitContext();
			context.Services.AddSingleton(collatzExpectations.Instance());
			context.Services.Add(new(typeof(IJSRuntime), runtimeExpectations.Instance()));
			context.Services.AddSingleton(new IHttpClientFactoryMakeExpectations().Instance());

			var charting = context.Render<Charting>();
			var chartingButton = charting.Find("#start");
			var chartingInput = charting.Find("input");
			var chartingCurrentInput = charting.Find("#currentSequence");

			charting.Instance.Start = "5";
			await chartingButton.ClickAsync(new MouseEventArgs());
			await Assert.That(chartingCurrentInput.TextContent).Contains("5, 8, 4, 2, 1");

			await Assert.That(arguments![0]).IsTypeOf<ElementReference>();

			var sequence = (ImmutableArray<int>)arguments[1]!;
			await Assert.That(sequence).IsEquivalentTo([5, 8, 4, 2, 1]);

			var labels = (string[])arguments[2]!;
			await Assert.That(labels).IsEquivalentTo(["1", "2", "3", "4", "5"]);
		}
	}

	[Test]
	public async Task CreateRandomStartValueWithMockAsync()
	{
		using (Assert.Multiple())
		{
			using var response = new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.OK,
				Content = new StringContent("5")
			};

			HttpRequestMessage? requestMessage = null;
			object?[]? arguments = null;

			using var mockContext = new RockContext();
			var messageHandlerExpectations = mockContext.Create<HttpMessageHandlerCreateExpectations>();
			messageHandlerExpectations.Setups.SendAsync(
				Arg.Any<HttpRequestMessage>(), Arg.Any<CancellationToken>())
				.Callback((message, token) =>
				{
					requestMessage = message;
#pragma warning disable CA2025 // Do not pass 'IDisposable' instances into unawaited tasks
				   return Task.FromResult(response);
#pragma warning restore CA2025 // Do not pass 'IDisposable' instances into unawaited tasks
				});

			using var messageHandler = messageHandlerExpectations.Instance();
			using var client = new HttpClient(messageHandler);

			var clientFactoryExpectations = mockContext.Create<IHttpClientFactoryCreateExpectations>();
			clientFactoryExpectations.Setups.CreateClient(Arg.Any<string>())
				.ReturnValue(client);

			var collatzExpectations = mockContext.Create<ICollatzCreateExpectations>();
			collatzExpectations.Setups.Generate<int>(5)
				.ReturnValue([5, 8, 4, 2, 1]);

			var objectReferenceExpectations = mockContext.Create<IJSObjectReferenceCreateExpectations>();
			objectReferenceExpectations.Setups.InvokeAsync<IJSVoidResult>(
				Constants.ChartingMethod, Arg.Any<object?[]?>())
				.Callback((identifier, args) =>
				{
					arguments = args;
					return ValueTask.FromResult(new IJSVoidResultMakeExpectations().Instance());
				});

			var runtimeExpectations = mockContext.Create<IJSRuntimeCreateExpectations>();
			runtimeExpectations.Setups.InvokeAsync<IJSObjectReference>(
				 Constants.Import, new object?[] { Constants.ChartingFileLocation })
				 .ReturnValue(ValueTask.FromResult(objectReferenceExpectations.Instance()));

			using var context = new BunitContext();
			context.Services.AddSingleton(collatzExpectations.Instance());
			context.Services.Add(new(typeof(IJSRuntime), runtimeExpectations.Instance()));
			context.Services.AddSingleton(clientFactoryExpectations.Instance());

			var charting = context.Render<Charting>();
			var chartingButton = charting.Find("#random");
			var chartingInput = charting.Find("input");
			var chartingCurrentInput = charting.Find("#currentSequence");

			await chartingButton.ClickAsync(new MouseEventArgs());
			await Assert.That(charting.Instance.Start).IsEqualTo("5");
			await Assert.That(chartingCurrentInput.TextContent).Contains("5, 8, 4, 2, 1");

			await Assert.That(requestMessage!.RequestUri).IsEqualTo(new Uri("http://localhost:5128/random"));

			await Assert.That(arguments![0]).IsTypeOf<ElementReference>();

			var sequence = (ImmutableArray<int>)arguments[1]!;
			await Assert.That(sequence).IsEquivalentTo([5, 8, 4, 2, 1]);

			var labels = (string[])arguments[2]!;
			await Assert.That(labels).IsEquivalentTo(["1", "2", "3", "4", "5"]);
		}
	}
}