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

        /// <summary>
        /// The node connected to the 'Previous' input.
        /// </summary>
        INodeOutput PreviousNode { get; set; }

        void CalculateInputsPos();

        /// <summary>
        /// Get the height of the node, in pixels. Disabled inputs do not contribute to the height.
        /// </summary>
        int GetHeight();

        /// <summary>
        /// Get all of the inputs to the node, including the 'previous' input and the sub-inputs of any InputCollections.
        /// InputCollections themselves are not returned.
        /// </summary>
        IEnumerable<NodeInput> GetAllInputs();
        void OnLayoutChanged(object sender, EventArgs e);
        bool IsDependentOn(INodeInput childInput);
        void OnSelected(EventArgs e);
        void OnDeselected(EventArgs e);

        event EventHandler LayoutChanged;
        event EventHandler Selected;
        event EventHandler Deselected;
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

        public IEnumerable<NodeInput> NodeInputs { get; }
        public abstract string Title { get; }
        public abstract string NodeInfo { get; }

        public NodeResult CachedOutput { get; private set; }

        public Vector2L OutputPos => Pos + new Vector2L(150, 14);

        public bool IsCollapsed { get; set; }

        public event EventHandler OutputChanged;
        public event EventHandler LayoutChanged;
        public event EventHandler Selected;
        public event EventHandler Deselected;

        protected virtual void OnOutputChanged(EventArgs e) => OutputChanged?.Invoke(this, e);

        public void OnLayoutChanged(object sender, EventArgs e)
        {
            CalculateInputsPos();
            foreach(var input in GetAllInputs().OfType<InputProcedural>())
            {
                input.Refresh();
            }
            LayoutChanged?.Invoke(this, e);
        }

        public void OnSelected(EventArgs e) => Selected?.Invoke(this, e);

        public void OnDeselected(EventArgs e) => Deselected?.Invoke(this, e);

        protected void OnInputsChanged(object sender, EventArgs e)
        {
            CachedOutput = GetOutput();
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
        /// Set the position of each input based on the position of the node.
        /// </summary>
        public void CalculateInputsPos()
        {
            //TODO: refactor using GetHeight() on each input
            Previous.Pos = new Vector2L(Pos.x + 2, Pos.y + 13);
            if (IsCollapsed)
            {
                const int startHeight = 13;
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
                            input.Pos = new Vector2L(Pos.x, Pos.y + startHeight);
                            startHeight += input.Height;
                            break;
                    }
                }
            }
        }

        /// <inheritdoc/>
        public IEnumerable<NodeInput> GetAllInputs()
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

        public bool IsDependentOn(INodeInput childInput)
        {
            return GetAllProceduralInputsRecursive(this).Any(input => input == childInput);

            static IEnumerable<InputProcedural> GetAllProceduralInputsRecursive(INode parent)
            {
                foreach(var input in parent.GetAllInputs().OfType<InputProcedural>())
                {
                    yield return input;

                    if (input.ConnectedNode is INode childNode)
                    {
                        foreach (var input2 in GetAllProceduralInputsRecursive(childNode))
                            yield return input2;
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
            if(Previous.Value != null)
            {
                builder.Prepend(Previous.Value);
            }
            return builder.Build();
        }

        protected abstract NodeResultBuilder GetValue();
    }
}
