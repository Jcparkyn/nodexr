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
                max = value > 0 ? value : null;
                OnValueChanged();
            }
        }

        public int? MinValue { get; set; } = null;

        public bool AutoClearMax { get; set; } = false;

        public override int Height => 50;

        public InputRange(int? min = null, int? max = null)
        {
            Min = min;
            Max = max;
        }

        public (int?, int?) GetValue()
        {
            return (min, max);
        }
    }
}
