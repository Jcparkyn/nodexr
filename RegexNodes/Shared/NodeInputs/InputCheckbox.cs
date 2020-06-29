namespace RegexNodes.Shared
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

        public override int Height => 19;

        public InputCheckbox(bool isChecked = false)
        {
            this.isChecked = isChecked;
        }
    }
}
