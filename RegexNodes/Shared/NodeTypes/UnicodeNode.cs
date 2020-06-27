using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegexNodes.Shared.NodeTypes
{
    public class UnicodeNode : Node
    {
        public override string Title => "Unicode";

        public override string NodeInfo => "Insert a unicode category, unicode block, or the hex value of a unicode/ascii character.";

        [NodeInput]
        protected InputDropdown InputMode { get; } = new InputDropdown(Modes.category, Modes.hex) { Title = "Mode" };
        [NodeInput]
        protected InputString InputCategory { get; } = new InputString("IsBasicLatin") { Title = "Unicode Category" };
        [NodeInput]
        protected InputString InputHexCode { get; } = new InputString("1e22") { Title = "Hex Code" };
        [NodeInput]
        protected InputCheckbox InputInvert { get; } = new InputCheckbox() { Title = "Invert" };

        private static class Modes
        {
            public const string category = "Category/Block";
            public const string hex = "Hex Code";
        }

        public UnicodeNode()
        {
            InputCategory.IsEnabled = () => InputMode.DropdownValue == Modes.category;
            InputHexCode.IsEnabled = () => InputMode.DropdownValue == Modes.hex;
        }

        protected override string GetValue()
        {
            switch (InputMode.DropdownValue)
            {
                case Modes.category:
                    return GetCategoryRegex(InputCategory.InputContents, InputInvert.IsChecked);
                case Modes.hex:
                    return GetHexCodeRegex(InputHexCode.InputContents, InputInvert.IsChecked);
                default: return "";
            }
        }

        string GetCategoryRegex(string input, bool invert)
        {
            if (invert)
            {
                return @"\P{" + input + "}";
            }
            else
            {
                return @"\p{" + input + "}";
            }
        }

        string GetHexCodeRegex(string input, bool invert)
        {
            if(input.Length > 2)
            {
                input = "\\u" + input.PadLeft(4, '0');
                
            }
            else
            {
                input = "\\x" + input.PadLeft(2, '0');
            }

            if (invert)
                return "[^" + input + "]";
            else
                return input;
        }
    }
}
