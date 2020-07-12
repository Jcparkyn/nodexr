using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;
using Nodexr.Shared.NodeInputs;
using Nodexr.Shared;


namespace Nodexr.Shared.Nodes
{
    public interface INodeOutput
    {
        Vector2L OutputPos { get; }
        string CssName { get; }
        string CssColor { get; }
        NodeResult CachedOutput { get; }

        event EventHandler OutputChanged;
    }

    public interface INode : IPositionable, INodeOutput
    {
        string Title { get; }
        string NodeInfo { get; }
        bool IsCollapsed { get; set; }

        IEnumerable<NodeInput> NodeInputs { get; }
        InputProcedural Previous { get; }
        INodeOutput PreviousNode { get; set; }

        void CalculateInputsPos();
        int GetHeight();
        IEnumerable<NodeInput> GetInputsRecursive();
        void OnLayoutChanged(object sender, EventArgs e);
        event EventHandler LayoutChanged;
    }

    public abstract class Node : INode
    {
        private Vector2L pos;

        public Vector2L Pos
        {
            get => pos;
            set
            {
                pos = value;
                CalculateInputsPos();
            }
        }
        public InputProcedural Previous { get; } = new InputProcedural();
        public INodeOutput PreviousNode
        {
            get => Previous.ConnectedNode;
            set => Previous.ConnectedNode = value;
        }

        public IEnumerable<NodeInput> NodeInputs { get; private set; }
        public abstract string Title { get; }
        public abstract string NodeInfo { get; }

        public NodeResult CachedOutput { get; private set; }

        public Vector2L OutputPos => Pos + new Vector2L(150, 14);

        public bool IsCollapsed { get; set; }

        public event EventHandler OutputChanged;
        public event EventHandler LayoutChanged;

        protected virtual void OnOutputChanged(EventArgs e)
        {
            OutputChanged?.Invoke(this, e);
        }

        public void OnLayoutChanged(object sender, EventArgs e)
        {
            CalculateInputsPos();
            foreach(var input in GetInputsRecursive().OfType<InputProcedural>())
            {
                input.Refresh();
            }
            LayoutChanged?.Invoke(this, e);
        }

        protected void OnInputsChanged(object sender, EventArgs e)
        {
            var newOutput = GetOutput();
            CachedOutput = newOutput;
            OnOutputChanged(EventArgs.Empty);
        }

        public Node()
        {
            var inputProperties = GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(prop => Attribute.IsDefined(prop, typeof(NodeInputAttribute)));

            NodeInputs = inputProperties
                    .Select(prop => prop.GetValue(this) as NodeInput)
                    .ToList();

            Previous.ValueChanged += OnInputsChanged;

            foreach (var input in NodeInputs)
            {
                input.ValueChanged += OnInputsChanged;
                if (input is InputCollection inputColl)
                {
                    inputColl.InputPositionsChanged += OnLayoutChanged;
                }
            }

            OnInputsChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Set the position of each input based on the position of the node
        /// </summary>
        public void CalculateInputsPos()
        {
            //TODO: refactor using GetHeight() on each input
            Previous.Pos = new Vector2L(Pos.x + 2, Pos.y + 13);
            if (IsCollapsed)
            {
                int startHeight = 13;
                foreach (var input in NodeInputs)
                {
                    switch (input)
                    {
                        case InputProcedural input1:
                            input1.Pos = new Vector2L(Pos.x + 2, Pos.y + startHeight);
                            break;
                        case InputCollection input1:
                            input1.Pos = new Vector2L(Pos.x + 2, Pos.y + startHeight);
                            foreach (var input2 in input1.Inputs)
                            {
                                input2.Pos = new Vector2L(Pos.x + 2, Pos.y + startHeight);
                            }
                            break;
                    }
                }
            }
            else
            {
                int startHeight = 44;
                //TODO: Support disabled inputs
                foreach (var input in NodeInputs)
                {
                    switch (input)
                    {
                        case InputProcedural inputProc:
                            inputProc.Pos = new Vector2L(Pos.x, Pos.y + startHeight);
                            startHeight += inputProc.Height;
                            break;
                        case InputCollection inputColl:
                            startHeight += 28;
                            inputColl.Pos = new Vector2L(Pos.x, Pos.y + startHeight);
                            foreach (var input2 in inputColl.Inputs)
                            {
                                input2.Pos = new Vector2L(Pos.x, Pos.y + startHeight);
                                startHeight += input2.Height;
                            }
                            break;
                        default:
                            startHeight += 50;
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// Get all of the inputs to the node, including the 'previous' input and the sub-inputs of any InputCollections.
        /// InputCollections themselves are not returned.
        /// </summary>
        public IEnumerable<NodeInput> GetInputsRecursive()
        {
            yield return Previous;
            foreach(var input in NodeInputs)
            {
                if(input is InputCollection coll)
                {
                    foreach (var subInput in coll.Inputs)
                        yield return subInput;
                }
                else
                {
                    yield return input;
                }
            }
        }

        public int GetHeight()
        {
            int baseHeight = 28;

            int inputHeight = NodeInputs
                .Where(input => input.IsEnabled())
                .Select(input => input.Height)
                .Sum();

            return baseHeight + inputHeight;
        }

        public string CssName => Title.Replace(" ", "").ToLowerInvariant();
        public string CssColor => $"var(--col-node-{CssName})";

        protected virtual NodeResult GetOutput()
        {
            var builder = GetValue() ?? new NodeResultBuilder();
            if(Previous.Value != null)
            {
                builder.Prepend(Previous.Value);
            }
            return builder.Build();
        }

        protected abstract NodeResultBuilder GetValue();
    }
}
