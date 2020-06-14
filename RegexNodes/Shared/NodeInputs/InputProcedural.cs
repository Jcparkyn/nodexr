using System.Threading.Tasks;

namespace RegexNodes.Shared
{
    public class InputProcedural : NodeInput
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
                value.OutputChanged += OnValueChanged;
                connectedNode = value;
                OnValueChanged();
            }
        }

        public string GetValue()
        {
            return ConnectedNode?.GetOutput() ?? "";
        }
    }
}
