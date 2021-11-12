namespace Nodexr.Tests.RegexParserTests;
using NUnit.Framework;
using Pidgin;
using Nodexr.NodeTypes;
using static Nodexr.RegexParsers.GroupParser;

internal class GroupParserTests
{
    [TestCase("()")]
    [TestCase("(?:)")]
    [TestCase("(?<name>)")]
    public void EmptyGroup_ReturnsSameValue(string input)
    {
        var result = ParseGroup.ParseOrThrow(input) as GroupNode;
        Assert.IsNotNull(result);
        Assert.AreEqual(input, result.CachedOutput.Expression);
    }

    [TestCase(@"(a)", @"a")]
    [TestCase(@"(abc)", @"abc")]
    [TestCase(@"(?:a)", @"a")]
    [TestCase(@"(?<name>a)", @"a")]
    public void VariousGroups_ReturnsGroupWithContents(string input, string expectedContents)
    {
        var result = ParseGroup.ParseOrThrow(input) as GroupNode;
        Assert.That(result, Is.Not.Null);
        Assert.AreEqual(expectedContents, result.Input.Value.Expression);
    }
}
