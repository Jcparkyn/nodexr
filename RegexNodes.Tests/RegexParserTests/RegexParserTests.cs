using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Pidgin;
using RegexNodes.Shared.RegexParsers;
using RegexNodes.Shared.NodeTypes;
using RegexNodes.Shared;

namespace RegexNodes.Tests.RegexParserTests
{
    class RegexParserTests
    {


/*        [TestCase(@"a", @"a")]
        public void TextNode_BasicString_ReturnsContents(string input, string expectedContents)
        {
            var result = TextParser.ParseTextWithOptionalQuantifier.ParseOrThrow(input);
            Assert.AreEqual(expectedContents, result.Input.Contents);
        }

        [TestCase(@"a")]
        [TestCase(@"abc")]
        [TestCase(@"ab\tc")]
        [TestCase(@"ab\\c")]
        [TestCase(@"ab\\\c")]
        [TestCase(@"\\\*c", Ignore = "Low priority, will fix later")]
        public void TextNode_ValidStrings_OutputsSame(string input)
        {
            var result = RegexParser.ParseTextNode.ParseOrThrow(input);
            Assert.AreEqual(input, result.CachedOutput);
        }

        [TestCase(@"\")]
        [TestCase(@"\\\")]
        [TestCase(@"*")]
        public void TextNode_InvalidStrings_Fails(string input)
        {
            TestDelegate getResult = () => RegexParser.ParseTextNode.ParseOrThrow(input);
            Assert.Throws<ParseException>(getResult);
        }*/

        [TestCase(@"abc[abc]", typeof(TextNode), typeof(CharSetNode))]
        [TestCase(@"_[a]", typeof(TextNode), typeof(CharSetNode))]
        [TestCase(@"_\*[a]", typeof(TextNode), typeof(CharSetNode))]
        [TestCase(@"[abc]def", typeof(CharSetNode), typeof(TextNode))]
        [TestCase(@"(abc)def", typeof(GroupNode), typeof(TextNode))]
        [TestCase(@"(abc)(def)", typeof(GroupNode), typeof(GroupNode))]
        [TestCase(@"(a[bc])def", typeof(GroupNode), typeof(TextNode))]
        public void ParseRegex_TwoChunks_ReturnsNodesInSequence(string input, Type type2, Type type1)
        {
            var node1 = RegexParser.ParseRegex.ParseOrThrow(input);
            var node2 = node1.PreviousNode;

            Assert.Multiple(() =>
            {
                Assert.That(node1, Is.TypeOf(type1));
                Assert.That(node2, Is.TypeOf(type2));
                Assert.AreEqual(input, node1.CachedOutput.Expression);
            });
        }

        [TestCase(@"abc[def]ghi", typeof(TextNode), typeof(CharSetNode), typeof(TextNode))]
        [TestCase(@"ijk(gdef)ghi", typeof(TextNode), typeof(GroupNode), typeof(TextNode))]
        public void ParseRegex_ThreeChunks_ReturnsNodesInSequence(string input, Type type3, Type type2, Type type1)
        {
            var node1 = RegexParser.ParseRegex.ParseOrThrow(input);
            var node2 = node1.PreviousNode as Node;
            var node3 = node2.PreviousNode;

            Assert.Multiple(() =>
            {
                Assert.That(node1, Is.TypeOf(type1));
                Assert.That(node2, Is.TypeOf(type2));
                Assert.That(node3, Is.TypeOf(type3));
                Assert.AreEqual(input, node1.CachedOutput.Expression);
            });
        }
    }
}
