using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using RegexNodes.Shared.NodeTypes;
using RegexNodes.Shared;

namespace RegexNodes.Shared.RegexParsers
{
    public class NodeTreeBuilder
    {
        private readonly NodeTree tree;
        private readonly Node endNode;
        private List<List<(Node node, double pos)>> columns;
        public NodeTreeBuilder(Node endNode)
        {
            this.endNode = endNode;
            tree = new NodeTree();
        }

        public NodeTree Build()
        {
            FillColumns(endNode);
            return tree;
        }

        public void GenerateLayout()
        {

        }

        void FillColumns(Node endNode)
        {
            //var outputPos = new Vector2L(1200, 300);
            var output = new OutputNode();// { Pos = outputPos };
            output.PreviousNode = endNode;
            tree.AddNode(output);

            AddNodeChildren(output, 0, 0);
        }

        void FillNextColumn(int previousColumn)
        {
            foreach(var (node, pos) in columns[previousColumn])
            {
                List<Node> children = GetChildren(node);
                foreach (var child in children)
                {
                    columns[previousColumn + 1].Add((child, pos));
                }
            }
        }

        private static List<Node> GetChildren(Node node)
        {
            var inputs = node.GetInputsRecursive().OfType<InputProcedural>();
            var children = inputs.Select(input => input.ConnectedNode).OfType<Node>().ToList();
            return children;
        }

        void AddNodeChildren(Node parent, double pos, int column)
        {
            var inputs = parent.GetInputsRecursive().OfType<InputProcedural>();
            var children = inputs.Select(input => input.ConnectedNode).OfType<Node>().ToList();

            foreach (var child in children)
            {
                tree.AddNode(child);
                AddToColumn(child, pos, column);
                AddNodeChildren(child, pos, column + 1);
            }
        }

        void AddToColumn (Node node, double pos, int column)
        {
            columns[column].Add((node, pos));
        }

        //List<Node> GetNodeChildren(Node parent)
        //{

        //}
    }
}
