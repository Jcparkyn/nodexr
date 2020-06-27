using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RegexNodes.Shared
{
    public class NodeResult
    {
        private ReadOnlyCollection<RegexSegment> contents;
        public IReadOnlyCollection<RegexSegment> Contents => contents;

        public NodeResult(IList<RegexSegment> contents)
        {
            this.contents = new ReadOnlyCollection<RegexSegment>(contents);
        }
    }

    public class NodeResultBuilder
    {
        private List<RegexSegment> contents;

        public NodeResultBuilder()
        {
            this.contents = new List<RegexSegment>();
        }

        public NodeResultBuilder(NodeResult contents)
        {
            this.contents = new List<RegexSegment>(contents.Contents);
        }

        public void Prepend(string expr, INodeOutput node)
        {
            var segment = new RegexSegment(expr, node);
            contents.Prepend(segment);
        }

        public void Append(string expr, INodeOutput node)
        {
            var segment = new RegexSegment(expr, node);
            contents.Append(segment);
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
