using System;
using NUnit.Framework;
using RegexNodes;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using RegexNodes.Shared;
using RegexNodes.Shared.NodeTypes;

using Reps = RegexNodes.Shared.NodeTypes.Quantifier.Repetitions;

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

            return node.GetOutput();
        }

        [TestCase(@".", 0, 0, ExpectedResult = @".{0,0}")]
        [TestCase(@".", 0, 1, ExpectedResult = @".{0,1}")]
        [TestCase(@".", 1, 2, ExpectedResult = @".{1,2}")]
        [TestCase(@"(.)", 1, 2, ExpectedResult = @"(.){1,2}")]
        [TestCase(@"(test)", 1, 2, ExpectedResult = @"(test){1,2}")]
        [TestCase(@"[test]", 1, 2, ExpectedResult = @"[test]{1,2}")]
        [TestCase(@"\t", 1, 2, ExpectedResult = @"\t{1,2}")]
        public string RangeWithGroupedContents_ReturnsContentsWithRange(string contents, int min, int max)
        {
            var node = CreateQuantifierWithRange(contents, min, max);
            return node.GetOutput();
        }

        [TestCase(@"test", 1, 2, ExpectedResult = @"(?:test){1,2}")]
        [TestCase(@"\t\n", 1, 2, ExpectedResult = @"(?:\t\n){1,2}")]
        [TestCase(@"(a)(b)", 1, 2, ExpectedResult = @"(?:(a)(b)){1,2}")]
        public string RangeWithUngroupedContents_ReturnsContentsGroupedWithRange(string contents, int min, int max)
        {
            var node = CreateQuantifierWithRange(contents, min, max);
            return node.GetOutput();
        }

        private Quantifier CreateQuantifierWithRange(string contents, int min, int max)
        {
            var node = CreateDefaultQuantifier(contents);

            node.InputCount.DropdownValue = Reps.range;
            node.InputMin.InputContents = min;
            node.InputMax.InputContents = max;

            return node;
        }

        private Quantifier CreateDefaultQuantifier(string contents)
        {
            var node = new Quantifier();
            node.InputContents.InputNode = new FakeNodeOutput(contents);
            return node;
        }
    }
}
