namespace Nodexr.RegexParsers;
using Pidgin;
using Nodexr.NodeTypes;
using static Pidgin.Parser;

public static class AnchorParser
{
    public static Parser<char, AnchorNode> ParseAnchor =>
        OneOf(
            Char('^').Select(_ => CreateWithType(AnchorNode.Mode.StartLine)),
            Char('$').Select(_ => CreateWithType(AnchorNode.Mode.EndLine))
            );

    public static Parser<char, AnchorNode> ParseAnchorAfterEscape =>
        Char('b').Select(_ => CreateWithType(AnchorNode.Mode.WordBoundary))
        .Or(Char('B').Select(_ => CreateWithType(AnchorNode.Mode.NotWordBoundary)));

    private static AnchorNode CreateWithType(AnchorNode.Mode type)
    {
        var node = new AnchorNode();
        node.InputAnchorType.Value = type;
        return node;
    }
}
