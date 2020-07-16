using System;

namespace Nodexr.Shared.NodeInputs
{
    public interface INodeInput : IPositionable
    {
        string Title { get; set; }
        event EventHandler ValueChanged;
        Func<bool> IsEnabled { get; }
        string Description { get; }
        Func<string> DescriptionFunc { get; set; }
    }

    public abstract class NodeInput : INodeInput
    {
        public string Title { get; set; }

        public string Description => DescriptionFunc?.Invoke();

        /// <summary>
        /// A function that returns the description for this input.
        /// </summary>
        public Func<string> DescriptionFunc { get; set; }

        public event EventHandler ValueChanged;

        public Vector2L Pos { get; set; }

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
