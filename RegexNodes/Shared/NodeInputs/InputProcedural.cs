using System.Threading.Tasks;

namespace RegexNodes.Shared
{
    public class InputProcedural : NodeInput
    {
        private INode inputNode;
        public INode InputNode
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
