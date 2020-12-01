using System.Text.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json.Serialization;
using System.ComponentModel.DataAnnotations;

namespace Nodexr.ApiFunctions.Models
{
    public class NodeTreeModel
    {
        public string id { get; set; }

        public string Title { get; set; }

        public string Expression { get; set; }
    }
}
