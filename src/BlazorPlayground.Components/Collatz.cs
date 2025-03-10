using Collatz;
using System.Collections.Immutable;
using System.Numerics;

namespace BlazorPlayground.Components;

internal sealed class Collatz
	: ICollatz
{
	public ImmutableArray<T> Generate<T>(T start) where T : IBinaryInteger<T> => 
		CollatzSequenceGenerator.Generate(start);
}