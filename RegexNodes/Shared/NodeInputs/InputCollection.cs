using System;
using System.Collections.Generic;

namespace RegexNodes.Shared
{
    public class InputCollection : NodeInput
    {
        public List<InputProcedural> Inputs { get; private set; }

        public InputCollection(List<InputProcedural> inputs)
        {
            Inputs = inputs;
        }
        public InputCollection(string title, int numInputs = 2)
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
            Inputs.Remove(item);
            //nodeHandler.OnRequireNoodleRefresh?.Invoke();
            OnInputPositionsChanged();
            OnValueChanged();
        }

        public void MoveUp(InputProcedural input)
        {
            //var temp = input;
            int index = Inputs.IndexOf(input);
            if (index > 0)
            {
                Inputs[index] = Inputs[index - 1];
                Inputs[index - 1] = input;
                //(Inputs[index].Pos, Inputs[index - 1].Pos) = (Inputs[index - 1].Pos, Inputs[index].Pos);
                //nodeHandler.OnRequireNoodleRefresh?.Invoke();
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
                //(Inputs[index].Pos, Inputs[index + 1].Pos) = (Inputs[index + 1].Pos, Inputs[index].Pos);
                //nodeHandler.OnRequireNoodleRefresh?.Invoke();
                OnInputPositionsChanged();
                OnValueChanged();
            }
        }
    }
}
