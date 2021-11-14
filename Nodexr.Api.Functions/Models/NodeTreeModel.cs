namespace Nodexr.Api.Functions.Models;
using System;

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
