using System.Collections.Generic;
using System.Linq;


namespace RegexNodes.Shared
{
    public class InputDropdown : NodeInput
    {
        private string dropdownValue;
        public string DropdownValue
        {
            get => dropdownValue;
            set
            {
                dropdownValue = value;
                OnValueChanged?.Invoke();
            }
        }

        public List<string> Options { get; set; }

        public InputDropdown(params string[] options)
        {
            DropdownValue = options[0];
            Options = options.ToList();
        }
    }
}
