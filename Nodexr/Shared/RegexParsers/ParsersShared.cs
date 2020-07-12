using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using Pidgin;
using Nodexr.Shared.NodeTypes;
using static Pidgin.Parser;
using static Pidgin.Parser<char>;

namespace Nodexr.Shared.RegexParsers
{
    public static class ParsersShared
    {
        public static readonly Parser<char, char> EscapeChar = Char('\\');
        public static readonly Parser<char, char> OpenPar = Char('(');
        public static readonly Parser<char, char> ClosePar = Char(')');

        public static Parser<char, T> ReturnLazy<T>(Func<T> func) =>
            Return<T>(default).Select(_ => func());

        public static Parser<char, bool> UpperOrLower(char letter) =>
            CIChar(letter)
            .Select(c => char.IsUpper(c));

        public static Parser<TToken, T?> OptionalOrNull<TToken, T>(this Parser<TToken, T> original)
            where T : struct =>
            original
            .Optional()
            .Select(maybe => maybe.HasValue ?
                             (T?)maybe.Value :
                             null);

        public static Parser<TToken, bool> WasMatched<TToken, T>(this Parser<TToken, T> original) =>
            original.Optional()
            .Select(maybe => maybe.HasValue);

    }
}
