using System.Collections.Generic;

namespace RegexNodes.Shared.NodeTypes
{
    public class OutputNode : Node
    {
        public override string Title => "Output";
        public override string NodeInfo => "The final output of your Regex. Use the 'Add Item' button to join together the outputs of multiple nodes, similar to the 'Concatenate' node.";

        [NodeInput]
        protected InputDropdown InputStartsAt { get; } = new InputDropdown(Modes.anywhere, Modes.startLine, Modes.wordBound) { Title="Starts at:"};

        [NodeInput]
        protected InputDropdown InputEndsAt { get; } = new InputDropdown(Modes.anywhere, Modes.endLine, Modes.wordBound) { Title = "Ends at:" };

        private class Modes
        {
            public const string anywhere = "Anywhere";
            public const string startLine = "Start of line";
            public const string endLine = "End of line";
            public const string wordBound= "Word boundary";
        }

        public override string GetOutput()
        {
            return GetValue();
        }

        protected override string GetValue()
        {
            //check whether nothing is connected to this node.
            if (PreviousNode.ConnectedNode is null)
            {
                return "Nothing connected to Output node";
            }

            //Prefix
            string result = InputStartsAt.DropdownValue switch
            {
                Modes.startLine => "^",
                Modes.wordBound => "\\b",
                _ => ""
            };

            result += PreviousNode.ConnectedNode.GetOutput();

            //Suffix
            result += InputEndsAt.DropdownValue switch
            {
                Modes.endLine => "$",
                Modes.wordBound => "\\b",
                _ => ""
            };

            return result;
        }
    }
}
