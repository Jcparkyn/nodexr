using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegexNodes.Shared.NodeTypes
{
    public class WildcardNode : Node
    {
        public override string Title => "Wildcard";
        public override string NodeInfo => "Matches any of the specified types of character. Note: the 'Everything' option will only match newlines if the Regex is in singleline mode.";

        [NodeInput]
        protected InputCheckbox InputAllowAll { get; } = new InputCheckbox(true) { Title = "Everything" };
        [NodeInput]
        protected InputCheckbox InputAllowWhitespace { get; } = new InputCheckbox() { Title = "Whitespace" };
        [NodeInput]
        protected InputCheckbox InputAllowLetters { get; } = new InputCheckbox() { Title = "Letters" };
        [NodeInput]
        protected InputCheckbox InputAllowDigits { get; } = new InputCheckbox() { Title = "Digits" };
        [NodeInput]
        protected InputCheckbox InputAllowUnderscore { get; } = new InputCheckbox() { Title = "Underscore" };
        [NodeInput]
        protected InputCheckbox InputAllowOther { get; } = new InputCheckbox() { Title = "Other" };

        public WildcardNode()
        {
            bool isAllowAllUnchecked() => !InputAllowAll.IsChecked;
            InputAllowWhitespace.IsEnabled = isAllowAllUnchecked;
            InputAllowUnderscore.IsEnabled = isAllowAllUnchecked;
            InputAllowLetters.IsEnabled = isAllowAllUnchecked;
            InputAllowDigits.IsEnabled = isAllowAllUnchecked;
            InputAllowOther.IsEnabled = isAllowAllUnchecked;
        }

        public override string GetValue()
        {
            string result;
            bool allowWhitespace = InputAllowWhitespace.IsChecked;
            bool allowUnderscore = InputAllowUnderscore.IsChecked;
            bool allowLetters = InputAllowLetters.IsChecked;
            bool allowDigits = InputAllowDigits.IsChecked;

            if (InputAllowAll.IsChecked)
            {
                result = ".";
            }
            else
            {
                //If "Other" is checked, use an inverted class
                if (InputAllowOther.IsChecked)
                {
                    string charsToExcept = "";
                    if (!allowWhitespace)
                    {
                        charsToExcept += @"\s";
                    }
                    if (!allowLetters && !allowDigits && !allowUnderscore)
                    {
                        charsToExcept += @"\w";
                    }
                    else
                    {
                        if (!allowUnderscore)
                        {
                            charsToExcept += @"_";
                        }
                        if (!allowLetters)
                        {
                            charsToExcept += @"a-zA-Z";
                        }
                        if (!allowDigits)
                        {
                            charsToExcept += @"\d";
                        }
                    }

                    if (charsToExcept.Length > 0)
                    {
                        result = "[^" + charsToExcept + "]"; 
                    }
                    else
                    {
                        result = ".";
                    }
                }

                //If "Other" is unchecked, use a normal class
                else
                {
                    var charsToInclude = new List<string>();

                    if (allowWhitespace)
                    {
                        charsToInclude.Add(@"\s");
                    }

                    if(allowLetters && allowDigits && allowUnderscore){
                        charsToInclude.Add(@"\w");
                    }
                    else
                    {
                        if (allowUnderscore)
                        {
                            charsToInclude.Add(@"_");
                        }
                        if (allowLetters)
                        {
                            charsToInclude.Add(@"a-zA-Z");
                        }
                        if (allowDigits)
                        {
                            charsToInclude.Add(@"\d");
                        }
                    }

                    if (charsToInclude.Count > 1)
                    {
                        result = "[" + string.Join("", charsToInclude) + "]";
                    }
                    else
                    {
                        result = charsToInclude.FirstOrDefault();
                    }
                }
            }
            return UpdateCache(result);
        }
    }
}
