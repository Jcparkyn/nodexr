using System.Text.Json;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Nodexr.ApiFunctions.Models
{
    public class NodeTreeModel
    {
        public string? id { get; set; } = Guid.NewGuid().ToString();

        public string Title { get; set; }

        public string? Expression { get; set; }

        public string? Description { get; set; }

        public NodeTreeModel(string title)
        {
            Title = title;
        }
    }
}
