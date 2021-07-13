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
        event EventHandler OutputChanged;
    }

    public interface INodeOutput<TOutput> : INodeOutput
    {
        TOutput CachedOutput { get; }
    }

    public interface INodeViewModel : INodeOutput
    {
        string NodeInfo { get; }
        string Title { get; }
        bool IsCollapsed { get; set; }
        Vector2 Pos { get; set; }

        IEnumerable<INodeInput> NodeInputs { get; }

        string OutputTooltip { get; }
        IInputPort PrimaryInput { get; }

        void CalculateInputsPos();

        void OnLayoutChanged(object sender, EventArgs e);
        void OnSelectionChanged(EventArgs e);
        void OnDeselected(EventArgs e);
        IEnumerable<INodeInput> GetAllInputs();

        event EventHandler LayoutChanged;
        event EventHandler SelectionChanged;
    }

    public abstract class NodeViewModelBase : INodeViewModel
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

        public IEnumerable<INodeInput> NodeInputs { get; }
        public abstract IInputPort PrimaryInput { get; }
        public abstract string Title { get; }
        public abstract string OutputTooltip { get; }

        public abstract Vector2 OutputPos { get; }

        public bool IsCollapsed { get; set; }

        public event EventHandler LayoutChanged;
        public event EventHandler SelectionChanged;
        public abstract event EventHandler OutputChanged;

        public void OnLayoutChanged(object sender, EventArgs e)
        {
            CalculateInputsPos();
            foreach (var input in GetAllInputs().OfType<InputProcedural>())
            {
                input.Refresh();
            }
            LayoutChanged?.Invoke(this, e);
        }

        public void OnSelectionChanged(EventArgs e) => SelectionChanged?.Invoke(this, e);

        public void OnDeselected(EventArgs e) => SelectionChanged?.Invoke(this, e);

        protected NodeViewModelBase()
        {
            var inputProperties = GetType()
                .GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance)
                .Where(prop => Attribute.IsDefined(prop, typeof(NodeInputAttribute)));

            NodeInputs = inputProperties
                    .Select(prop => prop.GetValue(this) as INodeInput)
                    .ToList();
        }

        public abstract IEnumerable<INodeInput> GetAllInputs();

        /// <summary>
        /// Set the position of each input based on the position of the node.
        /// </summary>
        public abstract void CalculateInputsPos();

        public abstract string CssName { get; }
        public abstract string CssColor { get; }

        public abstract string NodeInfo { get; }
    }

    public abstract class NodeViewModelBase<TOutput> : NodeViewModelBase, INodeOutput<TOutput>
    {
        public abstract TOutput CachedOutput { get; }
    }
}
