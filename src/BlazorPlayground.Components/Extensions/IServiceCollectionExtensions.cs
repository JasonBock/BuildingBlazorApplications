using Microsoft.Extensions.DependencyInjection;

namespace BlazorPlayground.Components.Extensions;

public static class IServiceCollectionExtensions
{
#pragma warning disable CA1034 // Nested types should not be visible
	extension(IServiceCollection self)
#pragma warning restore CA1034 // Nested types should not be visible
	{
		public IServiceCollection AddPlaygroundConfiguration() =>
			self.AddSingleton<ICollatz, Collatz>()
				.AddTransient<IIdentifier, Identifier>()
				.AddHttpClient();
	}
}