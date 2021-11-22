namespace Nodexr.Api.Functions.Models;
using System;

public class NodeTree
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public string Title { get; set; } = null!;

    public string Expression { get; set; } = null!;

    public string? Description { get; set; }

    public NodeTree(string title, string expression)
    {
        Title = title;
        Expression = expression;
    }

    public NodeTree() { }
}
