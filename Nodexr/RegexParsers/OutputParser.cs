﻿namespace Nodexr.RegexParsers;
using Pidgin;
using Nodexr.NodeTypes;

public static class OutputParser
{
    public static Parser<char, OutputNode> ParseOutputNode =>
        RegexParser.ParseRegex
        .Select(contents => AttachOutputToContents(new OutputNode(), contents));

    private static OutputNode AttachOutputToContents(OutputNode output, RegexNodeViewModelBase? contents)
    {
        if (contents is null)
        {
            output.PreviousNode = new TextNode();
            return output;
        }

        output.PreviousNode = contents;

        if (contents is AnchorNode anchorEnd)
        {
            switch (anchorEnd.InputAnchorType.Value)
            {
                case AnchorNode.Mode.EndLine:
                    output.InputEndsAt.Value = OutputNode.Mode.EndLine;
                    output.PreviousNode = contents.PreviousNode;
                    contents.PreviousNode = null;
                    break;
                case AnchorNode.Mode.WordBoundary:
                    output.InputEndsAt.Value = OutputNode.Mode.WordBound;
                    output.PreviousNode = contents.PreviousNode;
                    contents.PreviousNode = null;
                    break;
            }
        }

        var previousNodes = GetPreviousNodes(output).ToList();
        if (previousNodes.LastOrDefault() is AnchorNode anchorStart)
        {
            var anchorParent = previousNodes[^2];
            switch (anchorStart.InputAnchorType.Value)
            {
                case AnchorNode.Mode.StartLine:
                    output.InputStartsAt.Value = OutputNode.Mode.StartLine;
                    anchorParent.PreviousNode = null;
                    break;
                case AnchorNode.Mode.WordBoundary:
                    output.InputStartsAt.Value = OutputNode.Mode.WordBound;
                    anchorParent.PreviousNode = null;
                    break;
            }
        }
        return output;
    }

    private static IEnumerable<RegexNodeViewModelBase> GetPreviousNodes(RegexNodeViewModelBase parent)
    {
        var currentNode = parent;
        while (currentNode.PreviousNode is RegexNodeViewModelBase previous)
        {
            yield return previous;
            currentNode = previous;
        }
    }
}
