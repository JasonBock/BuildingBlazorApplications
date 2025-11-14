using BlazorPlayground.Components;
using Microsoft.Extensions.Logging;
using Microsoft.JSInterop;
using Microsoft.JSInterop.Infrastructure;
using Rocks;

[assembly: Rock(typeof(HttpMessageHandler), BuildType.Create)]
[assembly: Rock(typeof(ICollatz), BuildType.Create)]
[assembly: Rock(typeof(IHttpClientFactory), BuildType.Create | BuildType.Make)]
[assembly: Rock(typeof(IIdentifier), BuildType.Create)]
[assembly: Rock(typeof(IJSObjectReference), BuildType.Create)]
[assembly: Rock(typeof(IJSRuntime), BuildType.Create)]
[assembly: Rock(typeof(IJSVoidResult), BuildType.Make)]
[assembly: Rock(typeof(ILogger<>), BuildType.Make)]