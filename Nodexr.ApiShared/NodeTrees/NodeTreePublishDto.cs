using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

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
