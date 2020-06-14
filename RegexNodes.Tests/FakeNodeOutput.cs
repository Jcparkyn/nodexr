using System;
using System.Collections.Generic;
using System.Text;
using RegexNodes.Shared;

namespace RegexNodes.Tests
{
    class FakeNodeOutput : INodeOutput
    {
        private string output;

        public event EventHandler OutputChanged;

        public Vector2L OutputPos => throw new NotImplementedException();

        public string CssName => throw new NotImplementedException();

        public string CssColor => throw new NotImplementedException();

        public string CachedOutput => output;

        public string GetOutput() => output;

        public FakeNodeOutput(string output) => this.output = output;
    }
}
