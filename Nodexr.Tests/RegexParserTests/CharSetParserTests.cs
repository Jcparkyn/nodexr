using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Pidgin;
using Nodexr.RegexParsers;
using Nodexr.Shared.NodeTypes;
using Nodexr.Shared;
using static Nodexr.RegexParsers.CharSetParser;

namespace Nodexr.Tests.RegexParserTests
{
    internal class CharSetParserTests
    {
        [TestCase("[a]", "a")]
        [TestCase("[a-z]", "a-z")]
        [TestCase(@"[\\]", @"\\")]
        public void CharSetNode_NoSpecials_ReturnsContents(string input, string expectedContents)
        {
            CharSetNode result = ParseCharSet.ParseOrThrow(input);
            Assert.AreEqual(expectedContents, result.InputCharacters.Value);
        }

        [TestCase(@"[\]]", @"\]")]
        [TestCase(@"[abc\]def]", @"abc\]def")]
        public void CharSetNode_EscapedBracket_ReturnsContents(string input, string expectedContents)
        {
            var result = ParseCharSet.ParseOrThrow(input);
            Assert.AreEqual(expectedContents, result.InputCharacters.Value);
        }

        [TestCase(@"[^a]", @"a")]
        [TestCase(@"[^abc]", @"abc")]
        public void Inverted_ReturnsContents(string input, string expectedContents)
        {
            var result = ParseCharSet.ParseOrThrow(input);
            Assert.That(result.InputDoInvert.Checked, Is.True);
            Assert.AreEqual(expectedContents, result.InputCharacters.Value);
        }
    }
}
