using RegexNodes.Shared.NodeTypes;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace RegexNodes.Shared
{
    public class NodeTree
    {
        public event EventHandler OutputChanged;

        private readonly List<INode> nodes = new List<INode>();
        public IEnumerable<INode> Nodes => nodes.AsReadOnly();

        public NodeResult CachedOutput { get; private set; }

        public void AddNode<TNode>(bool refreshIndex = true) where TNode : Node, new()
        {
            Node newNode = new TNode();
            newNode.CalculateInputsPos();
            AddNode(newNode, refreshIndex);
        }

        public void AddNode(INode node, bool refreshIndex = true)
        {
            nodes.Add(node);

            if (node is OutputNode)
            {
                node.OutputChanged += OnOutputChanged;
                RecalculateOutput();
            }
        }

        public void DeleteNode(INode nodeToRemove, bool refreshIndex = true)
        {
            DeleteOutputNoodles(nodeToRemove);
            nodes.Remove(nodeToRemove);
            RecalculateOutput();
        }

        void OnOutputChanged(object sender, EventArgs e)
        {
            RecalculateOutput();
        }

        private IEnumerable<OutputNode> GetOutputNodes()
        {
            //TODO: refactor
            return nodes.OfType<OutputNode>();
        }

        public void RecalculateOutput()
        {
            var outputNodes = GetOutputNodes();

            NodeResult output = outputNodes.Count() switch
            {
                1 => outputNodes.First().CachedOutput,
                var count when count > 1 => new NodeResult("Too many Output nodes", null),
                _ => new NodeResult("Add an output node to get started", null)
            };

            if (output != CachedOutput)
            {
                CachedOutput = output;
                OutputChanged?.Invoke(this, EventArgs.Empty);
            }
        }

        private void DeleteOutputNoodles(INode nodeToRemove)
        {
            foreach (var node in nodes)
            {
                foreach (var input in node.GetInputsRecursive().OfType<InputProcedural>())
                {
                    DeleteNoodlesBetween(nodeToRemove, input);
                }
            }
        }

        private void DeleteNoodlesBetween(INode node, InputProcedural input)
        {
            if (input.ConnectedNode == node)
            {
                input.ConnectedNode = null;
            }
        }
    }
}
