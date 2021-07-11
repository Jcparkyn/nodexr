namespace Nodexr.Shared.Nodes
{
    using System;
    using Nodexr.Shared.NodeInputs;

    public class InputPort<TValue> : NodeInputBase, INoodleData
        where TValue : class
    {
        private INodeOutput<TValue> connectedNode;

        public INodeOutput<TValue> ConnectedNode
        {
            get => connectedNode;
            set
            {
                if (connectedNode != null)
                {
                    connectedNode.OutputChanged -= OnValueChanged;
                }
                if (value != null)
                {
                    value.OutputChanged += OnValueChanged;
                }
                connectedNode = value;
                OnValueChanged();
            }
        }

        public Vector2 StartPos => connectedNode.OutputPos;

        public Vector2 EndPos => Pos;

        public bool Connected => connectedNode != null && Enabled();

        public override int Height => 32;

        public event EventHandler NoodleChanged;

        public bool IsConnected => connectedNode != null;

        public TValue Value => ConnectedNode?.CachedOutput;

        /// <summary>
        /// Causes the connected noodle to be re-rendered
        /// </summary>
        public void Refresh()
        {
            NoodleChanged?.Invoke(this, EventArgs.Empty);
        }
    }

    public interface INoodleData
    {
        Vector2 StartPos { get; }
        Vector2 EndPos { get; }
        bool Connected { get; }
        void Refresh();

        event EventHandler NoodleChanged;
    }
}
