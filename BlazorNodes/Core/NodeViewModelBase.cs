namespace BlazorNodes.Core;
using System.Reflection;

public interface INodeOutput
{
    Vector2 OutputPos { get; }
    string CssName { get; }
    string CssColor { get; }
    event Action OutputChanged;
    event Action OutputPosChanged;
}

public interface INodeOutput<TOutput> : INodeOutput
{
    TOutput CachedOutput { get; }
}

public interface INodeViewModel : INodeOutput
{
    string NodeInfo { get; }
    string Title { get; }
    bool IsCollapsed { get; set; }
    Vector2 Pos { get; set; }
    bool Selected { get; set; }

    IEnumerable<INodeInput> NodeInputs { get; }

    string OutputTooltip { get; }
    IInputPort PrimaryInput { get; }

    void CalculateInputsPos();

    void OnLayoutChanged();
    IEnumerable<INodeInput> GetAllInputs();

    event Action LayoutChanged;
    event Action SelectionChanged;
}

public abstract class NodeViewModelBase : INodeViewModel
{
    private Vector2 pos;

    public Vector2 Pos
    {
        get => pos;
        set
        {
            pos = value;
            OutputPosChanged?.Invoke();
            OnLayoutChanged();
        }
    }

    public IEnumerable<INodeInput> NodeInputs { get; }
    public abstract IInputPort PrimaryInput { get; }
    public bool IsCollapsed { get; set; }

    private bool selected;
    public bool Selected
    {
        get => selected;
        set
        {
            if (value == selected) return;
            selected = value;
            SelectionChanged?.Invoke();
        }
    }
    public abstract string Title { get; }
    public abstract string OutputTooltip { get; }

    public abstract Vector2 OutputPos { get; }

    public event Action? LayoutChanged;
    public event Action? SelectionChanged;
    public event Action? OutputPosChanged;
    public abstract event Action OutputChanged;

    public void OnLayoutChanged()
    {
        CalculateInputsPos();
        foreach (var input in GetAllInputs().OfType<IInputPort>())
            input.Refresh();
        LayoutChanged?.Invoke();
    }

    protected NodeViewModelBase()
    {
        var inputProperties = GetType()
            .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
            .Where(prop => Attribute.IsDefined(prop, typeof(NodeInputAttribute)));

        NodeInputs = inputProperties
            .Select(prop => prop.GetValue(this))
            .OfType<INodeInput>()
            .ToList();
    }

    public abstract IEnumerable<INodeInput> GetAllInputs();

    /// <summary>
    /// Set the position of each input based on the position of the node.
    /// </summary>
    public abstract void CalculateInputsPos();

    public abstract string CssName { get; }
    public abstract string CssColor { get; }

    public abstract string NodeInfo { get; }
}

public abstract class NodeViewModelBase<TOutput> : NodeViewModelBase, INodeOutput<TOutput>
{
    public abstract TOutput CachedOutput { get; }
}
