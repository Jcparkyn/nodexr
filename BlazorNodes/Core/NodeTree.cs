using System;
using System.Collections.Generic;
using System.Linq;

namespace BlazorNodes.Core
{
    public class NodeTree
    {
        private readonly List<INodeViewModel> nodes = new();

        public IEnumerable<INodeViewModel> Nodes => nodes.AsReadOnly();

        public void AddNode(INodeViewModel node)
        {
            nodes.Add(node);
        }

        public void DeleteNode(INodeViewModel nodeToRemove)
        {
            DeleteOutputNoodles(nodeToRemove);
            nodes.Remove(nodeToRemove);
            foreach (var input in nodeToRemove.GetAllInputs()
                .OfType<IInputPort>()
                .Where(input => input.Connected))
            {
                input.TrySetConnectedNode(null);
            }
        }

        public void SelectNode(INodeViewModel node, bool unselectOthers = true)
        {
            if (node.Selected) return;

            if (unselectOthers)
            {
                DeselectAllNodes();
            }

            node.Selected = true;
        }

        public void DeselectAllNodes()
        {
            foreach (var node in GetSelectedNodes())
            {
                node.Selected = false;
            }
        }

        public IEnumerable<INodeViewModel> GetSelectedNodes() =>
            Nodes.Where(node => node.Selected);

        private void DeleteOutputNoodles(INodeViewModel nodeToRemove)
        {
            foreach (var node in nodes)
            {
                foreach (var input in node.GetAllInputs().OfType<IInputPort>())
                {
                    DeleteNoodlesBetween(nodeToRemove, input);
                }
            }
        }

        private static void DeleteNoodlesBetween(INodeViewModel node, IInputPort input)
        {
            if (ReferenceEquals(input.ConnectedNodeUntyped, node))
            {
                input.TrySetConnectedNode(null);
            }
        }
    }
}
