using System.Collections.Generic;

namespace RegexNodes.Shared.NodeTypes
{
    public class OutputNode : Node
    {
        public override string Title => "Output";
        public override string NodeInfo => "The final output of your Regex. Use the 'Add Item' button to join together the outputs of multiple nodes, similar to the 'Concatenate' node.";

        [NodeInput]
        protected InputCollection Inputs { get; } = new InputCollection(1);

        [NodeInput]
        protected InputDropdown InputStartsAt { get; } = new InputDropdown("Anywhere", "Start of line", "Word boundary") { Title="Starts at:"};

        [NodeInput]
        protected InputDropdown InputEndsAt { get; } = new InputDropdown("Anywhere", "End of line", "Word boundary") { Title = "Ends at:" };

        public override string GetValue()
        {
            string result = "";
            if(InputStartsAt.DropdownValue == "Start of line")
            {
                result += "^";
            }
            else if(InputStartsAt.DropdownValue == "Word boundary")
            {
                result += "\\b";
            }

            foreach (var input in Inputs.Inputs)
            {
                result += input.GetValue();
            }

            if (InputEndsAt.DropdownValue == "End of line")
            {
                result += "$";
            }
            else if (InputEndsAt.DropdownValue == "Word boundary")
            {
                result += "\\b";
            }
            CachedValue = result;
            return result;
        }
    }
}
