using System.Threading.Tasks;

namespace RegexNodes.Shared
{
    public class InputProcedural : NodeInput
    {
        private INodeOutput inputNode;
        public INodeOutput InputNode
        {
            get => inputNode;
            set
            {
                inputNode = value;
                OnValueChanged?.Invoke();
            }
        }

        public string GetValue()
        {
            return InputNode?.GetOutput() ?? "";
        }
    }
}
