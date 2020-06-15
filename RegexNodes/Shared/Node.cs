using System;
using System.Linq;
using System.Reflection;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RegexNodes.Shared
{
    public interface INodeOutput
    {
        Vector2L OutputPos { get; }
        string CssName { get; }
        string CssColor { get; }
        string CachedOutput { get; }

        string GetOutput();

        event EventHandler OutputChanged;
    }

    public interface INode : IPositionable, INodeOutput
    {
        string Title { get; }
        string NodeInfo { get; }
        bool IsCollapsed { get; set; }

        List<NodeInput> NodeInputs { get; }
        InputProcedural PreviousNode { get; }

        void CalculateInputsPos();
        IEnumerable<NodeInput> GetInputsRecursive();

        //event EventHandler LayoutChanged;
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
        public InputProcedural PreviousNode { get; } = new InputProcedural();

        public virtual List<NodeInput> NodeInputs { get; private set; }
        public abstract string Title { get; }
        public abstract string NodeInfo { get; }

        public string CachedOutput { get; private set; }

        public Vector2L OutputPos => Pos + new Vector2L(150, 14);

        public bool IsCollapsed { get; set; }

        public event EventHandler OutputChanged;
        //public event EventHandler LayoutChanged;

        protected virtual void OnOutputChanged(EventArgs e)
        {
            Console.WriteLine("Initiating update: " + Title);
            OutputChanged?.Invoke(this, e);
        }

        protected virtual void OnLayoutChanged(object sender, EventArgs e)
        {
            //LayoutChanged?.Invoke(this, e);
            CalculateInputsPos();
            foreach(var input in GetInputsRecursive().OfType<InputProcedural>())
            {
                input.Refresh();
            }
            //Console.WriteLine("Node layout changed");
        }

        protected virtual void OnInputsChanged(object sender, EventArgs e)
        {
            var newOutput = GetOutput();
            if (newOutput != CachedOutput)
            {
                CachedOutput = newOutput;
                OnOutputChanged(EventArgs.Empty);
            }
        }

        public Node()
        {
            var inputProperties = GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(prop => Attribute.IsDefined(prop, typeof(NodeInputAttribute)));

            NodeInputs = inputProperties
                    .Select(prop => prop.GetValue(this) as NodeInput)
                    .ToList();

            PreviousNode.ValueChanged += OnInputsChanged;

            foreach (var input in NodeInputs)
            {
                input.ValueChanged += OnInputsChanged;
                if (input.AffectsLayout)
                {
                    input.ValueChanged += OnLayoutChanged;
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
            PreviousNode.Pos = new Vector2L(Pos.x + 2, Pos.y + 13);
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
                int inputHeight = 32;
                //TODO: Support disabled inputs
                foreach (var input in NodeInputs)
                {
                    switch (input)
                    {
                        case InputProcedural input1:
                            input1.Pos = new Vector2L(Pos.x, Pos.y + startHeight);
                            startHeight += inputHeight;
                            break;
                        case InputCollection input1:
                            startHeight += 28;
                            input1.Pos = new Vector2L(Pos.x, Pos.y + startHeight);
                            foreach (var input2 in input1.Inputs)
                            {
                                input2.Pos = new Vector2L(Pos.x, Pos.y + startHeight);
                                startHeight += inputHeight;
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
            yield return PreviousNode;
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

        public string CssName => Title.Replace(" ", "").ToLowerInvariant();
        public string CssColor => $"var(--col-node-{CssName})";

        public virtual string GetOutput()
        {
            return PreviousNode.ConnectedNode?.CachedOutput + GetValue();
        }

        protected abstract string GetValue();
    }
}
