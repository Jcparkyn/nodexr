using System;
using System.Collections.Generic;
using System.Linq;


namespace RegexNodes.Shared
{
    public class InputDropdown : NodeInput
    {
        private string dropdownValue;
        public virtual string DropdownValue
        {
            get => dropdownValue;
            set
            {
                dropdownValue = value;
                OnValueChanged();
            }
        }

        public virtual List<string> Options { get; private set; }

        public InputDropdown() { }

        public InputDropdown(params string[] options)
        {
            dropdownValue = options[0];
            OnValueChanged();
            Options = options.ToList();
        }
    }

    public class InputDropdown<TValue> : InputDropdown
        where TValue : Enum
    {
        //private string dropdownValue;
        private Dictionary<TValue, string> displayNames;

        public TValue Value { get; set; } = default;

        public override string DropdownValue
        {
            get
            {
                return displayNames?.GetValueOrDefault(Value) ?? Value.ToString();
            }

            set
            {
                Value = displayNames.FirstOrDefault(x => x.Value == value).Key;
                OnValueChanged();
            }
        }

        public override List<string> Options
        {
            get
            {
                if(displayNames != null) return displayNames.Values.ToList();

                else return Enum.GetNames(typeof(TValue)).ToList();
            }
        }


        public InputDropdown(Dictionary<TValue, string> displayNames)
        {
            this.displayNames = displayNames;
            //DropdownValue = options[0];
            //Options = options.ToList();
        }

        public InputDropdown()
        {
        }
    }
}
