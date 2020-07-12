using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RegexNodes.Shared.Nodes;
using RegexNodes.Shared.NodeInputs;

namespace RegexNodes.Shared.NodeTypes
{
    public class UnicodeNode : Node
    {
        public override string Title => "Unicode";

        public override string NodeInfo => "Insert a unicode category, unicode block, or the hex value of a unicode/ascii character.";

        [NodeInput]
        public InputDropdown<Modes> InputMode { get; } = new InputDropdown<Modes>(modeDisplayNames) { Title = "Mode" };
        [NodeInput]
        public InputString InputCategory { get; } = new InputString("IsBasicLatin") { Title = "Unicode Category" };
        [NodeInput]
        public InputString InputHexCode { get; } = new InputString("1e22") { Title = "Hex Code" };
        [NodeInput]
        public InputCheckbox InputInvert { get; } = new InputCheckbox() { Title = "Invert" };

        public enum Modes
        {
            Category,
            Hex
        }

        private static readonly Dictionary<Modes, string> modeDisplayNames = new Dictionary<Modes, string>()
        {
            {Modes.Category, "Category/Block"},
            {Modes.Hex, "Hex Code"},
        };

        public UnicodeNode()
        {
            InputCategory.IsEnabled = () => InputMode.Value == Modes.Category;
            InputHexCode.IsEnabled = () => InputMode.Value == Modes.Hex;
        }

        protected override NodeResultBuilder GetValue()
        {
            return new NodeResultBuilder(ValueString(), this);
        }

        private string ValueString()
        {
            return InputMode.Value switch
            {
                Modes.Category => GetCategoryRegex(InputCategory.Contents, InputInvert.IsChecked),
                Modes.Hex => GetHexCodeRegex(InputHexCode.Contents, InputInvert.IsChecked),
                _ => "",
            };
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
