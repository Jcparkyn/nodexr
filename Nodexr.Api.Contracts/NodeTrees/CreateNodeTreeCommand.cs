namespace Nodexr.Api.Contracts.NodeTrees;

using MediatR;
using System.ComponentModel.DataAnnotations;

public class CreateNodeTreeCommand : IRequest<string>
{
    [Required]
    public string Title { get; set; } = null!;

    [Required]
    public string Expression { get; set; } = null!;

    public string? Description { get; set; }
}
