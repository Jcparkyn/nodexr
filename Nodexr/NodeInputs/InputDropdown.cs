namespace Nodexr.NodeInputs;
using BlazorNodes.Core;
using System;
using System.Collections.Generic;
using System.Linq;

public abstract class InputDropdown : NodeInputBase
{
    public abstract string ValueDisplayName { get; set; }

    public abstract IEnumerable<string> Options { get; }
}

public class InputDropdown<TValue> : InputDropdown
    where TValue : struct, Enum
{
    private readonly Dictionary<TValue, string>? displayNames;

    private TValue value = default;

    public override int Height => 50;

    public TValue Value
    {
        get => value;
        set
        {
            this.value = value;
            OnValueChanged();
        }
    }

    public override string ValueDisplayName
    {
        get
        {
            return displayNames?.GetValueOrDefault(Value) ?? Value.ToString();
        }

        set
        {
            if (displayNames != null)
                Value = displayNames.FirstOrDefault(x => x.Value == value).Key;
            else
                Value = Enum.Parse<TValue>(value);
        }
    }

    public override IEnumerable<string> Options
    {
        get
        {
            if (displayNames != null) return displayNames.Values;
            else return Enum.GetNames(typeof(TValue));
        }
    }

    public InputDropdown(Dictionary<TValue, string> displayNames)
    {
        this.displayNames = displayNames;
        Value = displayNames.Keys.FirstOrDefault();
    }

    public InputDropdown()
    {
    }
}
