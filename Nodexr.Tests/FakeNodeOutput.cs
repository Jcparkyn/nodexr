using System;
using System.Collections.Generic;
using System.Text;
using Nodexr.Shared;
using Nodexr.Shared.Nodes;
using Nodexr.Shared.NodeInputs;

namespace Nodexr.Tests
{
    class FakeNodeOutput : INodeOutput
    {
        readonly private string output;

        public event EventHandler OutputChanged;

        public Vector2L OutputPos => throw new NotImplementedException();

        public string CssName => throw new NotImplementedException();

        public string CssColor => throw new NotImplementedException();

        public NodeResult CachedOutput => new NodeResult(output, this);

        public FakeNodeOutput(string output) => this.output = output;
    }
}
