using System;
using System.Collections.Generic;
using System.Text;
using NUnit.Framework;
using Pidgin;
using Nodexr.Shared.RegexParsers;
using Nodexr.Shared.NodeTypes;
using Nodexr.Shared;
using static Nodexr.Shared.RegexParsers.GroupParser;

namespace Nodexr.Tests.RegexParserTests
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
