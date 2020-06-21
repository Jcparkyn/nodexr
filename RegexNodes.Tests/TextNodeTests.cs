using System;
using NUnit.Framework;
using RegexNodes;
using System.Text.RegularExpressions;
using RegexNodes.Shared;
using RegexNodes.Shared.NodeTypes;

namespace RegexNodes.Tests
{

    [TestFixture]
    public class TextNodeTests
    {
        [TestCase(@" ", " ")]
        [TestCase(@"a", "a")]
        [TestCase(@"(a)", "(a)")]
        [TestCase(@"a\\b", @"a\b")]
        [TestCase(@"*", @"*")]
        [TestCase(@"\*", @"*")]
        public void VariousStrings_MatchesString(string contents, string shouldMatch)
        {
            var node = new TextNode(contents);

            string nodeVal = node.CachedOutput;

            Assert.That(nodeVal, Is.Not.Null);
            Assert.That(shouldMatch, Does.Match(node.CachedOutput));
        }
    }
}