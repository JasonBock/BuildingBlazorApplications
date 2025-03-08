using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Http;
using System.Diagnostics;

namespace BlazorPlayground.Components.Pages;

public partial class Error
{
	[CascadingParameter]
	private HttpContext? HttpContext { get; set; }

	private string? RequestId { get; set; }
	private bool ShowRequestId => !string.IsNullOrEmpty(this.RequestId);

	protected override void OnInitialized() =>
		this.RequestId = Activity.Current?.Id ?? this.HttpContext?.TraceIdentifier;
}
