using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Pidgin;
using Nodexr.RegexParsers;
using Nodexr.Shared.NodeTypes;
using Nodexr.Shared;
using static Nodexr.RegexParsers.GroupParser;

namespace Nodexr.Tests.RegexParserTests
{
    internal class GroupParserTests
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
