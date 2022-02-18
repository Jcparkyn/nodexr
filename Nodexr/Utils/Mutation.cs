namespace Nodexr.Utils;

public class Mutation<TArg, TResult>
{
    public TResult? Data { get; private set; }

    public Exception? Error { get; private set; }

    public bool IsLoading { get; private set; }

    private readonly Func<TArg, Task<TResult>> action;
    private readonly Action<Exception>? onError;
    private readonly Action? onStateChanged;

    public Mutation(
        Action? onStateChanged,
        Func<TArg, Task<TResult>> action,
        Action<Exception>? onError = null)
    {
        this.action = action;
        this.onError = onError;
        this.onStateChanged = onStateChanged;
    }

    public async Task<TResult?> Trigger(TArg arg)
    {
        IsLoading = true;
        onStateChanged?.Invoke();
        var x = Task.FromResult(0);
        try
        {
            Data = await action(arg);
            return Data;
        }
        catch (Exception ex)
        {
            Error = ex;
            onError?.Invoke(ex);
            return default;
        }
        finally
        {
            IsLoading = false;
            onStateChanged?.Invoke();
        }
    }
}
