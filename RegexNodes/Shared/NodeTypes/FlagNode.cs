using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegexNodes.Shared.NodeTypes
{
    public class FlagNode : Node
    {
        public override string Title => "Flags";

        public override List<INodeInput> NodeInputs
        {
            get
            {
                return new List<INodeInput> {
                    InputContents, OptionDisable, OptionIgnoreCase, OptionMultiline, OptionSingleline, OptionExplicitCapture, OptionIgnoreWhitespace
                };
            }
        }

        protected InputProcedural InputContents { get; set; } = new InputProcedural() { Title = "(Optional) Contents" };

        protected InputDropdown OptionDisable { get; set; } = new InputDropdown("Enable", "Disable") { Title = "Disable Flags" };
        protected InputCheckbox OptionIgnoreCase { get; set; } = new InputCheckbox(false) { Title = "Case Insensitive" };
        protected InputCheckbox OptionMultiline { get; set; } = new InputCheckbox(false) { Title = "Multiline" };
        protected InputCheckbox OptionSingleline { get; set; } = new InputCheckbox(false) { Title = "Singleline" };
        protected InputCheckbox OptionExplicitCapture { get; set; } = new InputCheckbox(false) { Title = "Explicit Capture" };
        protected InputCheckbox OptionIgnoreWhitespace { get; set; } = new InputCheckbox(false) { Title = "Ignore Whitespace" };

        public override string GetValue()
        {
            string input = "" + InputContents.GetValue().RemoveNonCapturingGroup();
            string prefix = "";
            prefix += OptionDisable.DropdownValue == "Disable" ? "-" : "";
            prefix += OptionIgnoreCase.IsChecked ? "i" : "";
            prefix += OptionMultiline.IsChecked ? "m" : "";
            prefix += OptionSingleline.IsChecked ? "s" : "";
            prefix += OptionExplicitCapture.IsChecked ? "n" : "";
            prefix += OptionIgnoreWhitespace.IsChecked ? "x" : "";

            if (String.IsNullOrEmpty(InputContents.InputNode.GetValue()))
            {
                prefix += ":";
            }
            
            //string prefix = (InputGroupType.Value == "Capturing") ? "(" : "(?:";
            //if (input.StartsWith("(?:") && input.EndsWith(")"))
            //{
            //    return UpdateCache(prefix + input.Substring(3));
            //}
            return UpdateCache($"(?{prefix}{input})");
        }
    }
}
