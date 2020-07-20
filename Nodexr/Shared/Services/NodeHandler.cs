using Microsoft.AspNetCore.Components;
using Nodexr.Shared.NodeTypes;
using Nodexr.Shared.RegexParsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Nodexr.Shared.Nodes;
using Nodexr.Shared.NodeInputs;
using Blazored.Toast.Services;

namespace Nodexr.Shared.Services
{
    public interface INodeHandler
    {
        NodeResult CachedOutput { get; }

        event EventHandler OutputChanged;
        event EventHandler OnRequireNoodleRefresh;
        event EventHandler OnRequireNodeGraphRefresh;

        INode SelectedNode { get; }
        NodeTree Tree { get; }

        void DeleteSelectedNode();
        void ForceRefreshNodeGraph();
        void ForceRefreshNoodles();
        void SelectNode(INode node);
        void DeselectAllNodes();
        bool TryCreateTreeFromRegex(string regex);
        bool IsNodeSelected(INode node);
    }

    public class NodeHandler : INodeHandler
    {
        private NodeTree tree;

        /// <summary>
        /// The <c>NodeTree</c> containing the collection of nodes in the current expression.
        /// </summary>
        public NodeTree Tree
        {
            get => tree;
            private set
            {
                if(tree != null) tree.OutputChanged -= OnOutputChanged;
                value.OutputChanged += OnOutputChanged;
                tree = value;
            }
        }

        public NodeResult CachedOutput => Tree.CachedOutput;

        /// <summary>
        /// The currently selected node.
        /// </summary>
        public INode SelectedNode { get; private set; }

        /// <summary>
        /// Called when the output of the node graph has changed.
        /// </summary>
        public event EventHandler OutputChanged;

        /// <summary>
        /// Called when the state of the noodles has changed, but the noodles have not been re-rendered automatically.
        /// </summary>
        public event EventHandler OnRequireNoodleRefresh;

        /// <summary>
        /// Called when the state of the node graph has changed, but the node graph has not been re-rendered automatically.
        /// </summary>
        public event EventHandler OnRequireNodeGraphRefresh;

        private readonly IToastService toastService;

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
                Tree = new NodeTree();
                CreateDefaultNodes(Tree);
            }

            Tree.OutputChanged += OnOutputChanged;
        }

        /// <summary>
        /// Parses a Regular Expression and creates a node tree from it. The previous <c>Tree</c> is then overwritten by the new one.
        /// </summary>
        /// <param name="regex">The regular expression to parse, in string format</param>
        /// <returns>Whether or not the parse attempt succeeded</returns>
        public bool TryCreateTreeFromRegex(string regex)
        {
            var parseResult = RegexParser.Parse(regex);

            if (parseResult.Success)
            {
                Tree = parseResult.Value;
                ForceRefreshNodeGraph();
                OnOutputChanged(this, EventArgs.Empty);
                return true;
            }
            else
            {
                toastService.ShowError(parseResult.Error.ToString(), "Couldn't parse input");
                Console.WriteLine("Couldn't parse input: " + parseResult.Error);
                return false;
            }
        }

        public void ForceRefreshNodeGraph()
        {
            OnRequireNodeGraphRefresh?.Invoke(this, EventArgs.Empty);
        }

        public void ForceRefreshNoodles()
        {
            OnRequireNoodleRefresh?.Invoke(this, EventArgs.Empty);
        }

        public void SelectNode(INode node)
        {
            var selectedNodePrevious = SelectedNode;
            SelectedNode = node;
            selectedNodePrevious?.OnDeselected(EventArgs.Empty);
            node.OnSelected(EventArgs.Empty);
        }

        public void DeselectAllNodes()
        {
            if (SelectedNode != null)
            {
                SelectedNode.OnLayoutChanged(this, EventArgs.Empty);
                var previousSelectedNode = SelectedNode;
                SelectedNode = null;
                previousSelectedNode.OnDeselected(EventArgs.Empty);
                //ForceRefreshNodeGraph();
            }
        }

        public bool IsNodeSelected(INode node)
        {
            return ReferenceEquals(SelectedNode, node);
        }

        public void DeleteSelectedNode()
        {
            if (SelectedNode != null)
            {
                Tree.DeleteNode(SelectedNode, false);
                SelectedNode = null;
                ForceRefreshNodeGraph();
            }
        }

        private void OnOutputChanged(object sender, EventArgs e)
        {
            OutputChanged?.Invoke(this, e);
        }

        private void CreateDefaultNodes(NodeTree tree)
        {
            var defaultOutput = new OutputNode() { Pos = new Vector2L(1100, 300) };
            var defaultTextNodeFox = new TextNode() { Pos = new Vector2L(300, 200) };
            var defaultTextNodeDog = new TextNode() { Pos = new Vector2L(300, 450) };
            defaultTextNodeFox.Input.Contents = "fox";
            defaultTextNodeDog.Input.Contents = "dog";
            var defaultOrNode = new OrNode(new List<INodeOutput>() { defaultTextNodeFox, defaultTextNodeDog })
            {
                Pos = new Vector2L(700, 300)
            };
            defaultOutput.PreviousNode = defaultOrNode;
            tree.AddNode(defaultTextNodeFox);
            tree.AddNode(defaultTextNodeDog);
            tree.AddNode(defaultOrNode);
            tree.AddNode(defaultOutput);
        }
    }
}
