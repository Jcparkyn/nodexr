
namespace RegexNodes.Shared.NodeInputs
{
    public class InputString : NodeInput
    {
        private string contents;

        public string Contents
        {
            get => contents;
            set
            {
                contents = value;
                OnValueChanged();
            }
        }

        public override int Height => 50;

        public InputString(string contents)
        {
            Contents = contents;
        }
        public string GetValue()
        {
            return Contents;
        }
    }
}
