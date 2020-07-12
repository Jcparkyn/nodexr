using System;
using System.Threading.Tasks;
using Nodexr.Shared.Nodes;

namespace Nodexr.Shared.NodeInputs
{
    public class InputProcedural : NodeInput, INoodleData
    {
        private INodeOutput connectedNode;
        public INodeOutput ConnectedNode
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

        public Vector2L StartPos => connectedNode.OutputPos;

        public Vector2L EndPos => Pos;

        public bool Enabled => connectedNode != null;

        public override int Height => 32;

        public event EventHandler NoodleChanged;

        public string GetValue()
        {
            return ConnectedNode?.CachedOutput.Expression ?? "";
        }

        public bool IsConnected => connectedNode != null;

        public NodeResult Value => ConnectedNode?.CachedOutput;

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
        Vector2L StartPos { get; }
        Vector2L EndPos { get; }
        bool Enabled { get; }
        void Refresh();

        event EventHandler NoodleChanged;
    }
}
