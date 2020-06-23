using System;
using NUnit.Framework;
using RegexNodes;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using RegexNodes.Shared;
using RegexNodes.Shared.NodeTypes;

using Reps = RegexNodes.Shared.NodeTypes.QuantifierNode.Repetitions;

namespace RegexNodes.Tests
{
    [TestFixture]
    public class QuantifierTests
    {

        [TestCase(@".", Reps.oneOrMore, ExpectedResult = @".+")]
        public string Greedy_ReturnsOutput(string contents, string repetitions)
        {
            var node = CreateDefaultQuantifier(contents);
            node.InputCount.DropdownValue = repetitions;

            return node.CachedOutput;
        }

        [TestCase(@".", ExpectedResult = @".*")]
        [TestCase(@"(.)", ExpectedResult = @"(.)*")]
        [TestCase(@"(test)", ExpectedResult = @"(test)*")]
        [TestCase(@"[test]", ExpectedResult = @"[test]*")]
        [TestCase(@"\t", ExpectedResult = @"\t*")]
        public string GetOutput_GroupedContents_ReturnsContentsWithAsterisk(string contents)
        {
            var node = CreateDefaultQuantifier(contents);
            return node.CachedOutput;
        }

        [TestCase(@"test", ExpectedResult = @"(?:test)*")]
        [TestCase(@"\t\n", ExpectedResult = @"(?:\t\n)*")]
        [TestCase(@"(a)(b)", ExpectedResult = @"(?:(a)(b))*")]
        public string GetOutput_UngroupedContents_ReturnsContentsGroupedWithAsterisk(string contents)
        {
            var node = new QuantifierNode();
            var input = new TextNode();
            input.Input.Contents = contents;
            input.InputDoEscape.IsChecked = false;

            node.InputContents.ConnectedNode = input;
            node.InputSearchType.DropdownValue = Reps.zeroOrMore;

            return node.CachedOutput;
        }

        [TestCase(Reps.zeroOrMore, ExpectedResult = @"*")]
        [TestCase(Reps.oneOrMore, ExpectedResult = @"+")]
        [TestCase(Reps.zeroOrOne, ExpectedResult = @"?")]
        public string GetSuffix_BasicModes_ReturnsSuffix(string mode)
        {
            return Reps.GetSuffix(mode);
        }

        [TestCase(0, ExpectedResult = @"{0}")]
        [TestCase(1, ExpectedResult = @"{1}")]
        [TestCase(99, ExpectedResult = @"{99}")]
        [TestCase(null, ExpectedResult = @"{0}")]
        public string GetSuffix_Number_ReturnsSuffix(int? number)
        {
            const string mode = Reps.number;
            return Reps.GetSuffix(mode, number: number);
        }

        [TestCase(0, 0, ExpectedResult = @"{0,0}")]
        [TestCase(0, 1, ExpectedResult = @"{0,1}")]
        [TestCase(0, 99, ExpectedResult = @"{0,99}")]
        [TestCase(null, 1, ExpectedResult = @"{0,1}")]
        [TestCase(1, 99, ExpectedResult = @"{1,99}")]
        [TestCase(1, null, ExpectedResult = @"{1,}")]
        public string GetSuffix_Range_ReturnsSuffix(int? min, int? max)
        {
            const string mode = Reps.range;
            return Reps.GetSuffix(mode, min: min, max: max);
        }

        private QuantifierNode CreateQuantifierWithRange(string contents, int min, int max)
        {
            var node = CreateDefaultQuantifier(contents);

            node.InputCount.DropdownValue = Reps.range;
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
