
namespace RegexNodes.Shared
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
