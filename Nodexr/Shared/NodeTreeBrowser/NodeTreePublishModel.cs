using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace Nodexr.Shared.NodeTreeBrowser
{
    public class NodeTreePublishModel
    {
        [Required]
        public string Title { get; set; }

        public string Expression { get; set; }

        public string Description { get; set; }
    }
}
