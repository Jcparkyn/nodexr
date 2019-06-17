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
                OnValueChanged?.Invoke();
            }
        }

        public InputCheckbox(bool contents)
        {
            IsChecked = contents;
        }
    }
}
