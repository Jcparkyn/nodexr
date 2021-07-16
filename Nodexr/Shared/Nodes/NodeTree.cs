using Nodexr.NodeTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using Nodexr.Shared;
using Nodexr.Shared.NodeInputs;
using BlazorNodes.Core;

namespace Nodexr.Shared.Nodes
{
    public class NodeTree
    {
        private readonly List<INodeViewModel> nodes = new();

        public IEnumerable<INodeViewModel> Nodes => nodes.AsReadOnly();

        /// <summary>
        /// The currently selected node.
        /// </summary>
        public INodeViewModel SelectedNode { get; set; }

        public NodeResult CachedOutput { get; private set; }

        public event EventHandler OutputChanged;

        public void AddNode(INodeViewModel node)
        {
            nodes.Add(node);

            if (node is OutputNode)
            {
                node.OutputChanged += OnOutputChanged;
                RecalculateOutput();
            }
        }

        public void DeleteNode(INodeViewModel nodeToRemove)
        {
            DeleteOutputNoodles(nodeToRemove);
            nodes.Remove(nodeToRemove);
            foreach(var input in nodeToRemove.GetAllInputs()
                .OfType<InputProcedural>()
                .Where(input => input.Connected))
            {
                input.ConnectedNode = null;
            }
            RecalculateOutput();
        }

        public void SelectNode(INodeViewModel node)
        {
            var selectedNodePrevious = SelectedNode;
            SelectedNode = node;
            selectedNodePrevious?.OnSelectionChanged(EventArgs.Empty);
            node.OnSelectionChanged(EventArgs.Empty);
        }

        public void DeselectAllNodes()
        {
            if (SelectedNode != null)
            {
                SelectedNode.OnLayoutChanged(this, EventArgs.Empty);
                var previousSelectedNode = SelectedNode;
                SelectedNode = null;
                previousSelectedNode.OnSelectionChanged(EventArgs.Empty);
                //ForceRefreshNodeGraph();
            }
        }

        public bool IsNodeSelected(INodeViewModel node)
        {
            return ReferenceEquals(SelectedNode, node);
        }

        private void OnOutputChanged(object sender, EventArgs e)
        {
            RecalculateOutput();
        }

        private OutputNode GetOutputNode()
        {
            return nodes.OfType<OutputNode>().Single();
        }

        public void RecalculateOutput()
        {
            var outputNode = GetOutputNode();

            CachedOutput = outputNode.CachedOutput;
            OutputChanged?.Invoke(this, EventArgs.Empty);
        }

        private void DeleteOutputNoodles(INodeViewModel nodeToRemove)
        {
            foreach (var node in nodes)
            {
                foreach (var input in node.GetAllInputs().OfType<InputProcedural>())
                {
                    DeleteNoodlesBetween(nodeToRemove, input);
                }
            }
        }

        private static void DeleteNoodlesBetween(INodeViewModel node, InputProcedural input)
        {
            if (input.ConnectedNode == node)
            {
                input.ConnectedNode = null;
            }
        }
    }
}
