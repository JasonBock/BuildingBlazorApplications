namespace BlazorPlayground.Components;

internal sealed class Identifier
	: IIdentifier
{
	public Identifier() => 
		this.Id = Guid.NewGuid();
	
	public Guid Id { get; private set; }
}