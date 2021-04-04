using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Pidgin;
using Nodexr.Shared.RegexParsers;
using Nodexr.Shared.NodeTypes;
using Nodexr.Shared;
using Nodexr.Shared.Nodes;
using Nodexr.Shared.NodeInputs;
using static Nodexr.Shared.RegexParsers.QuantifierParser;

namespace Nodexr.Tests.RegexParserTests
{
    internal class QuantifierParserTests
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
            Assert.AreEqual(input, node.CachedOutput.Expression);
        }
        
        [TestCase("+", IQuantifiableNode.Reps.OneOrMore)]
        [TestCase("*", IQuantifiableNode.Reps.ZeroOrMore)]
        [TestCase("?", IQuantifiableNode.Reps.ZeroOrOne)]
        [TestCase("{0}", IQuantifiableNode.Reps.Number)]
        [TestCase("{0,1}", IQuantifiableNode.Reps.Range)]
        [TestCase("{0,}", IQuantifiableNode.Reps.Range)]
        public void Quantifier_Various_ReturnsQuantifier(string input, IQuantifiableNode.Reps expectedRepetitions)
        {
            var node = ParseQuantifier.ParseOrThrow(input);
            Assert.AreEqual(expectedRepetitions, node.InputCount.Value);
        }

        [TestCase("[a]+", "a")]
        [TestCase("[ab]+", "ab")]
        [TestCase("[a]{0,1}", "a")]
        public void ParseQuantifier_AfterCharSet_ReturnsCharSetWithQuantifier(string input, string expectedContents)
        {
            var previous = CharSetParser.ParseCharSet;
            var parser = previous.Cast<RegexNodeViewModelBase>().WithOptionalQuantifier();
            var node = parser.ParseOrThrow(input) as CharSetNode;

            Assert.That(node, Is.Not.Null);
            Assert.AreEqual(expectedContents, node.InputCharacters.GetValue());
        }
    }
}
