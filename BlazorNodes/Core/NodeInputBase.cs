namespace BlazorNodes.Core;

public interface INodeInput
{
    string? Title { get; set; }
    event EventHandler ValueChanged;
    Func<bool> Enabled { get; }
    string? Description { get; set; }
    Vector2 Pos { get; set; }
    int Height { get; }
}

public interface INodeInput<TValue>
{
    public TValue Value { get; }
}

public abstract class NodeInputBase : INodeInput
{
    public string? Title { get; set; }

    /// <summary>
    /// The description for this input. Displayed as a tooltip for most types of inputs.
    /// </summary>
    public string? Description { get; set; }

    public event EventHandler? ValueChanged;

    public Vector2 Pos { get; set; }

    public Func<bool> Enabled { get; set; } = () => true;

    public virtual int Height { get; } = 32;

    protected virtual void OnValueChanged(object? sender, EventArgs e)
    {
        ValueChanged?.Invoke(this, e);
    }

    protected virtual void OnValueChanged()
    {
        OnValueChanged(this, EventArgs.Empty);
    }
}

public abstract class NodeInputBase<TValue> : NodeInputBase, INodeInput<TValue>
{
    public abstract TValue Value { get; }
}
