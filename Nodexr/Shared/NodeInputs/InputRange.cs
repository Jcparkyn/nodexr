namespace Nodexr.Shared.NodeInputs
{
    public class InputRange : NodeInput
    {
        private int? min;

        public int? Min
        {
            get => min;
            set
            {
                min = value;
                OnValueChanged();
            }
        }

        private int? max;

        public int? Max
        {
            get => max;
            set
            {
                max = value;
                OnValueChanged();
            }
        }

        public int? MinValue { get; set; }

        public override int Height => 50;

        public InputRange(int? min = null, int? max = null, int? minValue = null)
        {
            Min = min;
            Max = max;
            MinValue = minValue;
        }

        public (int?, int?) GetValue()
        {
            return (min, max);
        }
    }
}
