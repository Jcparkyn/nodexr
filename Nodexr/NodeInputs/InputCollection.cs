namespace Nodexr.NodeInputs;
using BlazorNodes.Core;
using Nodexr.Nodes;

public class InputCollection : NodeInputBase<IReadOnlyCollection<INodeViewModel>>
{
    // TODO replace Inputs and refactor
    public override IReadOnlyCollection<INodeViewModel> Value
    {
        get => inputs.Select(i => i.ConnectedNode).OfType<INodeViewModel>().ToList();
        set => inputs = new(value.Select(node => new InputProcedural()
        {
            ConnectedNode = node as INodeOutput<NodeResult>,
        }));
    }

    private List<InputProcedural> inputs;
    public IReadOnlyCollection<InputProcedural> Inputs => inputs;

    public InputCollection(string title, int numInputs = 0)
    {
        Title = title;
        inputs = new List<InputProcedural>(numInputs);
        for (int i = 0; i < numInputs; i++)
        {
            AddItem();
        }
    }

    public event EventHandler? InputPositionsChanged;

    public override int Height => inputs.Select(input => input.Height).Sum() + 28;

    private void OnInputPositionsChanged()
    {
        InputPositionsChanged?.Invoke(this, EventArgs.Empty);
    }

    public void AddItem(INodeOutput<NodeResult>? node = null)
    {
        var newInput = new InputProcedural() { Title = Title };
        newInput.ConnectedNode = node;
        newInput.ValueChanged += OnValueChanged;
        inputs.Add(newInput);
        OnInputPositionsChanged();
        OnValueChanged();
    }

    public void RemoveItem(InputProcedural item)
    {
        item.ValueChanged -= OnValueChanged;
        item.ConnectedNode = null;
        inputs.Remove(item);
        OnInputPositionsChanged();
        OnValueChanged();
    }

    public void RemoveAll()
    {
        foreach (var input in inputs)
        {
            input.ValueChanged -= OnValueChanged;
        }
        inputs.Clear();
        OnInputPositionsChanged();
        OnValueChanged();
    }

    public void MoveUp(InputProcedural input)
    {
        int index = inputs.IndexOf(input);
        if (index > 0)
        {
            inputs[index] = inputs[index - 1];
            inputs[index - 1] = input;
            OnInputPositionsChanged();
            OnValueChanged();
        }
    }

    public void MoveDown(InputProcedural input)
    {
        int index = inputs.IndexOf(input);
        if (index < inputs.Count - 1)
        {
            inputs[index] = inputs[index + 1];
            inputs[index + 1] = input;
            OnInputPositionsChanged();
            OnValueChanged();
        }
    }
}
