namespace Nodexr.RegexParsers;
using System.Collections.Generic;
using System.Linq;
using BlazorNodes.Core;
using Nodexr.NodeInputs;
using Nodexr.Nodes;

public class NodeTreeBuilder
{
    private readonly NodeTree tree;
    private readonly RegexNodeViewModelBase endNode;
    private readonly List<int> columnHeights = new();
    private const int SpacingX = 250;
    private const int SpacingY = 20;
    private static readonly Vector2 outputPos = new(1000, 300);

    public NodeTreeBuilder(RegexNodeViewModelBase endNode)
    {
        this.endNode = endNode;
        tree = new NodeTree();
    }

    public NodeTree Build()
    {
        FillColumns(endNode);
        return tree;
    }

    private void FillColumns(RegexNodeViewModelBase endNode)
    {
        tree.AddNode(endNode);
        int startHeight = (int)outputPos.y;
        AddToColumn(endNode, startHeight, 0);
        AddNodeChildren(endNode, startHeight, 1);
    }

    private static List<RegexNodeViewModelBase> GetChildren(RegexNodeViewModelBase node)
    {
        var inputs = node.GetAllInputs().OfType<InputProcedural>();
        var children = inputs.Select(input => input.ConnectedNode).OfType<RegexNodeViewModelBase>().ToList();
        return children;
    }

    private void AddNodeChildren(RegexNodeViewModelBase parent, int pos, int column)
    {
        var children = GetChildren(parent);
        int childrenHeight = children.Skip(1).Select(node => node.GetHeight() + SpacingY).Sum();
        int startPos = pos - (childrenHeight / 2);
        foreach (var child in children)
        {
            tree.AddNode(child);
            int childPos = AddToColumn(child, startPos, column);
            AddNodeChildren(child, childPos, column + 1);
        }
    }

    private int AddToColumn(RegexNodeViewModelBase node, int pos, int column)
    {
        if (columnHeights.Count <= column)
        {
            //Assumes that no columns are skipped
            columnHeights.Add(int.MinValue);
        }

        if (columnHeights[column] < pos)
        {
            //Leave empty spaces when appropriate
            columnHeights[column] = pos;
        }

        double xPos = outputPos.x - (column * SpacingX);
        int yPos = columnHeights[column];
        columnHeights[column] += node.GetHeight() + SpacingY;

        node.Pos = new Vector2(xPos, yPos);
        return yPos;
    }
}
