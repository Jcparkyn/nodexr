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
    public static class ParsersShared
    {
        public static readonly Parser<char, char> EscapeChar = Char('\\');

        public static Parser<char, T> ReturnLazy<T>(Func<T> func) =>
            Return<T>(default).Select(a => func());

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

        public static Parser<TToken, T> OptionalOrDefault<TToken, T>(this Parser<TToken, T> original) =>
            original
            .Optional()
            .Select(maybe => maybe.HasValue ?
                             maybe.Value :
                             default);
    }
}
