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
            return node.CachedOutput.Expression;
        }

        [TestCase(@"test", ExpectedResult = @"(?:test)*")]
        [TestCase(@"\t\n", ExpectedResult = @"(?:\t\n)*")]
        [TestCase(@"(a)(b)", ExpectedResult = @"(?:(a)(b))*")]
        public string GetOutput_UngroupedContents_ReturnsContentsGroupedWithAsterisk(string contents)
        {
            var node = new QuantifierNode();
            var input = new TextNode();
            input.Input.Contents = contents;
            input.InputEscapeSpecials.IsChecked = false;

            node.InputContents.ConnectedNode = input;
            node.InputSearchType.Value = QuantifierNode.SearchMode.Greedy;

            return node.CachedOutput.Expression;
        }

        [TestCase(Reps.ZeroOrMore, ExpectedResult = @"*")]
        [TestCase(Reps.OneOrMore, ExpectedResult = @"+")]
        [TestCase(Reps.ZeroOrOne, ExpectedResult = @"?")]
        public string GetSuffix_BasicModes_ReturnsSuffix(Reps mode)
        {
            return IQuantifiableNode.GetSuffix(mode);
        }

        [TestCase(0, ExpectedResult = @"{0}")]
        [TestCase(1, ExpectedResult = @"{1}")]
        [TestCase(99, ExpectedResult = @"{99}")]
        [TestCase(null, ExpectedResult = @"{0}")]
        public string GetSuffix_Number_ReturnsSuffix(int? number)
        {
            return IQuantifiableNode.GetSuffix(Reps.Number, number: number);
        }

        [TestCase(0, 0, ExpectedResult = @"{0,0}")]
        [TestCase(0, 1, ExpectedResult = @"{0,1}")]
        [TestCase(0, 99, ExpectedResult = @"{0,99}")]
        [TestCase(null, 1, ExpectedResult = @"{0,1}")]
        [TestCase(1, 99, ExpectedResult = @"{1,99}")]
        [TestCase(1, null, ExpectedResult = @"{1,}")]
        public string GetSuffix_Range_ReturnsSuffix(int? min, int? max)
        {
            return IQuantifiableNode.GetSuffix(Reps.Range, min: min, max: max);
        }

        private QuantifierNode CreateQuantifierWithRange(string contents, int min, int max)
        {
            var node = CreateDefaultQuantifier(contents);

            node.InputCount.Value = Reps.Range;
            node.InputMin.InputContents = min;
            node.InputMax.InputContents = max;

            return node;
        }

        private QuantifierNode CreateDefaultQuantifier(string contents)
        {
            var node = new QuantifierNode();
            node.InputContents.ConnectedNode = new FakeNodeOutput(contents);
            return node;
        }
    }
}
