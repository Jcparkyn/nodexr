using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Pidgin;
using RegexNodes.Shared.RegexParsers;
using RegexNodes.Shared.NodeTypes;
using RegexNodes.Shared;
using static RegexNodes.Shared.RegexParsers.OrParser;

namespace RegexNodes.Tests.RegexParserTests
{
    class OrParserTests
    {
        [TestCase("a|b", "(?:a|b)")]
        [TestCase("abc|def", "(?:abc|def)")]
        public void FirstOrSecond_ReturnsOrNode(string input, string expected)
        {
            var node = RegexParser.ParseRegex.ParseOrThrow(input);

            Assert.That(node, Is.TypeOf<OrNode>());
            Assert.AreEqual(expected, node.CachedOutput);
        }
    }
}
