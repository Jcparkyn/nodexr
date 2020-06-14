using System;

namespace RegexNodes.Shared
{
    public interface INodeInput : IPositionable
    {
        string Title { get; set; }
        event EventHandler ValueChanged;
        Func<bool> IsEnabled { get; }
    }

    public abstract class NodeInput : INodeInput
    {
        public string Title { get; set; } = "Input";
        public event EventHandler ValueChanged;
        public Vector2L Pos { get; set; }
        public Func<bool> IsEnabled { get; set; } = (() => true);

        protected virtual void OnValueChanged(object sender, EventArgs e)
        {
            ValueChanged?.Invoke(this, e);
        }

        protected virtual void OnValueChanged()
        {
            OnValueChanged(this, EventArgs.Empty);
        }
    }
}
