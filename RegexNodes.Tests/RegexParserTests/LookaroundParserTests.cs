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
    class LookaroundParserTests
    {
        [TestCase(@"(?=a)", @"a", LookaroundNode.Types.lookahead)]
        [TestCase(@"(?!a)", @"a", LookaroundNode.Types.lookaheadNeg)]
        [TestCase(@"(?<=a)", @"a", LookaroundNode.Types.lookbehind)]
        [TestCase(@"(?<!a)", @"a", LookaroundNode.Types.lookbehindNeg)]
        public void VariousGroups_ReturnsLookaroundWithContentsAndType(string input, string expectedContents, string expectedType)
        {
            var result = ParseGroup.ParseOrThrow(input);
            var lookaround = result as LookaroundNode;

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<LookaroundNode>());
                Assert.AreEqual(expectedContents, lookaround.Input.GetValue());
                Assert.AreEqual(expectedType, lookaround.InputGroupType.DropdownValue);
            });
        }
    }
}
