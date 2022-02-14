namespace Nodexr.Services;
using Microsoft.AspNetCore.Components;
using Nodexr.NodeTypes;
using Nodexr.RegexParsers;
using Microsoft.AspNetCore.WebUtilities;
using Blazored.Toast.Services;
using BlazorNodes.Core;
using Nodexr.Nodes;

public interface INodeHandler
{
    NodeResult CachedOutput { get; }

    event EventHandler OutputChanged;
    event EventHandler OnRequireNoodleRefresh;
    event EventHandler OnRequireNodeGraphRefresh;

    NodeTree Tree { get; set; }

    void DeleteSelectedNodes();
    void ForceRefreshNodeGraph();
    void ForceRefreshNoodles();
    void TryCreateTreeFromRegex(string regex);
    void RevertPreviousParse();
}

public class NodeHandler : INodeHandler
{
    private NodeTree? tree;

    /// <summary>
    /// The <see cref="NodeTree"/> containing the collection of nodes in the current expression.
    /// </summary>
    public NodeTree Tree
    {
        get => tree!;
        set
        {
            // TODO refactor this. The current implementation breaks if the Output node is replaced.
            if (tree != null) GetOutputNode(tree).OutputChanged -= OnOutputChanged;
            GetOutputNode(value).OutputChanged += OnOutputChanged;
            tree = value;
            ForceRefreshNodeGraph();
            OnOutputChanged(this, EventArgs.Empty);
        }
    }

    public NodeResult CachedOutput => GetOutputNode(Tree).CachedOutput;

    /// <summary>
    /// Called when the output of the node graph has changed.
    /// </summary>
    public event EventHandler? OutputChanged;

    /// <summary>
    /// Called when the state of the noodles has changed, but the noodles have not been re-rendered automatically.
    /// </summary>
    public event EventHandler? OnRequireNoodleRefresh;

    /// <summary>
    /// Called when the state of the node graph has changed, but the node graph has not been re-rendered automatically.
    /// </summary>
    public event EventHandler? OnRequireNodeGraphRefresh;

    private readonly IToastService toastService;

    //Stores the previous tree from before the most recent parse, so that the parse can be reverted.
    private NodeTree? treePrevious;

    public NodeHandler(NavigationManager navManager, IToastService toastService)
    {
        this.toastService = toastService;

        var uriParams = QueryHelpers.ParseQuery(navManager.ToAbsoluteUri(navManager.Uri).Query);
        if (uriParams.TryGetValue("parse", out var parseString))
        {
            TryCreateTreeFromRegex(parseString[0]);
        }

        if (Tree is null)
        {
            Tree = CreateDefaultNodeTree();
        }
    }

    /// <summary>
    /// Parses a Regular Expression and creates a node tree from it. The previous <c>Tree</c> is then overwritten by the new one.
    /// </summary>
    /// <param name="regex">The regular expression to parse, in string format</param>
    /// <returns>Whether or not the parse attempt succeeded</returns>
    public void TryCreateTreeFromRegex(string regex)
    {
        var parseResult = RegexParser.Parse(regex);

        if (parseResult.Success)
        {
            treePrevious = tree;
            Tree = parseResult.Value;
            ForceRefreshNodeGraph();
            OnOutputChanged(this, EventArgs.Empty);

            if (CachedOutput.Expression == regex)
            {
                var fragment = Components.ToastButton.GetRenderFragment(RevertPreviousParse, "Revert to previous");
                toastService.ShowSuccess(fragment, "Converted to node tree successfully");
            }
            else
            {
                var fragment = Components.ToastButton.GetRenderFragment(
                    RevertPreviousParse,
                    "Revert to previous",
                    "Your expression was parsed, but the resulting output is slighty different to your input. " +
                        "This is most likely due to a simplification that has been performed automatically.\n");
                toastService.ShowInfo(fragment, "Converted to node tree");
            }
        }
        else
        {
            toastService.ShowError(parseResult.Error?.ToString(), "Couldn't parse input");
            Console.WriteLine("Couldn't parse input: " + parseResult.Error);
        }
    }

    public void RevertPreviousParse()
    {
        Tree = treePrevious ?? throw new InvalidOperationException("No previous tree to revert to");
        ForceRefreshNodeGraph();
        OnOutputChanged(this, EventArgs.Empty);
    }

    public void ForceRefreshNodeGraph()
    {
        OnRequireNodeGraphRefresh?.Invoke(this, EventArgs.Empty);
    }

    public void ForceRefreshNoodles()
    {
        OnRequireNoodleRefresh?.Invoke(this, EventArgs.Empty);
    }

    public void DeleteSelectedNodes()
    {
        var selectedNodes = Tree.GetSelectedNodes().ToList();
        if (selectedNodes.Count == 0) return;

        foreach (var node in selectedNodes)
        {
            if (node is OutputNode)
            {
                toastService.ShowInfo("", "Can't delete the Output node");
            }
            else
            {
                Tree.DeleteNode(node);
            }
        }
        OutputChanged?.Invoke(this, EventArgs.Empty);
        ForceRefreshNodeGraph();
    }

    private static OutputNode GetOutputNode(NodeTree tree)
    {
        return tree.Nodes.OfType<OutputNode>().Single();
    }

    private void OnOutputChanged(object? sender, EventArgs e)
    {
        OutputChanged?.Invoke(this, e);
    }

    private static NodeTree CreateDefaultNodeTree()
    {
        var tree = new NodeTree();
        var quoteNode1 = new TextNode() { Pos = new(200, 300) };
        quoteNode1.Input.Value = "\"";
        var charSetNode = new CharSetNode() { Pos = new(200, 450) };
        charSetNode.InputCount.Value = IQuantifiableNode.Reps.ZeroOrMore;
        charSetNode.InputCharacters.Value = "\"";
        charSetNode.InputDoInvert.Checked = true;
        var groupNode = new GroupNode()
        {
            Pos = new(500, 300),
            PreviousNode = quoteNode1,
        };
        groupNode.Input.ConnectedNode = charSetNode;
        var quoteNode2 = new TextNode()
        {
            Pos = new(800, 300),
            PreviousNode = groupNode,
        };
        quoteNode2.Input.Value = "\"";
        var defaultOutput = new OutputNode()
        {
            Pos = new(1100, 300),
            PreviousNode = quoteNode2,
        };

        tree.AddNode(quoteNode1);
        tree.AddNode(quoteNode2);
        tree.AddNode(groupNode);
        tree.AddNode(charSetNode);
        tree.AddNode(defaultOutput);
        return tree;
    }
}
