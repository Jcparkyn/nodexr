namespace Nodexr.Api.Functions.IntegrationTests.NodeTrees.Queries;

using FluentAssertions;
using Nodexr.Api.Contracts.NodeTrees;
using Nodexr.Api.Functions.Models;
using NUnit.Framework;
using System.Threading.Tasks;
using static Testing;

public class GetNodeTreesTests : TestBase
{
    [Test]
    public async Task ShouldReturnMatchingNodeTrees()
    {
        await AddAsync(new NodeTree("Animal", "fox|dog"));

        var query = new GetNodeTreesQuery()
        {
            SearchString = "Animal",
        };

        var result = await SendAsync(query);

        result.Contents.Should().HaveCountGreaterThanOrEqualTo(1);
    }
}
