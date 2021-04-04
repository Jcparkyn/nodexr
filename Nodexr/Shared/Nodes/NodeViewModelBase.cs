using Nodexr.Shared.NodeInputs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Nodexr.Shared.Nodes
{
    public interface INodeViewModel : IPositionable
    {
        string Title { get; }
        bool IsCollapsed { get; set; }

        IEnumerable<NodeInput> NodeInputs { get; }

        void CalculateInputsPos();

        void OnLayoutChanged(object sender, EventArgs e);
        void OnSelectionChanged(EventArgs e);
        void OnDeselected(EventArgs e);

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

        public IEnumerable<NodeInput> NodeInputs { get; }
        public abstract string Title { get; }

        public abstract Vector2 OutputPos { get; }

        public bool IsCollapsed { get; set; }

        public event EventHandler LayoutChanged;
        public event EventHandler SelectionChanged;

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
                    .Select(prop => prop.GetValue(this) as NodeInput)
                    .ToList();
        }

        public abstract IEnumerable<NodeInput> GetAllInputs();

        /// <summary>
        /// Set the position of each input based on the position of the node.
        /// </summary>
        public abstract void CalculateInputsPos();

        public abstract string CssName { get; }
        public abstract string CssColor { get; }
    }
}
