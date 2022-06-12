namespace Nodexr.NodeInputs;
using BlazorNodes.Core;

public readonly record struct IntRange(int? Min, int? Max);

public class InputRange : NodeInputBase<IntRange>
{
    // TODO replace Min/Max
    public override IntRange Value
    {
        get => new(Min, Max);
        set
        {
            Min = value.Min;
            Max = value.Max;
        }
    }

    private int? min;

    public int? Min
    {
        get => min;
        set
        {
            min = value;
            OnValueChanged();
        }
    }

    private int? max;

    public int? Max
    {
        get => max;
        set
        {
            if (AutoClearMax && value <= MinValue)
                max = null;
            else
                max = value;
            OnValueChanged();
        }
    }

    public int? MinValue { get; set; }

    /// <summary>
    /// Automatically clear the 'Max' field if the user set the value below MinValue.
    /// </summary>
    public bool AutoClearMax { get; set; }

    public override int Height => 50;

    public InputRange(int? min = null, int? max = null)
    {
        Min = min;
        Max = max;
    }
}
