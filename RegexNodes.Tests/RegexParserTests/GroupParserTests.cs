using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Pidgin;
using RegexNodes.Shared.RegexParsers;
using RegexNodes.Shared.NodeTypes;
using RegexNodes.Shared;
using static RegexNodes.Shared.RegexParsers.GroupParser;

namespace RegexNodes.Tests.RegexParserTests
{
    class GroupParserTests
    {
        [TestCase(@"(a)", @"a")]
        [TestCase(@"(abc)", @"abc")]
        [TestCase(@"(?:a)", @"a")]
        [TestCase(@"(?<name>a)", @"a")]
        public void VariousGroups_ReturnsGroupWithContents(string input, string expectedContents)
        {
            var result = ParseGroup.ParseOrThrow(input) as GroupNode;
            Assert.That(result, Is.Not.Null);
            Assert.AreEqual(expectedContents, result.Input.GetValue());
        }
    }
}
