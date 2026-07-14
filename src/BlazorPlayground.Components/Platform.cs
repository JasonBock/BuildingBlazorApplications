using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Rendering;
using System.Runtime.InteropServices;

namespace BlazorPlayground.Components;

public sealed class Platform
	: ComponentBase
{
	protected override void BuildRenderTree(RenderTreeBuilder builder)
	{
		ArgumentNullException.ThrowIfNull(builder);
		builder.OpenElement(0, "span");
		builder.AddContent(1, RuntimeInformation.OSDescription);
		builder.CloseElement();
	}
}