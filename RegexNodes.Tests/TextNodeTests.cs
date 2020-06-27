using System;
using NUnit.Framework;
using RegexNodes;
using System.Text.RegularExpressions;
using RegexNodes.Shared;
using RegexNodes.Shared.NodeTypes;

namespace RegexNodes.Tests
{

    [TestFixture]
    public class TextNodeTests
    {
        TextNode _node;

        [SetUp]
        public void SetUp()
        {
            _node = new TextNode();
        }

        [TestCase(@" ")]
        [TestCase(@"Testing")]
        [TestCase(@"(Price) is: $21.99?")]
        [TestCase(@"High\medium\\low high/medium//low")]
        [TestCase(@"((][fg]lnd#$23\4)_")]
        public void VariousStrings(string contents)
        {
            //_node.Input = new InputString(contents);

            Assert.That("Test case: " + contents + " End.", Does.Match(_node.CachedOutput));
        }
    }
}