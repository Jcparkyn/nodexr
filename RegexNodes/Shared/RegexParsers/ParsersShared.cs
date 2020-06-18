using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Pidgin;
using RegexNodes.Shared.NodeTypes;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace RegexNodes.Shared.RegexParsers
{
    public class ParsersShared
    {
        public static readonly Parser<char, char> EscapeChar = Char('\\');

    }
}
