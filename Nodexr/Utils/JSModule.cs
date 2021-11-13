namespace Nodexr.Utils;

using Microsoft.JSInterop;
using System;
using System.Threading;
using System.Threading.Tasks;

public sealed class JSModule : IJSObjectReference
{
    private readonly Task<IJSObjectReference> _objectTask;

    public JSModule(Task<IJSObjectReference> objectTask)
    {
        _objectTask = objectTask;
    }

    public async ValueTask DisposeAsync()
    {
        var jsObject = await _objectTask.ConfigureAwait(false);
        await jsObject.DisposeAsync().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    public async ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
    {
        return await
            (await _objectTask.ConfigureAwait(false))
            .InvokeAsync<TValue>(identifier, args).ConfigureAwait(false);
    }

    public async ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
    {
        return await
            (await _objectTask.ConfigureAwait(false))
            .InvokeAsync<TValue>(identifier, cancellationToken, args).ConfigureAwait(false);
    }
}
