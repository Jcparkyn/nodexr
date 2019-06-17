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
        public InputCollection(int numInputs = 2, string inputTitle = "Input")
        {
            Inputs = new List<InputProcedural>();
            for(int i = 0; i < numInputs; i++)
            {
                Inputs.Add(new InputProcedural() { Title = inputTitle});
            }
        }

        public void AddItem()
        {
            var newInput = new InputProcedural();
            //newInput.Pos = new Vector2L(Pos.x, Pos.y + 35 * Inputs.Count); //TODO: refactor
            Inputs.Add(newInput);
            //nodeHandler.OnRequireNoodleRefresh?.Invoke();
            OnValueChanged?.Invoke();
        }

        public void RemoveItem(InputProcedural item)
        {
            Inputs.Remove(item);
            //nodeHandler.OnRequireNoodleRefresh?.Invoke();
            OnValueChanged?.Invoke();
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
                OnValueChanged?.Invoke();
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
                OnValueChanged?.Invoke();
            }
        }
    }
}
