using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegexNodes.Shared.NodeTypes
{
    public class WildcardNode : Node
    {
        public override string Title => "Wildcard";
        public override string NodeInfo => "Matches any of the specified types of character. The 'Everything' option will only match newlines if the Regex is in singleline mode.";

        public override List<INodeInput> NodeInputs
        {
            get
            {
                return new List<INodeInput> { InputAllowAll, InputAllowWhitespace, InputAllowLetters, InputAllowDigits, InputAllowUnderscore, InputAllowOther };
            }
        }

        protected InputCheckbox InputAllowAll = new InputCheckbox(true) { Title = "Everything" };
        protected InputCheckbox InputAllowWhitespace = new InputCheckbox() { Title = "Whitespace" };
        protected InputCheckbox InputAllowLetters = new InputCheckbox() { Title = "Letters" };
        protected InputCheckbox InputAllowDigits = new InputCheckbox() { Title = "Digits" };
        protected InputCheckbox InputAllowUnderscore = new InputCheckbox() { Title = "Underscore" };
        protected InputCheckbox InputAllowOther = new InputCheckbox() { Title = "Other" };

        [Obsolete("Replaced by code")]
        private Dictionary<int, string> OutputMapping =  new Dictionary<int, string>
        {
            { 0b0000, @"" },
            { 0b0001, @"[^\w]" },
            { 0b0010, @"[^\w]" },
            { 0b0011, @"[^\w]" },
            { 0b0100, @"[^\w]" },
            { 0b0101, @"[^\w]" },
            { 0b0110, @"[^\w]" },
            { 0b0111, @"[^\w]" },
            { 0b1000, @"[^\w]" },
            { 0b1001, @"[^\w]" },
            { 0b1010, @"[^\w]" },
            { 0b1011, @"[^\w]" },
            { 0b1100, @"[^\w]" },
            { 0b1101, @"[^\w]" },
            { 0b1110, @"[^\w]" },
            { 0b1111, @"[^\w]" },
        };

        public WildcardNode()
        {
            Func<bool> isAllowAllUnchecked = () => !InputAllowAll.IsChecked;
            InputAllowWhitespace.IsEnabled = isAllowAllUnchecked;
            InputAllowUnderscore.IsEnabled = isAllowAllUnchecked;
            InputAllowLetters.IsEnabled = isAllowAllUnchecked;
            InputAllowDigits.IsEnabled = isAllowAllUnchecked;
            InputAllowOther.IsEnabled = isAllowAllUnchecked;
        }

        public override string GetValue()
        {
            string result = "";
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
                    string charsToInclude = "";

                    if (allowWhitespace)
                    {
                        charsToInclude += @"\s";
                    }

                    if(allowLetters && allowDigits && allowUnderscore){
                        charsToInclude += @"\w";
                    }
                    else
                    {
                        if (allowUnderscore)
                        {
                            charsToInclude += @"_";
                        }
                        if (allowLetters)
                        {
                            charsToInclude += @"a-zA-Z";
                        }
                        if (allowDigits)
                        {
                            charsToInclude += @"\d";
                        }
                    }
                    result = "[" + charsToInclude + "]";
                }
            }
            return UpdateCache(result);
        }
    }
}
