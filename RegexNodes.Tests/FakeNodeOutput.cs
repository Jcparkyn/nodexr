using System;
using System.Collections.Generic;
using System.Text;
using RegexNodes.Shared;

namespace RegexNodes.Tests
{
    class FakeNodeOutput : INodeOutput
    {
        private string output;

        public Vector2L OutputPos => throw new NotImplementedException();

        public string CssName => throw new NotImplementedException();

        public string CssColor => throw new NotImplementedException();

        public string GetOutput() => output;

        public FakeNodeOutput(string output) => this.output = output;
    }
}
