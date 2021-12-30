namespace Nodexr.Api.Functions.IntegrationTests.NodeTrees.Queries;

using FluentAssertions;
using Nodexr.Api.Functions.Models;
using Nodexr.Api.Functions.NodeTrees.Queries;
using NUnit.Framework;
using static Testing;

public class GetNodeTreesTests : TestBase
{
    [Test]
    public async Task ShouldReturnMatchingNodeTrees()
    {
        var matchingTrees = new[]
        {
            new NodeTree("Animal", "a"),
            new NodeTree("Animals", "a"),
            new NodeTree("animals", "a"),
            new NodeTree("two animals", "a"),
        };

        var nonMatchingTrees = new[]
        {
            new NodeTree("Thing", "a"),
            new NodeTree("Anima", "a"),
        };

        await AddRangeAsync(matchingTrees);
        await AddRangeAsync(nonMatchingTrees);

        var query = new GetNodeTreesQuery()
        {
            SearchString = "Animal",
        };

        var result = await SendAsync(query);

        result.Contents.Should().BeEquivalentTo(
            matchingTrees,
            x => x.IncludingAllDeclaredProperties()
        );
    }
}
