using System;
using NUnit.Framework;
using Nodexr;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using Nodexr.Shared;
using Nodexr.Shared.NodeTypes;

using Reps = Nodexr.Shared.NodeTypes.IQuantifiableNode.Reps;

namespace Nodexr.Tests
{
    [TestFixture]
    public class QuantifierTests
    {

        [TestCase(@".", Reps.OneOrMore, ExpectedResult = @".+")]
        public string Greedy_ReturnsOutput(string contents, Reps repetitions)
        {
            var node = CreateDefaultQuantifier(contents);
            node.InputCount.Value = repetitions;

            return node.CachedOutput.Expression;
        }

        [TestCase(@".", ExpectedResult = @".*")]
        [TestCase(@"(.)", ExpectedResult = @"(.)*")]
        [TestCase(@"(test)", ExpectedResult = @"(test)*")]
        [TestCase(@"[test]", ExpectedResult = @"[test]*")]
        [TestCase(@"\t", ExpectedResult = @"\t*")]
        public string GetOutput_GroupedContents_ReturnsContentsWithAsterisk(string contents)
        {
            var node = CreateDefaultQuantifier(contents);
            node.InputCount.Value = Reps.ZeroOrMore;
            return node.CachedOutput.Expression;
        }

        [TestCase(@"test", ExpectedResult = @"(?:test)*")]
        [TestCase(@"\t\n", ExpectedResult = @"(?:\t\n)*")]
        [TestCase(@"(a)(b)", ExpectedResult = @"(?:(a)(b))*")]
        public string GetOutput_UngroupedContents_ReturnsContentsGroupedWithAsterisk(string contents)
        {
            var node = new QuantifierNode();
            node.InputCount.Value = Reps.ZeroOrMore;
            var input = new TextNode();
            input.Input.Value = contents;
            input.InputEscapeSpecials.Checked = false;

            node.InputContents.ConnectedNode = input;
            node.InputSearchType.Value = QuantifierNode.SearchMode.Greedy;

            return node.CachedOutput.Expression;
        }

        [TestCase(Reps.ZeroOrMore, ExpectedResult = @"*")]
        [TestCase(Reps.OneOrMore, ExpectedResult = @"+")]
        [TestCase(Reps.ZeroOrOne, ExpectedResult = @"?")]
        public string GetSuffix_BasicModes_ReturnsSuffix(Reps mode)
        {
            var node = new QuantifierNode();
            node.InputCount.Value = mode;
            return IQuantifiableNode.GetSuffix(node);
        }

        [TestCase(0, ExpectedResult = @"{0}")]
        [TestCase(1, ExpectedResult = @"{1}")]
        [TestCase(99, ExpectedResult = @"{99}")]
        [TestCase(null, ExpectedResult = @"{0}")]
        public string GetSuffix_Number_ReturnsSuffix(int? number)
        {
            var node = new QuantifierNode();
            node.InputCount.Value = Reps.Number;
            node.InputNumber.Value = number;
            return IQuantifiableNode.GetSuffix(node);
        }

        [TestCase(0, 0, ExpectedResult = @"{0,}")]
        [TestCase(0, 1, ExpectedResult = @"{0,1}")]
        [TestCase(0, 99, ExpectedResult = @"{0,99}")]
        [TestCase(null, 1, ExpectedResult = @"{0,1}")]
        [TestCase(1, 99, ExpectedResult = @"{1,99}")]
        [TestCase(1, null, ExpectedResult = @"{1,}")]
        public string GetSuffix_Range_ReturnsSuffix(int? min, int? max)
        {
            var node = new QuantifierNode();
            node.InputCount.Value = Reps.Range;
            node.InputRange.Min = min;
            node.InputRange.Max = max;
            return IQuantifiableNode.GetSuffix(node);
        }

        private static QuantifierNode CreateDefaultQuantifier(string contents)
        {
            var node = new QuantifierNode();
            node.InputContents.ConnectedNode = new FakeNodeOutput(contents);
            return node;
        }
    }
}
