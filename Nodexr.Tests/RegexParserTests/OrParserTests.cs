using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Pidgin;
using Nodexr.Shared.RegexParsers;
using Nodexr.Shared.NodeTypes;
using Nodexr.Shared;
using static Nodexr.Shared.RegexParsers.OrParser;

namespace Nodexr.Tests.RegexParserTests
{
    internal class OrParserTests
    {
        [TestCase("a|b", "(?:a|b)")]
        [TestCase("abc|def", "(?:abc|def)")]
        public void FirstOrSecond_ReturnsOrNode(string input, string expected)
        {
            var node = RegexParser.ParseRegex.ParseOrThrow(input);

            Assert.That(node, Is.TypeOf<OrNode>());
            Assert.AreEqual(expected, node.CachedOutput.Expression);
        }
    }
}
