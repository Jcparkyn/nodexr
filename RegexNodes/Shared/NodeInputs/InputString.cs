
namespace RegexNodes.Shared
{
    public class InputString : NodeInput
    {
        private string inputContents;

        public string InputContents
        {
            get => inputContents;
            set
            {
                inputContents = value;
                OnValueChanged();
            }
        }

        public InputString(string contents)
        {
            InputContents = contents;
        }
        public string GetValue()
        {
            return InputContents;
        }
    }
}
