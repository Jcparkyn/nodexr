namespace Nodexr.Api.Contracts.NodeTrees;

using MediatR;
using System.ComponentModel.DataAnnotations;

public class CreateNodeTreeCommand : IRequest<string>
{
    [Required]
    public string Title { get; set; }

    public string Expression { get; set; }

    public string Description { get; set; }
}
