namespace Nodexr.Api.Functions.Models;
using System;

public class NodeTree
{
    public string Id { get; set; } = Guid.NewGuid().ToString();

    public bool Searchable { get; set; } = false;

    public string? Title { get; set; }

    public string? Expression { get; set; }

    public string? Description { get; set; }

    public NodeTree(string title, string expression)
    {
        Title = title;
        Expression = expression;
    }

    public NodeTree() { }
}
