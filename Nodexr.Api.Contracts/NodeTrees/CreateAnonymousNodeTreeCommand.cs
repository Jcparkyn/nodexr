namespace Nodexr.Api.Contracts.NodeTrees;

using MediatR;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

public record CreateAnonymousNodeTreeCommand(
    [property: JsonPropertyName("nodes")] List<JsonObject> Nodes
) : IRequest<string>;
