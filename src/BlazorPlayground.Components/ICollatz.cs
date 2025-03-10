using System.Collections.Immutable;
using System.Numerics;

namespace BlazorPlayground.Components;

public interface ICollatz
{
	ImmutableArray<T> Generate<T>(T start)
		where T : IBinaryInteger<T>;
}