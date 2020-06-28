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

        protected override NodeResult GetOutput()
        {
            var builder = GetValue();
            return builder.Build();
        }

        protected override NodeResultBuilder GetValue()
        {
            var builder = new NodeResultBuilder();
            //check whether nothing is connected to this node.
            if (PreviousNode is null)
            {
                builder.Append("Nothing connected to Output node", this);
                return builder;
            }
            builder.Append(Previous.Value);

            //Prefix
            string prefix = InputStartsAt.Value switch
            {
                Modes.startLine => "^",
                Modes.wordBound => "\\b",
                _ => ""
            };

            //Suffix
            string suffix = InputEndsAt.Value switch
            {
                Modes.endLine => "$",
                Modes.wordBound => "\\b",
                _ => ""
            };

            builder.Prepend(prefix, this);
            builder.Append(suffix, this);
            if (PreviousNode is OrNode)
                builder.StripNonCaptureGroup();
            return builder;
        }
    }
}
