using Microsoft.AspNetCore.Components;
using Nodexr.NodeTypes;
using Nodexr.RegexParsers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.WebUtilities;
using Nodexr.Shared.Nodes;
using Blazored.Toast.Services;
using BlazorNodes.Core;

namespace Nodexr.Services
{
    public interface INodeHandler
    {
        NodeResult CachedOutput { get; }

        event EventHandler OutputChanged;
        event EventHandler OnRequireNoodleRefresh;
        event EventHandler OnRequireNodeGraphRefresh;

        NodeTree Tree { get; }

        void DeleteSelectedNodes();
        void ForceRefreshNodeGraph();
        void ForceRefreshNoodles();
        void TryCreateTreeFromRegex(string regex);
        void RevertPreviousParse();
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

        //Stores the previous tree from before the most recent parse, so that the parse can be reverted.
        private NodeTree treePrevious;

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
        public void TryCreateTreeFromRegex(string regex)
        {
            var parseResult = RegexParser.Parse(regex);

            if (parseResult.Success)
            {
                treePrevious = tree;
                Tree = parseResult.Value;
                ForceRefreshNodeGraph();
                OnOutputChanged(this, EventArgs.Empty);

                if(CachedOutput.Expression == regex)
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
                toastService.ShowError(parseResult.Error.ToString(), "Couldn't parse input");
                Console.WriteLine("Couldn't parse input: " + parseResult.Error);
            }
        }

        public void RevertPreviousParse()
        {
            Tree = treePrevious;
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

            ForceRefreshNodeGraph();
        }

        private void OnOutputChanged(object sender, EventArgs e)
        {
            OutputChanged?.Invoke(this, e);
        }

        private static void CreateDefaultNodes(NodeTree tree)
        {
            var defaultOutput = new OutputNode() { Pos = new Vector2(1100, 300) };
            var defaultTextNodeFox = new TextNode() { Pos = new Vector2(300, 200) };
            var defaultTextNodeDog = new TextNode() { Pos = new Vector2(300, 450) };
            defaultTextNodeFox.Input.Value = "fox";
            defaultTextNodeDog.Input.Value = "dog";
            var defaultOrNode = new OrNode(new [] { defaultTextNodeFox, defaultTextNodeDog })
            {
                Pos = new Vector2(700, 300)
            };
            defaultOutput.PreviousNode = defaultOrNode;
            tree.AddNode(defaultTextNodeFox);
            tree.AddNode(defaultTextNodeDog);
            tree.AddNode(defaultOrNode);
            tree.AddNode(defaultOutput);
        }
    }
}
