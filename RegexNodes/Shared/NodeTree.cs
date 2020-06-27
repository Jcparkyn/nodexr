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

        private List<INode> nodes = new List<INode>();
        public IEnumerable<INode> Nodes => nodes.AsReadOnly();

        public string CachedOutput { get; private set; }

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

            string output = outputNodes.Count() switch
            {
                1 => outputNodes.First().CachedOutput,
                var count when count > 1 => "Too many Output nodes",
                _ => "Add an output node to get started"
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
                DeleteNoodlesBetween(nodeToRemove, node.PreviousNode);

                foreach (var input in node.NodeInputs)
                {
                    DeleteNoodlesBetween(nodeToRemove, input);
                }
            }
        }

        private void DeleteNoodlesBetween(INode node, INodeInput input)
        {
            if (input is InputProcedural inputProcedural && inputProcedural.ConnectedNode == node)
            {
                inputProcedural.ConnectedNode = null;
            }
            else if (input is InputCollection inputCollection)
            {
                foreach (INodeInput _input in inputCollection.Inputs)
                {
                    DeleteNoodlesBetween(node, _input);
                }
            }
        }
    }
}
