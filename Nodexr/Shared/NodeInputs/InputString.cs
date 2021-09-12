
using BlazorNodes.Core;

namespace Nodexr.Shared.NodeInputs
{
    public class InputString : NodeInputBase
    {
        private string _value;

        public string Value
        {
            get => _value;
            set
            {
                _value = value;
                OnValueChanged();
            }
        }

        public override int Height => 50;

        public InputString(string contents)
        {
            _value = contents;
        }

        public string GetValue() => Value;
    }
}
