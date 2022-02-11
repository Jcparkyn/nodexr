namespace Nodexr.Api.Contracts.NodeTrees;

using MediatR;
using System.ComponentModel.DataAnnotations;
using System.Text.Json.Nodes;

public class CreateNodeTreeCommand : IRequest<string>
{
    [Required]
    public string? Title { get; set; }

    [Required]
    public string? Expression { get; set; }

    public string? Description { get; set; }

    public JsonArray? Nodes { get; set; }
}
