using System.Collections.Generic;

namespace RegexNodes.Shared.NodeTypes
{
    public class OutputNode : Node
    {
        public override string Title => "Output";
        public override string NodeInfo => "The final output of your Regex. " +
            "Use the \"Starts at\" and \"Ends at\" options to only include matches in a certain position" +
            "(This is equivalent to using the Anchor node).";

        [NodeInput]
        public InputDropdown<Mode> InputStartsAt { get; } = new InputDropdown<Mode>(
            new Dictionary<Mode, string>()
            {
                {Mode.Anywhere, "Anywhere"},
                {Mode.StartLine, "Start of line"},
                {Mode.WordBound, "Word boundary"},
            })
        { Title="Starts at:"};

        [NodeInput]
        public InputDropdown<Mode> InputEndsAt { get; } = new InputDropdown<Mode>(
            new Dictionary<Mode, string>()
            {
                {Mode.Anywhere, "Anywhere"},
                {Mode.EndLine, "End of line"},
                {Mode.WordBound, "Word boundary"},
            })
        { Title = "Ends at:" };

        public enum Mode
        {
            Anywhere,
            StartLine,
            EndLine,
            WordBound,
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
            switch (InputStartsAt.Value)
            {
                case Mode.StartLine: builder.Prepend("^", this); break;
                case Mode.WordBound: builder.Prepend("\\b", this); break;
            };
            //Suffix
            switch (InputEndsAt.Value)
            {
                case Mode.EndLine: builder.Append("$", this); break;
                case Mode.WordBound: builder.Append("\\b", this); break;
            };

            if (PreviousNode is OrNode)
                builder.StripNonCaptureGroup();
            return builder;
        }
    }
}
