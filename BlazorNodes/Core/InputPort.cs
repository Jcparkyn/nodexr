namespace BlazorNodes.Core;

public interface IInputPort : INodeInput, INoodleData
{
    /// <summary>
    /// Allows the ConnectedNode property to be accessed without the type argument.
    /// </summary>
    public INodeOutput? ConnectedNodeUntyped { get; }

    /// <summary>
    /// Attempt to set the connected node using a weakly-typed <see cref="INodeOutput"/>.
    /// </summary>
    /// <returns>Whether or not the connected node was set.</returns>
    public bool TrySetConnectedNode(INodeOutput? node);
}

public class InputPort<TValue> : NodeInputBase, IInputPort
    where TValue : class
{
    private INodeOutput<TValue>? connectedNode;

    public INodeOutput<TValue>? ConnectedNode
    {
        get => connectedNode;
        set
        {
            if (connectedNode != null)
            {
                connectedNode.OutputChanged -= OnValueChanged;
                connectedNode.OutputPosChanged -= Refresh;
            }

            if (value != null)
            {
                value.OutputChanged += OnValueChanged;
                value.OutputPosChanged += Refresh;
            }

            connectedNode = value;
            OnValueChanged();
        }
    }

    public Vector2 StartPos => connectedNode?.OutputPos ?? throw new InvalidOperationException();

    public Vector2 EndPos => Pos;

    public bool Connected => connectedNode != null && Enabled();

    public override int Height => 32;

    public event Action? NoodleChanged;

    public TValue? Value => ConnectedNode?.CachedOutput;

    /// <inheritdoc/>
    public INodeOutput? ConnectedNodeUntyped => ConnectedNode;

    /// <inheritdoc/>
    public bool TrySetConnectedNode(INodeOutput? node)
    {
        switch (node)
        {
            case INodeOutput<TValue> nodeSafe:
                ConnectedNode = nodeSafe;
                return true;
            case null:
                ConnectedNode = null;
                return true;
            default:
                return false;
        }
    }

    /// <summary>
    /// Causes the connected noodle to be re-rendered
    /// </summary>
    public void Refresh()
    {
        NoodleChanged?.Invoke();
    }

    public override bool TrySetValue(object? value)
    {
        // TODO Refactor or implement
        throw new NotImplementedException();
    }
}

public interface INoodleData
{
    Vector2 StartPos { get; }
    Vector2 EndPos { get; }
    bool Connected { get; }
    void Refresh();

    event Action NoodleChanged;
}
