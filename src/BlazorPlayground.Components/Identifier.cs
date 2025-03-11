using Microsoft.Extensions.Logging;

namespace BlazorPlayground.Components;

internal sealed class Identifier
	: IIdentifier, IDisposable
{
	private readonly ILogger<Identifier> logger;

	public Identifier(ILogger<Identifier> logger) => 
		(this.logger, this.Id) = (logger, Guid.NewGuid());
	
	public Guid Id { get; private set; }

   public void Dispose() => 
		this.logger.LogInformation("Identifier with Id {Id} disposed.", this.Id);
}