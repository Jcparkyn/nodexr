using System;
using NUnit.Framework;
using RegexNodes;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using RegexNodes.Shared;
using RegexNodes.Shared.NodeTypes;
using System.Runtime.InteropServices;
using System.Linq;

namespace RegexNodes.Tests
{
    [TestFixture]
    class OrNodeTests
    {

        [TestCase("a", "b", "a")]
        [TestCase("a", "b", "b")]
        [TestCase("a", "b", "_a_")]
        [TestCase(@"abc", "def", "def")]
        public void Or_VariousPairs_MatchesString(string in0, string in1, string shouldMatch)
        {
            var node = new OrNode();
            node.Inputs.Inputs.ElementAt(0).ConnectedNode = new FakeNodeOutput(in0);
            node.Inputs.Inputs.ElementAt(1).ConnectedNode = new FakeNodeOutput(in1);

            string nodeVal = node.CachedOutput.Expression;

            Assert.That(nodeVal, Is.Not.Null);
            Assert.That(shouldMatch, Does.Match(nodeVal));
        }
    }
}
