using Microsoft.JSInterop;

namespace BlazorPlayground.Components.Pages;

public sealed partial class Location
{
	private readonly IJSRuntime jsRuntime;
	private DotNetObjectReference<Location>? reference;

	public Location(IJSRuntime jsRuntime) =>
		this.jsRuntime = jsRuntime;

	[JSInvokable]
	public void Change(double latitude, double longitude, double accuracy)
	{
		(this.Latitude, this.Longitude, this.Accuracy) = (latitude, longitude, accuracy);
		this.BingMainUrl = $"https://www.bing.com/maps/embed?h=400&w=500&cp={latitude}~{longitude}&lvl=11&typ=d&sty=r&src=SHELL&FORM=MBEDV8";
		this.BingLargeMapUrl = $"https://www.bing.com/maps?cp={latitude}~{longitude}&amp;sty=r&amp;lvl=11&amp;FORM=MBEDLD";
		this.BingDirectionsUrl = $"https://www.bing.com/maps/directions?cp={latitude}~-{longitude}&amp;sty=r&amp;lvl=11&amp;rtp=~pos.{latitude}_{longitude}____&amp;FORM=MBEDLD";
		this.StateHasChanged();
	}

	protected override async Task OnAfterRenderAsync(bool firstRender)
	{
		if (firstRender)
		{
			this.reference = DotNetObjectReference.Create(this);

			var module = await this.jsRuntime.InvokeAsync<IJSObjectReference>(
				Constants.Import, Constants.LocationFileLocation);
			await module.InvokeVoidAsync(
				Constants.LocationMethod, this.reference);
		}
	}

	public void Dispose()
	{
		GC.SuppressFinalize(this);
		this.reference?.Dispose();
	}

	public double Accuracy { get; set; }
	public string? BingMainUrl { get; set; }
	public string? BingLargeMapUrl { get; set; }
	public string? BingDirectionsUrl { get; set; }
	public double Latitude { get; set; }
	public double Longitude { get; set; }
}
