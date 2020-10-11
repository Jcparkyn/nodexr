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
        public InputProcedural InputContents { get; } = new InputProcedural()
        {
            Title = "Input",
            Description = "The node or set of nodes that will be matched the chosen number of times.",
        };

        [NodeInput]
        public InputDropdown<Reps> InputCount { get; } = new InputDropdown<Reps>(displayNamesExcludingOne)
        {
            Title = "Repetitions:",
            Description = "The number of times to match the input.",
        };

        [NodeInput]
        public InputNumber InputNumber { get; } = new InputNumber(0, min: 0) { Title = "Amount:" };

        [NodeInput]
        public InputNumber InputMin { get; } = new InputNumber(0, min: 0) { Title = "Minimum:" };

        [NodeInput]
        public InputNumber InputMax { get; } = new InputNumber(1, min: 0) { Title = "Maximum:" };

        [NodeInput]
        public InputDropdown<SearchMode> InputSearchType { get; } = new InputDropdown<SearchMode>()
        {
            Title = "Search type:",
            Description = "Changes the way that the Regex engine tries to match the repetition."
        };

        public enum SearchMode
        {
            Greedy,
            Lazy,
            Possessive,
        }

        private static readonly Dictionary<Reps, string> displayNamesExcludingOne = new Dictionary<Reps, string>()
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
            InputSearchType.IsEnabled = () => InputCount.Value != Reps.Number;
        }

        protected override NodeResultBuilder GetValue()
        {
            var builder = new NodeResultBuilder(InputContents.Value);

            string suffix = "";
            string prefix = "";

            //Surround with non-capturing group if necessary
            if (InputContents.ConnectedNode is Node _node
                && RequiresGroupToQuantify(_node))
            {
                prefix += "(?:";
                suffix += ")";
            }

            //Add quantifier
            suffix += GetSuffix(
                InputCount.Value,
                InputNumber.InputContents,
                InputMin.GetValue(),
                InputMax.GetValue());

            //Add modifier
            if (InputCount.Value != Reps.Number)
            {
                if (InputSearchType.Value == SearchMode.Lazy)
                {
                    suffix += "?";
                }
                else if (InputSearchType.Value == SearchMode.Possessive)
                {
                    suffix += ")";
                    prefix = "(?>" + prefix;
                }
            }

            builder.Prepend(prefix, this);
            builder.Append(suffix, this);
            return builder;
        }

        /// <summary>
        /// Check whether the given node needs a non-capturing group before it can be quantified.
        /// </summary>
        internal static bool RequiresGroupToQuantify(Node val)
        {
            if (val is null) throw new ArgumentNullException(nameof(val));

            //Any chain of 2 or more nodes will always need to be wrapped in a group to quantify properly.
            if (!(val.PreviousNode is null))
                return true;

            //All Concat, Quantifier, Decimal and Optional nodes also need to be wrapped in a group to quantify properly.
            if (val is ConcatNode
                || val is QuantifierNode
                || val is DecimalNode
                || val is OptionalNode)
            {
                return true;
            }

            if (val is TextNode && !val.CachedOutput.Expression.IsSingleRegexChar())
                return true;

            return false;
        }
    }
}
