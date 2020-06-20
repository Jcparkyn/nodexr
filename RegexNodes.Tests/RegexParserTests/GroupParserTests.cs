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
        public void GroupNode_ReturnsGroupWithContents(string input, string expectedContents)
        {
            var result = ParseGroup.ParseOrThrow(input);
            Assert.AreEqual(expectedContents, result.Input.GetValue());
        }
    }
}
