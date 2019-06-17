using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace RegexNodes.Shared.NodeTypes
{
    public class ExactString : Node
    {
        public override string Title => "Exact String";

        public override List<INodeInput> NodeInputs
        {
            get
            {
                return new List<INodeInput> { Input };
            }
        }

        public InputString Input { get; set; } = new InputString("");

        public ExactString() { }
        public ExactString(string contents)
        {
            Input = new InputString(contents);
        }

        static readonly HashSet<char> charsToEscape = new HashSet<char> { '\\', '/', '(', ')', '[', ']', '{', '}', '$', '^', '?', '^', '.', '+', '*', '|' };

        public override string GetValue()
        {
            string result = Input.GetValue();

            if (String.IsNullOrEmpty(result))
            {
                CachedValue = null;
                return null;
            }

            result = result.EscapeCharacters(charsToEscape);

            return UpdateCache(result);
        }
    }
}
