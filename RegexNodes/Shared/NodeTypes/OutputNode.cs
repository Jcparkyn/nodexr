using System.Collections.Generic;

namespace RegexNodes.Shared.NodeTypes
{
    public class OutputNode : Node
    {
        public override string Title => "Output";
        public override string NodeInfo => "The final output of your Regex. Use the 'Add Item' button to join together the outputs of multiple nodes, similar to the 'Concatenate' node.";

        [NodeInput]
        protected InputDropdown InputStartsAt { get; } = new InputDropdown("Anywhere", "Start of line", "Word boundary") { Title="Starts at:"};

        [NodeInput]
        protected InputDropdown InputEndsAt { get; } = new InputDropdown("Anywhere", "End of line", "Word boundary") { Title = "Ends at:" };

        public override string GetOutput()
        {
            return GetValue();
        }

        protected override string GetValue()
        {
            //check whether nothing is connected to this node.
            if (PreviousNode.InputNode is null)
            {
                return UpdateCache("Nothing connected to Output node");
            }

            string result = "";
            if(InputStartsAt.DropdownValue == "Start of line")
            {
                result += "^";
            }
            else if(InputStartsAt.DropdownValue == "Word boundary")
            {
                result += "\\b";
            }

            result += PreviousNode.InputNode.GetOutput();

            if (InputEndsAt.DropdownValue == "End of line")
            {
                result += "$";
            }
            else if (InputEndsAt.DropdownValue == "Word boundary")
            {
                result += "\\b";
            }
            CachedValue = result;
            return UpdateCache(result);
        }
    }
}
