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

        public InputDropdown(params string[] options)
        {
            DropdownValue = options[0];
            Options = options.ToList();
        }
    }

    public class InputDropdown<TValue> : InputDropdown
        where TValue : Enum
    {
        //private string dropdownValue;
        private Dictionary<TValue, string> displayNames;

        public TValue Value { get; set; }

        public override string DropdownValue
        {
            get => displayNames[Value];
            set
            {
                Value = displayNames.FirstOrDefault(x => x.Value == value).Key;
                OnValueChanged();
            }
        }

        public override List<string> Options
        {
            get => displayNames.Values.ToList();
        }


        public InputDropdown(Dictionary<TValue, string> displayNames)
        {
            this.displayNames = displayNames;
            //DropdownValue = options[0];
            //Options = options.ToList();
        }
    }
}
