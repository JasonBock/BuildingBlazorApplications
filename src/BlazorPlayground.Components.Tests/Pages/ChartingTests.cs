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

public sealed class ChartingTests
{
	[Test]
	public async Task CreateSequenceWithEmulatorAsync()
	{
		using (Assert.EnterMultipleScope())
		{
			using var context = new BunitContext();
			context.Services.AddPlaygroundConfiguration();
			context.Services.AddSingleton(
				new IHttpClientFactoryMakeExpectations().Instance());
			context.JSInterop.Mode = JSRuntimeMode.Strict;

			var chartingInterop = context.JSInterop.SetupModule(
				Constants.ChartingFileLocation);
			chartingInterop.SetupVoid(Constants.ChartingMethod, new InvocationMatcher(target =>
			{
				Assert.That(target.Arguments[0], Is.InstanceOf<ElementReference>());

				var sequence = (ImmutableArray<int>)target.Arguments[1]!;
				Assert.That(sequence, Is.EquivalentTo([5, 8, 4, 2, 1]));

				var labels = (string[])target.Arguments[2]!;
				Assert.That(labels, Is.EquivalentTo(["1", "2", "3", "4", "5"]));

				return true;
			}))
			.SetVoidResult();

			var charting = context.Render<Charting>();
			var chartingButton = charting.Find("#start");
			var chartingInput = charting.Find("input");
			var chartingCurrentInput = charting.Find("#currentSequence");

			charting.Instance.Start = "5";
			await chartingButton.ClickAsync(new MouseEventArgs());
			Assert.That(chartingCurrentInput.TextContent,
				Does.Contain("5, 8, 4, 2, 1"));
		}
	}

	[Test]
	public async Task CreateSequenceWithMockAsync()
	{
		using (Assert.EnterMultipleScope())
		{
			using var mockContext = new RockContext();
			var collatzExpectations = mockContext.Create<ICollatzCreateExpectations>();
			collatzExpectations.Setups.Generate<int>(5)
				.ReturnValue([5, 8, 4, 2, 1]);

			var objectReferenceExpectations = mockContext.Create<IJSObjectReferenceCreateExpectations>();
			objectReferenceExpectations.Setups.InvokeAsync<IJSVoidResult>(
				Constants.ChartingMethod,
				Arg.Validate<object?[]?>(values =>
				{
					Assert.That(values![0], Is.InstanceOf<ElementReference>());

					var sequence = (ImmutableArray<int>)values[1]!;
					Assert.That(sequence, Is.EquivalentTo([5, 8, 4, 2, 1]));

					var labels = (string[])values[2]!;
					Assert.That(labels, Is.EquivalentTo(["1", "2", "3", "4", "5"]));

					return true;
				}))
				.ReturnValue(ValueTask.FromResult(new IJSVoidResultMakeExpectations().Instance()));

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
			Assert.That(chartingCurrentInput.TextContent,
				Does.Contain("5, 8, 4, 2, 1"));
		}
	}

	[Test]
	public async Task CreateRandomStartValueWithMockAsync()
	{
		using (Assert.EnterMultipleScope())
		{
			using var response = new HttpResponseMessage
			{
				StatusCode = HttpStatusCode.OK,
				Content = new StringContent("5")
			};

			using var mockContext = new RockContext();
			var messageHandlerExpectations = mockContext.Create<HttpMessageHandlerCreateExpectations>();
			messageHandlerExpectations.Setups.SendAsync(
				Arg.Validate<HttpRequestMessage>(_ =>
				{
					Assert.That(_.RequestUri,
						Is.EqualTo(new Uri("http://localhost:5128/random")));
					return true;
				}), Arg.Any<CancellationToken>())
				.ReturnValue(Task.FromResult(response));

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
				Constants.ChartingMethod,
				Arg.Validate<object?[]?>(values =>
				{
					Assert.That(values![0], Is.InstanceOf<ElementReference>());

					var sequence = (ImmutableArray<int>)values[1]!;
					Assert.That(sequence, Is.EquivalentTo([5, 8, 4, 2, 1]));

					var labels = (string[])values[2]!;
					Assert.That(labels, Is.EquivalentTo(["1", "2", "3", "4", "5"]));

					return true;
				}))
				.ReturnValue(ValueTask.FromResult(new IJSVoidResultMakeExpectations().Instance()));

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
			Assert.That(charting.Instance.Start, Is.EqualTo("5"));
			Assert.That(chartingCurrentInput.TextContent,
				Does.Contain("5, 8, 4, 2, 1"));
		}
	}
}