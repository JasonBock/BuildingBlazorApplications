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

namespace BlazorPlayground.Components.Tests.Pages;

public static class ChartingTests
{
	[Test]
	public static async Task CreateSequenceWithEmulatorAsync()
	{
		using var context = new BUnitTestContext();
		context.Services.AddPlaygroundConfiguration();
		context.JSInterop.Mode = JSRuntimeMode.Strict;

		var chartingInterop = context.JSInterop.SetupModule(Constants.ChartingFileLocation);
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
		var chartingButton = charting.Find("button");
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

		var charting = context.RenderComponent<Charting>();
		var chartingButton = charting.Find("button");
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
}