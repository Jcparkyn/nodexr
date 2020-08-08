using System;

namespace Nodexr.Shared.NodeInputs
{
    public interface INodeInput : IPositionable
    {
        string Title { get; set; }
        event EventHandler ValueChanged;
        Func<bool> IsEnabled { get; }
        string Description { get; set; }
    }

    public abstract class NodeInput : INodeInput
    {
        public string Title { get; set; }

        /// <summary>
        /// The description for this input. Displayed as a tooltip for most types of inputs.
        /// </summary>
        public string Description { get; set; }

        public event EventHandler ValueChanged;

        public Vector2 Pos { get; set; }

        public Func<bool> IsEnabled { get; set; } = (() => true);

        public virtual int Height { get; } = 32;

        /// <summary>
        /// Determines whether a change from this input should trigger a
        /// re-render of noodles connected to the parent node.
        /// </summary>
        public virtual bool AffectsLayout { get; set; } = false;

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
