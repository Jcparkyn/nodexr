namespace Nodexr.Api.Functions.IntegrationTests.NodeTrees.Commands;

using FluentAssertions;
using Nodexr.Api.Contracts.NodeTrees;
using Nodexr.Api.Functions.Models;
using NUnit.Framework;
using System.Threading.Tasks;
using static Testing;

public class CreateNodeTreeCommandHandlerTests : TestBase
{
    [Test]
    public async Task ShouldCreateNodeTree()
    {
        var command = new CreateNodeTreeCommand()
        {
            Description = "Placeholder",
            Expression = "[a-z]*",
            Title = "Placeholder",
        };

        string? nodeTreeId = await SendAsync(command);

        var nodeTree = await FindAsync<NodeTree>(nodeTreeId);

        nodeTree.Should().NotBeNull();
        nodeTree!.Description.Should().Be(command.Description);
        nodeTree.Expression.Should().Be(command.Expression);
        nodeTree.Title.Should().Be(command.Title);
    }
}
