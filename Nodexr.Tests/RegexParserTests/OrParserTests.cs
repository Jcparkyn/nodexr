namespace Nodexr.Tests.RegexParserTests;
using NUnit.Framework;
using Pidgin;
using Nodexr.RegexParsers;
using Nodexr.NodeTypes;

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
