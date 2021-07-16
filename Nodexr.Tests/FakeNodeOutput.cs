using System;
using System.Collections.Generic;
using System.Text;
using Nodexr.Shared.Nodes;
using Nodexr.Shared.NodeInputs;
using BlazorNodes.Core;

namespace Nodexr.Tests
{
    internal class FakeNodeOutput : INodeOutput<NodeResult>
    {
        private readonly string output;

        public event EventHandler OutputChanged;

        public Vector2 OutputPos => throw new NotImplementedException();

        public string CssName => throw new NotImplementedException();

        public string CssColor => throw new NotImplementedException();

        public NodeResult CachedOutput => new(output, null);

        public FakeNodeOutput(string output) => this.output = output;
    }
}
