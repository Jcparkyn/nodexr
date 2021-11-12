namespace Nodexr.Tests;
using System;
using Nodexr.Shared.Nodes;
using BlazorNodes.Core;

internal class FakeNodeOutput : INodeOutput<NodeResult>
{
    private readonly string output;

#pragma warning disable CS0067 // TODO: use this in a test
    public event EventHandler OutputChanged;
#pragma warning restore CS0067

    public Vector2 OutputPos => throw new NotImplementedException();

    public string CssName => throw new NotImplementedException();

    public string CssColor => throw new NotImplementedException();

    public NodeResult CachedOutput => new(output, null);

    public FakeNodeOutput(string output) => this.output = output;
}
