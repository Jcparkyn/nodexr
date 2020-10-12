using System;
using System.Collections.Generic;
using Nodexr.Shared;
using Nodexr.Shared.Nodes;
using Nodexr.Shared.NodeInputs;
using static Nodexr.Shared.NodeTypes.IQuantifiableNode;

namespace Nodexr.Shared.NodeTypes
{
    public class ListNode : Node
    {
        public override string Title => "List";

        public override string NodeInfo => "Matches a list of similar items, with a separator between each one." +
            "\n\nWarning: this node is marked as 'Experimental' because it will not be preserved " +
            "after using the 'Create Link' or 'Edit' buttons.";

        [NodeInput]
        public InputProcedural InputContents { get; } = new InputProcedural()
        {
            Title = "List Item",
            Description = "The node or set of nodes to match multiple times.",
        };

        [NodeInput]
        public InputString InputSeparator { get; } = new InputString(",")
        {
            Title = "Separator:",
            Description = "The separator character or string to match between each list item.",
        };

        [NodeInput]
        public InputRange InputListLength { get; } = new InputRange(1, null)
        {
            Title = "List Length:",
            Description = "The number of items in the list.",
            MinValue = 0,
            AutoClearMax = true,
        };

        [NodeInput]
        public InputCheckbox InputAllowWhitespace { get; } = new InputCheckbox(true)
        {
            Title = "Allow Whitespace",
            Description = "If checked, whitespace is allowed between each list item (after the separator).",
        };

        [NodeInput]
        public InputCheckbox InputAllowRegex { get; } = new InputCheckbox(false)
        {
            Title = "Regex in Separator",
            Description = "If checked, the separator will be interpreted as a full regular expression, instead of text.",
        };

        private const string separatorCharsToEscape = "()[]{}$^?.+*|";

        protected override NodeResultBuilder GetValue()
        {
            string whitespace = InputAllowWhitespace.IsChecked ? "\\s*?" : "";

            string separator = InputSeparator.Contents;
            if(!InputAllowRegex.IsChecked)
                separator = separator.EscapeCharacters(separatorCharsToEscape);

            string quantifier = (InputListLength.Min, InputListLength.Max) switch
            {
                (0, 2) => "?",
                (1, 2) => "?",
                (0, null) => "*",
                (1, null) => "*",
                (2, null) => "+",
                (0, int max) => $"{{{0},{max - 1}}}",
                (int a, int b) when a == b => $"{{{a - 1}}}",
                (int a, int b) => $"{{{a - 1},{b - 1}}}"
            };

            string prefix = "(?:";
            string suffix = ")" + quantifier;

            if (InputListLength.Min <= 0)
            {
                prefix = "(?:" + prefix;
                suffix += ")?";
            }

            var builder = new NodeResultBuilder(InputContents.Value);

            builder.Prepend(prefix, this);
            builder.Append(")(?:" + separator + whitespace, this);
            builder.Append(InputContents.Value);
            builder.Append(suffix, this);

            return builder;
        }
    }
}
