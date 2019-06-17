namespace RegexNodes.Shared
{
    public class InputNumber : NodeInput
    {
        private int? inputContents;
        public int? InputContents
        {
            get => inputContents;
            set
            {
                inputContents = value;
                OnValueChanged?.Invoke();
            }
        }

        public int? Min { get; set; }
        public int? Max { get; set; }

        public InputNumber(int contents, int? min = null, int? max = null)
        {
            InputContents = contents;
            Min = min;
            Max = max;
        }

        public int? GetValue()
        {
            return InputContents;
        }
    }
}
