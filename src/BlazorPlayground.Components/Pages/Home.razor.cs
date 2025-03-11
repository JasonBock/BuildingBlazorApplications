namespace BlazorPlayground.Components.Pages;

public partial class Home
{
	private readonly IIdentifier identifier;

	public Home(IIdentifier identifier) =>
		this.identifier = identifier;
}