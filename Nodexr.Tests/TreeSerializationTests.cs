﻿namespace Nodexr.Tests;

using BlazorNodes.Core;
using Nodexr.Nodes;
using Nodexr.NodeTypes;
using Nodexr.Serialization;
using NUnit.Framework;
using System.Text.Json;

[TestFixture]
public class TreeSerializationTests
{
    [TestCase("[12]{4}")]
    [TestCase("a|b")]
    public void NodeTree_SerializesWithoutError(string expression)
    {
        var tree = RegexParsers.RegexParser.Parse(expression).Value;

        var options = new JsonSerializerOptions()
        {
            ReferenceHandler = new CachePreservingReferenceHandler(),
            WriteIndented = true,
            Converters = { new RegexNodeJsonConverter() },
        };

        var serialized = JsonSerializer.Serialize(tree, options);
        Assert.That(serialized, Is.Not.Null);
    }

    [TestCase(@"abc")]
    [TestCase(@"[12]{4}")]
    [TestCase(@"a|b")]
    [TestCase(@"\b[-+]?\d*\.?\d+([eE][-+]?\d+)?\b")]
    public void NodeTree_HasSameRegexAfterRoundTrip(string expression)
    {
        var tree = RegexParsers.RegexParser.Parse(expression).Value;

        var options = new JsonSerializerOptions()
        {
            ReferenceHandler = new CachePreservingReferenceHandler(),
            WriteIndented = true,
            Converters = { new RegexNodeJsonConverter() },
        };

        string serialized = JsonSerializer.Serialize(tree.Nodes, options);


        var options2 = new JsonSerializerOptions()
        {
            ReferenceHandler = new CachePreservingReferenceHandler(),
            WriteIndented = true,
            Converters = { new RegexNodeJsonConverter() },
        };

        var deserialized = JsonSerializer.Deserialize<List<INodeViewModel>>(serialized, options2);

        Assert.AreEqual(expression, GetOutputNode(deserialized).CachedOutput.Expression);
    }

    private static OutputNode GetOutputNode(IEnumerable<INodeViewModel> nodes)
    {
        return nodes.OfType<OutputNode>().Single();
    }
}
