using System;
using NUnit.Framework;
using Pidgin;
using Nodexr.RegexParsers;
using Nodexr.NodeTypes;
using Nodexr.Shared.Nodes;

namespace Nodexr.Tests.RegexParserTests
{
    internal class RegexParserTests
    {
        [Test]
        public void ParseRegex_Empty_ReturnsNull()
        {
            var outputNode = RegexParser.ParseRegex.ParseOrThrow("");
            Assert.IsNull(outputNode);
        }

        [TestCase(@"abc")]
        [TestCase(@"abc[abc]")]
        [TestCase(@"[abc]+")]
        [TestCase(@"(a[bc])+")]
        [TestCase(@"^\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*$")] //Email address
        [TestCase(@"^[-+]?[0-9]*\.?[0-9]+([eE][-+]?[0-9]+)?$")] //Floating point
        [TestCase(@"^((25[0-5]|(2[0-4]|1[0-9]|[1-9]|)[0-9])(\.(?!$)|$)){4}$")] //IPv4. Contains empty group option.
        public void ParseRegex_OutputIsSameAsInput(string input)
        {
            var outputNode = RegexParser.ParseRegex.ParseOrThrow(input);
            Assert.AreEqual(input, outputNode.CachedOutput.Expression);
        }

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
            var node2 = node1.PreviousNode as RegexNodeViewModelBase;
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
