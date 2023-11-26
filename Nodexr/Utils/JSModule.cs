namespace Nodexr.Utils;

using Microsoft.JSInterop;

public sealed class JSModule(Task<IJSObjectReference> objectTask) : IJSObjectReference
{
    public async ValueTask DisposeAsync()
    {
        var jsObject = await objectTask.ConfigureAwait(false);
        await jsObject.DisposeAsync().ConfigureAwait(false);
        GC.SuppressFinalize(this);
    }

    public async ValueTask<TValue> InvokeAsync<TValue>(string identifier, object?[]? args)
    {
        return await
            (await objectTask.ConfigureAwait(false))
            .InvokeAsync<TValue>(identifier, args).ConfigureAwait(false);
    }

    public async ValueTask<TValue> InvokeAsync<TValue>(string identifier, CancellationToken cancellationToken, object?[]? args)
    {
        return await
            (await objectTask.ConfigureAwait(false))
            .InvokeAsync<TValue>(identifier, cancellationToken, args).ConfigureAwait(false);
    }
}
