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
        private readonly ReadOnlyCollection<RegexSegment> contents;

        public string Expression =>
            string.Concat(
                contents.Select(segment => segment.Expression));

        public NodeResult(IList<RegexSegment> contents)
        {
            this.contents = new ReadOnlyCollection<RegexSegment>(contents);
        }
        public NodeResult(string expression, INodeOutput node)
        {
            var segment = new RegexSegment(expression, node);
            var segments = new List<RegexSegment> (){ segment };
            contents = new ReadOnlyCollection<RegexSegment>(segments);
        }

        public IEnumerator<RegexSegment> GetEnumerator() => contents.GetEnumerator();
        IEnumerator IEnumerable.GetEnumerator() => contents.GetEnumerator();
    }

    public class NodeResultBuilder
    {
        private readonly List<RegexSegment> contents;

        public NodeResultBuilder()
        {
            this.contents = new List<RegexSegment>();
        }

        public NodeResultBuilder(string expression, INodeOutput node)
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

        public void Prepend(string expr, INodeOutput node)
        {
            var segment = new RegexSegment(expr, node);
            contents.Insert(0, segment);
        }

        public void Prepend(NodeResult segments)
        {
            contents.InsertRange(0, segments);
        }

        public void Append(string expr, INodeOutput node)
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
            if (contents is null || !contents.Any())
            {
                return;
            }
            var first = contents.First();
            var last = contents.Last();
            if (first.Expression == "(?:"
                && last.Expression == ")"
                && ReferenceEquals(first.Node, last.Node))
            {
                contents.RemoveAt(contents.Count - 1);
                contents.RemoveAt(0);
            }
        }

        public NodeResult Build()
        {
            return new NodeResult(contents);
        }
    }

    public readonly struct RegexSegment
    {
        public readonly string Expression { get; }
        public readonly INodeOutput Node { get; }

        public RegexSegment(string expression, INodeOutput node)
        {
            Expression = expression;
            Node = node;
        }
    }
}
