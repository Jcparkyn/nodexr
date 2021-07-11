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
    internal class LookaroundParserTests
    {
        [TestCase(@"(?=a)", @"a", LookaroundNode.Types.lookahead)]
        [TestCase(@"(?!a)", @"a", LookaroundNode.Types.lookaheadNeg)]
        [TestCase(@"(?<=a)", @"a", LookaroundNode.Types.lookbehind)]
        [TestCase(@"(?<!a)", @"a", LookaroundNode.Types.lookbehindNeg)]
        public void VariousGroups_ReturnsLookaroundWithContentsAndType(
            string input,
            string expectedContents,
            LookaroundNode.Types expectedType)
        {
            var result = ParseGroup.ParseOrThrow(input);
            var lookaround = result as LookaroundNode;

            Assert.Multiple(() =>
            {
                Assert.That(result, Is.Not.Null);
                Assert.That(result, Is.TypeOf<LookaroundNode>());
                Assert.AreEqual(expectedContents, lookaround.Input.GetValue());
                Assert.AreEqual(expectedType, lookaround.InputGroupType.Value);
            });
        }
    }
}
