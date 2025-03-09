global using BUnitTestContext = Bunit.TestContext;
using Microsoft.JSInterop;
using NUnit.Framework;
using Rocks;

[assembly: Parallelizable(ParallelScope.Children)]
[assembly: Rock(typeof(IJSObjectReference), BuildType.Create)]
[assembly: Rock(typeof(IJSRuntime), BuildType.Create)]
