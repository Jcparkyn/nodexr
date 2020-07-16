namespace Nodexr.Shared.NodeInputs
{
    public class InputCheckbox : NodeInput
    {
        private bool isChecked;

        public bool IsChecked
        {
            get => isChecked;
            set
            {
                isChecked = value;
                OnValueChanged();
            }
        }

        public override int Height => 23;

        public InputCheckbox(bool isChecked = false)
        {
            this.isChecked = isChecked;
        }
    }
}
