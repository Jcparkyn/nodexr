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
        public InputDropdown<Modes> InputStartsAt { get; } = new InputDropdown<Modes>(
            new Dictionary<Modes, string>()
            {
                {Modes.anywhere, "Anywhere"},
                {Modes.startLine, "Start of line"},
                {Modes.wordBound, "Word boundary"},
            })
        { Title="Starts at:"};

        [NodeInput]
        public InputDropdown<Modes> InputEndsAt { get; } = new InputDropdown<Modes>(
            new Dictionary<Modes, string>()
            {
                {Modes.anywhere, "Anywhere"},
                {Modes.endLine, "End of line"},
                {Modes.wordBound, "Word boundary"},
            })
        { Title = "Ends at:" };

        public enum Modes
        {
            anywhere,
            startLine,
            endLine,
            wordBound,
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
            string result = InputStartsAt.Value switch
            {
                Modes.startLine => "^",
                Modes.wordBound => "\\b",
                _ => ""
            };

            result += contents;

            //Suffix
            result += InputEndsAt.Value switch
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
                && InputStartsAt.Value == Modes.anywhere
                && InputEndsAt.Value == Modes.anywhere;
        }
    }
}
