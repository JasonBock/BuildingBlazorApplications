﻿using Microsoft.Extensions.DependencyInjection;

namespace BlazorPlayground.Components.Extensions;

public static class IServiceCollectionExtensions
{
   public static IServiceCollection AddPlaygroundConfiguration(
	   this IServiceCollection @self) => 
		self.AddSingleton<ICollatz, Collatz>()
			.AddSingleton<IIdentifier, Identifier>()
			.AddHttpClient();
}