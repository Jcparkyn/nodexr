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
        //private readonly List<List<(Node node, double pos)>> columns;
        private readonly List<int> columnHeights = new List<int>();
        const int spacingX = 250;
        const int spacingY =20;
        static readonly Vector2L outputPos = new Vector2L(1000, 300);
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

        void FillColumns(Node endNode)
        {
            //var outputPos = new Vector2L(1200, 300);
            var output = new OutputNode();// { Pos = outputPos };
            output.PreviousNode = endNode;
            tree.AddNode(output);
            int startHeight = (int)outputPos.y;
            AddToColumn(output, startHeight, 0);
            AddNodeChildren(output, startHeight, 1);
        }

        private static List<Node> GetChildren(Node node)
        {
            var inputs = node.GetInputsRecursive().OfType<InputProcedural>();
            var children = inputs.Select(input => input.ConnectedNode).OfType<Node>().ToList();
            return children;
        }

        void AddNodeChildren(Node parent, int pos, int column)
        {
            var children = GetChildren(parent);
            int childrenHeight = children.Skip(1).Select(node => node.GetHeight() + spacingY).Sum();
            int startPos = pos - childrenHeight / 2;
            foreach (var child in children)
            {
                tree.AddNode(child);
                var childPos = AddToColumn(child, startPos, column);
                AddNodeChildren(child, childPos, column + 1);
            }
        }

        int AddToColumn (Node node, int pos, int column)
        {
            
            if(columnHeights.Count <= column)
            {
                //Assumes that no columns are skipped
                columnHeights.Add(int.MinValue);
            }

            if (columnHeights[column] < pos)
            {
                //Leave empty spaces when appropriate
                columnHeights[column] = pos;
            }

            long xPos = outputPos.x - column * spacingX;
            int yPos = columnHeights[column];
            columnHeights[column] += node.GetHeight() + spacingY;

            node.Pos = new Vector2L(xPos, yPos);
            return yPos;
        }
    }
}
