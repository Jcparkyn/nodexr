using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Pidgin;
using RegexNodes.Shared.RegexParsers;
using RegexNodes.Shared.NodeTypes;
using RegexNodes.Shared;
using static RegexNodes.Shared.RegexParsers.QuantifierParser;

namespace RegexNodes.Tests.RegexParserTests
{
    class QuantifierParserTests
    {
        [TestCase("a*")]
        [TestCase("a{0}")]
        [TestCase("ab{0}")]
        [TestCase("a{0,1}")]
        [TestCase("[a]ab{0,1}")]
        [TestCase("[a]a{0,1}")]
        public void VariousEndingWithQuantifier_OutputsSame(string input)
        {
            var node = RegexParser.ParseRegex.ParseOrThrow(input);

            Assert.That(node, Is.TypeOf<QuantifierNode>());
            Assert.AreEqual(input, node.CachedOutput);
        }
        
        [TestCase("+", QuantifierNode.Repetitions.oneOrMore)]
        [TestCase("*", QuantifierNode.Repetitions.zeroOrMore)]
        [TestCase("?", QuantifierNode.Repetitions.zeroOrOne)]
        [TestCase("{0}", QuantifierNode.Repetitions.number)]
        [TestCase("{0,1}", QuantifierNode.Repetitions.range)]
        [TestCase("{0,}", QuantifierNode.Repetitions.range)]
        public void Quantifier_Various_ReturnsQuantifier(string input, string expectedRepetitions)
        {
            var node = ParseQuantifier.ParseOrThrow(input);
            Assert.AreEqual(expectedRepetitions, node.InputCount.DropdownValue);
        }

        [TestCase("[a]+", "[a]")]
        public void ParseQuantifier_AfterCharSet_ReturnsCharSetWithQuantifier(string input, string expectedContents)
        {
            var previous = CharSetParser.ParseCharSet;
            var parser = previous.Cast<Node>().WithOptionalQuantifier();
            var node = parser.ParseOrThrow(input) as QuantifierNode;

            Assert.That(node, Is.Not.Null);
            Assert.That(node.InputContents.ConnectedNode, Is.TypeOf<CharSetNode>());
            Assert.AreEqual(expectedContents, node.InputContents.GetValue());
        }
    }
}
