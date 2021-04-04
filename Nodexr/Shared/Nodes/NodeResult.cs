using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Nodexr.Shared;

namespace Nodexr.Shared.Nodes
{
    public class NodeResult : IEnumerable<RegexSegment>
    {
        public ReadOnlyCollection<RegexSegment> Contents { get; }

        public string Expression =>
            string.Concat(
                Contents.Select(segment => segment.Expression));

        public NodeResult(IList<RegexSegment> contents)
        {
            Contents = new ReadOnlyCollection<RegexSegment>(contents);
        }

        public NodeResult(string expression, IRegexNodeViewModel node)
        {
            var segment = new RegexSegment(expression, node);
            var segments = new List<RegexSegment>() { segment };
            Contents = new ReadOnlyCollection<RegexSegment>(segments);
        }

        public IEnumerator<RegexSegment> GetEnumerator() => Contents.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => Contents.GetEnumerator();
    }

    public class NodeResultBuilder
    {
        private readonly List<RegexSegment> contents;

        public NodeResultBuilder()
        {
            contents = new List<RegexSegment>();
        }

        public NodeResultBuilder(string expression, IRegexNodeViewModel node)
        {
            contents = new List<RegexSegment>
            {
                new RegexSegment(expression, node)
            };
        }

        public NodeResultBuilder(NodeResult contents)
        {
            if (contents is null)
            {
                this.contents = new List<RegexSegment>();
            }
            else
            {
                this.contents = new List<RegexSegment>(contents);
            }
        }

        public void Prepend(string expr, IRegexNodeViewModel node)
        {
            var segment = new RegexSegment(expr, node);
            contents.Insert(0, segment);
        }

        public void Prepend(NodeResult segments)
        {
            contents.InsertRange(0, segments);
        }

        public void Append(string expr, IRegexNodeViewModel node)
        {
            var segment = new RegexSegment(expr, node);
            contents.Add(segment);
        }

        public void Append(NodeResult segments)
        {
            if (!(segments is null))
            {
                contents.AddRange(segments);
            }
        }

        public void StripNonCaptureGroup()
        {
            if (contents is null || contents.Count == 0)
            {
                return;
            }
            var first = contents[0];
            var last = contents.Last();
            if (first.Expression == "(?:"
                && last.Expression == ")"
                && ReferenceEquals(first.Node, last.Node))
            {
                contents.RemoveAt(contents.Count - 1);
                contents.RemoveAt(0);
            }
        }

        public void AddNonCaptureGroup(IRegexNodeViewModel node)
        {
            Prepend("(?:", node);
            Append(")", node);
        }

        public NodeResult Build()
        {
            return new NodeResult(contents);
        }
    }

    public class RegexSegment
    {
        public string Expression { get; }
        public IRegexNodeViewModel Node { get; }

        public RegexSegment(string expression, IRegexNodeViewModel node)
        {
            Expression = expression;
            Node = node;
        }
    }
}
