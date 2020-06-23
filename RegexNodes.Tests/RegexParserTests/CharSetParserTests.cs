using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Pidgin;
using RegexNodes.Shared.RegexParsers;
using RegexNodes.Shared.NodeTypes;
using RegexNodes.Shared;
using static RegexNodes.Shared.RegexParsers.CharSetParser;

namespace RegexNodes.Tests.RegexParserTests
{
    class CharSetParserTests
    {
        [TestCase("[a]", "a")]
        [TestCase("[a-z]", "a-z")]
        [TestCase(@"[\\]", @"\\")]
        public void CharSetNode_NoSpecials_ReturnsContents(string input, string expectedContents)
        {
            CharSetNode result = ParseCharSet.ParseOrThrow(input);
            Assert.AreEqual(expectedContents, result.InputCharacters.Contents);
        }

        [TestCase(@"[\]]", @"\]")]
        [TestCase(@"[abc\]def]", @"abc\]def")]
        public void CharSetNode_EscapedBracket_ReturnsContents(string input, string expectedContents)
        {
            var result = ParseCharSet.ParseOrThrow(input);
            Assert.AreEqual(expectedContents, result.InputCharacters.Contents);
        }

        [TestCase(@"[^a]", @"a")]
        [TestCase(@"[^abc]", @"abc")]
        public void Inverted_ReturnsContents(string input, string expectedContents)
        {
            var result = ParseCharSet.ParseOrThrow(input);
            Assert.That(result.InputDoInvert.IsChecked, Is.True);
            Assert.AreEqual(expectedContents, result.InputCharacters.Contents);
        }
    }
}
