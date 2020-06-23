using System.Collections.Generic;

namespace RegexNodes.Shared.NodeTypes
{
    public class OutputNode : Node
    {
        public override string Title => "Output";
        public override string NodeInfo => "The final output of your Regex." +
            "Use the \"Starts at\" and \"Ends at\" options to only include matches in a certain position" +
            "(This is equivalent to using the Anchor node).";

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
            if (PreviousNode is null)
            {
                return "Nothing connected to Output node";
            }

            string contents = Previous.ConnectedNode.CachedOutput;
            //Remove the uneccessary group from an OrNode if it is the final node
            if(CanStripNonCapturingGroup())
            {
                contents = contents[3..^1];
            }

            //Prefix
            string result = InputStartsAt.DropdownValue switch
            {
                Modes.startLine => "^",
                Modes.wordBound => "\\b",
                _ => ""
            };

            result += contents;

            //Suffix
            result += InputEndsAt.DropdownValue switch
            {
                Modes.endLine => "$",
                Modes.wordBound => "\\b",
                _ => ""
            };

            return result;
        }

        private bool CanStripNonCapturingGroup()
        {
            return PreviousNode is OrNode _node
                && _node.PreviousNode is null
                && InputStartsAt.DropdownValue == Modes.anywhere
                && InputEndsAt.DropdownValue == Modes.anywhere;
        }
    }
}
