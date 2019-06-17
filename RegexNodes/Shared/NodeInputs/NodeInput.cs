using System;

namespace RegexNodes.Shared
{
    public interface INodeInput : IPositionable
    {
        string Title { get; set; }
        Action OnValueChanged { get; set; }
        Func<bool> IsEnabled { get; }
    }

    public abstract class NodeInput : INodeInput
    {
        public string Title { get; set; } = "Input";
        public Action OnValueChanged { get; set; }
        public Vector2L Pos { get; set; }
        public Func<bool> IsEnabled { get; set; } = (() => true);
    }
}
