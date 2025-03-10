using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Globalization;

namespace BlazorPlayground.Components.Pages;

public sealed partial class Charting
{
	private readonly IJSRuntime jsRuntime;
	private readonly ICollatz collatz;

	public Charting(IJSRuntime jsRuntime, ICollatz collatz) =>
		(this.jsRuntime, this.collatz) = (jsRuntime, collatz);

	private ElementReference ChartReference { get; set; }
	public string CurrentSequence { get; set; } = string.Empty;
	public string? Start { get; set; }

	private async Task CreateSequenceAsync()
	{
		if (int.TryParse(this.Start, out var value))
		{
			try
			{
				var sequence = this.collatz.Generate(value);
				this.CurrentSequence = string.Join(", ", sequence);
				var labels = sequence.Length > 0 ?
					[.. Enumerable.Range(1, sequence.Length).Select(_ => _.ToString(CultureInfo.CurrentCulture))] :
					Array.Empty<string>();

				var module = await this.jsRuntime.InvokeAsync<IJSObjectReference>(
					Constants.Import, Constants.ChartingFileLocation);
				await module.InvokeVoidAsync(Constants.ChartingMethod,
					this.ChartReference,
					sequence, labels);
			}
			catch (OverflowException)
			{
				this.CurrentSequence = $"The value, {value}, is incorrect.";
			}
		}
		else
		{
			this.CurrentSequence = $"{this.Start} is not a valid integer.";
		}

		this.StateHasChanged();
	}
}
