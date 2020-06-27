using System;
using System.Collections.Generic;

namespace RegexNodes.Shared
{
    public class InputCollection : NodeInput
    {
        public List<InputProcedural> Inputs { get; }

        public InputCollection(string title, int numInputs = 0)
        {
            Title = title;
            Inputs = new List<InputProcedural>(numInputs);
            for(int i = 0; i < numInputs; i++)
            {
                AddItem();
            }
        }

        public event EventHandler InputPositionsChanged;

        private void OnInputPositionsChanged()
        {
            InputPositionsChanged?.Invoke(this, EventArgs.Empty);
        }

        public void AddItem()
        {
            var newInput = new InputProcedural() { Title = this.Title };
            newInput.ValueChanged += OnValueChanged;
            Inputs.Add(newInput);
            OnInputPositionsChanged();
            OnValueChanged();
        }

        public void RemoveItem(InputProcedural item)
        {
            item.ValueChanged -= OnValueChanged;
            Inputs.Remove(item);
            OnInputPositionsChanged();
            OnValueChanged();
        }

        public void RemoveAll()
        {
            foreach(var input in Inputs)
            {
                input.ValueChanged -= OnValueChanged;
            }
            Inputs.Clear();
            OnInputPositionsChanged();
            OnValueChanged();
        }

        public void MoveUp(InputProcedural input)
        {
            int index = Inputs.IndexOf(input);
            if (index > 0)
            {
                Inputs[index] = Inputs[index - 1];
                Inputs[index - 1] = input;
                OnInputPositionsChanged();
                OnValueChanged();
            }
        }

        public void MoveDown(InputProcedural input)
        {
            int index = Inputs.IndexOf(input);
            if (index < Inputs.Count - 1)
            {
                Inputs[index] = Inputs[index + 1];
                Inputs[index + 1] = input;
                OnInputPositionsChanged();
                OnValueChanged();
            }
        }
    }
}
