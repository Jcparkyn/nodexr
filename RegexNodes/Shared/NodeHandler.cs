using RegexNodes.Shared.NodeTypes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RegexNodes.Shared
{
    public interface INodeHandler
    {
        List<INode> Nodes { get; set; }
        string CachedOutput { get; }
        Action OnRequireNoodleRefresh { get; set; }

        event Action OnOutputHasChanged;
        Action OnNodeCountChanged { get; set; }
        INode SelectedNode { get; set; }
        Action OnRequireNodeGraphRefresh { get; set; }

        void AddNode<T>(bool refreshIndex = true) where T : Node, new();
        void AddNode(INode node, bool refreshIndex = true);
        void DeleteNode(INode node, bool refreshIndex = true);
        OutputNode GetOutputNode();
        void RecalculateOutput();
        void DeleteSelectedNode();
    }

    public class NodeHandler : INodeHandler
    {
        public string CachedOutput { get; private set; } = "";

        public List<INode> Nodes { get; set; } = new List<INode>();
        public INode SelectedNode { get; set; }

        public event Action OnOutputHasChanged;
        public Action OnNodeCountChanged { get; set; }
        public Action OnRequireNoodleRefresh { get; set; }
        public Action OnRequireNodeGraphRefresh { get; set; }

        public void RecalculateOutput()
        {
            string output;
            OutputNode outputNode = GetOutputNode();
            if (outputNode != null)
            {
                output = outputNode.GetOutput();
            }
            else
            {
                output = "Add an 'Output' node to get started";
            }

            if (output != CachedOutput)
            {
                CachedOutput = output;
                OnOutputHasChanged?.Invoke();
            }
        }

        public OutputNode GetOutputNode()
        {
            return (OutputNode)(Nodes.FirstOrDefault(x => x is OutputNode));
        }

        public void AddNode<T>(bool refreshIndex = true) where T : Node, new()
        {
            Node newNode = new T();
            newNode.CalculateInputsPos();
            AddNode(newNode, refreshIndex);
        }

        public void AddNode(INode node, bool refreshIndex = true)
        {
            Nodes.Add(node);

            if(node is OutputNode)
            {
                //
                RecalculateOutput();
            }
            if (refreshIndex)
            {
                OnNodeCountChanged?.Invoke();
                //Task.Run(RecalculateOutput);
            }
        }

        public void DeleteNode(INode nodeToRemove, bool refreshIndex = true)
        {
            DeleteOutputNoodles(nodeToRemove);
            Nodes.Remove(nodeToRemove);
            Task.Run(RecalculateOutput);
            if (refreshIndex)
            {
                OnNodeCountChanged?.Invoke();
            }
        }

        public void DeleteSelectedNode()
        {
            if (SelectedNode != null)
            {
                DeleteNode(SelectedNode, false); 
            }
        }

        private void DeleteOutputNoodles(INode nodeToRemove)
        {
            foreach (var node in Nodes)
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
            if (input is InputProcedural inputProcedural && inputProcedural.InputNode == node)
            {
                inputProcedural.InputNode = null;
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
