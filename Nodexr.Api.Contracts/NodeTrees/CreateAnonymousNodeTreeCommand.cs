namespace Nodexr.Api.Contracts.NodeTrees;

using MediatR;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

public record CreateAnonymousNodeTreeCommand(
    [property: JsonPropertyName("nodes")] JsonObject Nodes
) : IRequest<string>;
