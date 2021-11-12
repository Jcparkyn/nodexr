namespace Nodexr.Tests;
using NUnit.Framework;
using Nodexr.NodeTypes;

[TestFixture]
public class TextNodeTests
{
    [TestCase(@" ", " ")]
    [TestCase(@"a", "a")]
    [TestCase(@"(a)", "(a)")]
    [TestCase(@"a\\b", @"a\b")]
    [TestCase(@"*", @"*")]
    [TestCase(@"\*", @"*")]
    public void VariousStrings_MatchesString(string contents, string shouldMatch)
    {
        var node = new TextNode();
        node.Input.Value = contents;

        string nodeVal = node.CachedOutput.Expression;

        Assert.That(nodeVal, Is.Not.Null);
        Assert.That(shouldMatch, Does.Match(node.CachedOutput.Expression));
    }
}
