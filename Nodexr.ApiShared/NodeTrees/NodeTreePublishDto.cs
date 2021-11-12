namespace Nodexr.ApiShared.NodeTrees;
using System.ComponentModel.DataAnnotations;

public class NodeTreePublishDto
{
    [Required]
    public string Title { get; set; }

    public string Expression { get; set; }

    public string Description { get; set; }
}
