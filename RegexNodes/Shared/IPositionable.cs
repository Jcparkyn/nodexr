namespace RegexNodes.Shared
{
    public interface IPositionable
    {
        Vector2L Pos { get; set; }
    }

    public class TempNoodleEnd : IPositionable
    {
        //private Vector2L pos = new Vector2L(0,0);
        public Vector2L Pos { get; set; }
    }
}
