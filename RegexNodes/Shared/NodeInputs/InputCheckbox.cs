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

        public InputCheckbox(bool isChecked = false)
        {
            this.isChecked = isChecked;
        }
    }
}
