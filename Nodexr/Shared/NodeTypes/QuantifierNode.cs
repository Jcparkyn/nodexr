using System;
using System.Collections.Generic;
using Nodexr.Shared;
using Nodexr.Shared.Nodes;
using Nodexr.Shared.NodeInputs;
using static Nodexr.Shared.NodeTypes.IQuantifiableNode;

namespace Nodexr.Shared.NodeTypes
{
    public class QuantifierNode : Node, IQuantifiableNode
    {
        public override string Title => "Quantifier";
        public override string NodeInfo => "Inserts a quantifier to set the minimum and maximum number " +
            "of 'repeats' for the inputted node. Leave the 'max' option blank to allow unlimited repeats." +
            "\n'Greedy' and 'Lazy' search type will attempt to match as many or as few times as possible respectively." +
            "\nThe .NET Regex engine does not support possessive quantifiers, so they are automatically replaced " +
            "by atomic groups (which are functionally identical).";

        [NodeInput]
        public InputProcedural InputContents { get; } = new InputProcedural() { Title = "Input" };
        [NodeInput]
        public InputDropdown<Reps> InputCount { get; } = new InputDropdown<Reps>(displayNamesExcludingOne)
        { Title = "Repetitions:" };
        [NodeInput]
        public InputNumber InputNumber { get; } = new InputNumber(0, min: 0) { Title = "Amount:" };
        [NodeInput]
        public InputNumber InputMin { get; } = new InputNumber(0, min: 0) { Title = "Minimum:" };
        [NodeInput]
        public InputNumber InputMax { get; } = new InputNumber(1, min: 0) { Title = "Maximum:" };
        [NodeInput]
        public InputDropdown<SearchMode> InputSearchType { get; } = new InputDropdown<SearchMode>()
        { Title = "Search type:" };

        public enum SearchMode
        {
            Greedy,
            Lazy,
            Possessive,
        }

        static readonly Dictionary<Reps, string> displayNamesExcludingOne = new Dictionary<Reps, string>()
        {
            {Reps.ZeroOrMore, "Zero or more"},
            {Reps.OneOrMore, "One or more"},
            {Reps.ZeroOrOne, "Zero or one"},
            {Reps.Number, "Number"},
            {Reps.Range, "Range"}
        };

        public QuantifierNode()
        {
            InputNumber.IsEnabled = () => InputCount.Value == Reps.Number;
            InputMin.IsEnabled = () => InputCount.Value == Reps.Range;
            InputMax.IsEnabled = () => InputCount.Value == Reps.Range;
        }

        protected override NodeResultBuilder GetValue()
        {
            var builder = new NodeResultBuilder(InputContents.Value);

            string suffix = GetSuffix(
                InputCount.Value,
                InputNumber.InputContents,
                InputMin.GetValue(),
                InputMax.GetValue());

            if (InputSearchType.Value == SearchMode.Lazy)
            {
                suffix += "?";
            }
            else if (InputSearchType.Value == SearchMode.Possessive)
            {
                suffix += ")";
                builder.Prepend("(?>", this);
            }

            //TODO: remove uneccessary groups
            //string contents = InputContents.GetValue();
            if (InputContents.ConnectedNode is Node _node
                && RequiresGroupToQuantify(_node))
            {
                //contents = contents.InNonCapturingGroup();
                builder.Prepend("(?:", this);
                builder.Append(")", this);
            }

            builder.Append(suffix, this);
            return builder;
        }

        private bool RequiresGroupToQuantify(Node val)
        {
            if (val is null) throw new ArgumentNullException(nameof(val));
            
            //Any chain of 2 or more nodes will always need to be wrapped in a group to quantify properly.
            if (!(val.PreviousNode is null))
                return true;

            //All Concat, and Quantifier nodes also need to be wrapped in a group to quantify properly.
            if (val is ConcatNode || val is QuantifierNode)
                return true;

            if (val is TextNode && !val.CachedOutput.Expression.IsSingleRegexChar())
                return true;

            return false;
        }
    }
}
