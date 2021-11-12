namespace Nodexr.Tests.RegexParserTests;
using NUnit.Framework;
using Pidgin;
using Nodexr.NodeTypes;
using static Nodexr.RegexParsers.GroupParser;

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
            Assert.AreEqual(expectedContents, lookaround.Input.Value.Expression);
            Assert.AreEqual(expectedType, lookaround.InputGroupType.Value);
        });
    }
}
