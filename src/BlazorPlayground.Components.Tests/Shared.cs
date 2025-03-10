global using BUnitTestContext = Bunit.TestContext;
using BlazorPlayground.Components;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using NUnit.Framework;
using Rocks;

[assembly: Parallelizable(ParallelScope.Children)]
[assembly: Rock(typeof(HttpMessageHandler), BuildType.Create)]
[assembly: Rock(typeof(ICollatz), BuildType.Create)]
[assembly: Rock(typeof(IHttpClientFactory), BuildType.Create | BuildType.Make)]
[assembly: Rock(typeof(IJSObjectReference), BuildType.Create)]
[assembly: Rock(typeof(IJSRuntime), BuildType.Create)]
[assembly: Rock(typeof(IJSVoidResult), BuildType.Make)]