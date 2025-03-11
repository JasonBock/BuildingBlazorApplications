using Microsoft.Extensions.Logging;

namespace BlazorPlayground.Components.Pages;

public partial class Counter
{
	private int currentCount;
	private readonly IIdentifier identifier;
	private readonly ILogger<Counter> logger;

	public Counter(ILogger<Counter> logger, IIdentifier identifier) =>
		(this.logger, this.identifier) = (logger, identifier);

	private void IncrementCount()
	{
#pragma warning disable CA1848 // Use the LoggerMessage delegates
		this.logger.LogInformation("Incrementing count.");
#pragma warning restore CA1848 // Use the LoggerMessage delegates
		this.currentCount++;
	}
}