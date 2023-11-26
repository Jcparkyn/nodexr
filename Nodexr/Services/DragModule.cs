namespace Nodexr.Services;

using Microsoft.JSInterop;
using Nodexr.Utils;
using System.Threading.Tasks;

/// <summary>
/// C# wrapper for the DragModule.js module.
/// </summary>
public sealed class DragModule(IJSRuntime jsRuntime) : IAsyncDisposable
{
    private readonly JSModule module = jsRuntime.LoadJSModule("./js/GeneratedJS/DragModule.js");

    public ValueTask StartDrag<T>(DotNetObjectReference<T> callbackObject, string methodName)
        where T : class
    {
        return module.InvokeVoidAsync("startDrag", callbackObject, methodName);
    }

    public ValueTask DisposeAsync()
    {
        return module.DisposeAsync();
    }
}
