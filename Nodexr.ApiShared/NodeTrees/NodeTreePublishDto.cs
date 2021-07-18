using System.ComponentModel.DataAnnotations;

namespace Nodexr.ApiShared.NodeTrees
{
    public class NodeTreePublishDto
    {
        [Required]
        public string Title { get; set; }

        public string Expression { get; set; }

        public string Description { get; set; }
    }
}
