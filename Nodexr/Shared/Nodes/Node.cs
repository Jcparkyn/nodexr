using Nodexr.Shared.NodeInputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nodexr.Shared.Nodes
{
    public interface INodeOutput
    {
        Vector2 OutputPos { get; }
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

        /// <summary>
        /// The node connected to the 'Previous' input.
        /// </summary>
        INodeOutput PreviousNode { get; set; }

        void CalculateInputsPos();

        /// <summary>
        /// Get the height of the node, in pixels. Disabled inputs do not contribute to the height.
        /// </summary>
        int GetHeight();

        void OnLayoutChanged(object sender, EventArgs e);
        void OnSelectionChanged(EventArgs e);
        void OnDeselected(EventArgs e);

        event EventHandler LayoutChanged;
        event EventHandler SelectionChanged;
    }

    public abstract class Node : INode
    {
        private Vector2 pos;

        public Vector2 Pos
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

        public IEnumerable<NodeInput> NodeInputs { get; }
        public abstract string Title { get; }
        public abstract string NodeInfo { get; }

        public NodeResult CachedOutput { get; private set; }

        public Vector2 OutputPos => Pos + new Vector2(154, 13);

        public bool IsCollapsed { get; set; }

        public event EventHandler OutputChanged;
        public event EventHandler LayoutChanged;
        public event EventHandler SelectionChanged;

        protected virtual void OnOutputChanged(EventArgs e) => OutputChanged?.Invoke(this, e);

        public void OnLayoutChanged(object sender, EventArgs e)
        {
            CalculateInputsPos();
            foreach (var input in this.GetAllInputs().OfType<InputProcedural>())
            {
                input.Refresh();
            }
            LayoutChanged?.Invoke(this, e);
        }

        public void OnSelectionChanged(EventArgs e) => SelectionChanged?.Invoke(this, e);

        public void OnDeselected(EventArgs e) => SelectionChanged?.Invoke(this, e);

        protected void OnInputsChanged(object sender, EventArgs e)
        {
            CachedOutput = GetOutput();
            OnOutputChanged(EventArgs.Empty);
        }

        protected Node()
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
        /// Set the position of each input based on the position of the node.
        /// </summary>
        public void CalculateInputsPos()
        {
            //TODO: refactor using GetHeight() on each input
            Previous.Pos = new Vector2(Pos.x + 2, Pos.y + 13);
            if (IsCollapsed)
            {
                const int startHeight = 13;
                foreach (var input in NodeInputs)
                {
                    switch (input)
                    {
                        case InputProcedural input1:
                            input1.Pos = new Vector2(Pos.x + 2, Pos.y + startHeight);
                            break;
                        case InputCollection input1:
                            input1.Pos = new Vector2(Pos.x + 2, Pos.y + startHeight);
                            foreach (var input2 in input1.Inputs)
                            {
                                input2.Pos = new Vector2(Pos.x + 2, Pos.y + startHeight);
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
                    if (input is InputCollection inputColl)
                    {
                        startHeight += 28;
                        inputColl.Pos = new Vector2(Pos.x, Pos.y + startHeight);
                        foreach (var input2 in inputColl.Inputs)
                        {
                            input2.Pos = new Vector2(Pos.x, Pos.y + startHeight);
                            startHeight += input2.Height;
                        }
                    }
                    else
                    {
                        input.Pos = new Vector2(Pos.x, Pos.y + startHeight);
                        startHeight += input.Height;
                    }
                }
            }
        }

        public int GetHeight()
        {
            const int baseHeight = 28;

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
            if (Previous.Value != null)
            {
                builder.Prepend(Previous.Value);
            }
            return builder.Build();
        }

        protected abstract NodeResultBuilder GetValue();
    }
}
