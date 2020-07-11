# Nodexr
Nodexr is a node-based Regular Expression editor, created in C# and Blazor.
__Nodexr is published [here](https://www.nodexr.net).__

Currently it is based on the .NET Regex flavour, although there is potential to allow the user to easily switch between flavours while using the same nodes.

This screenshot shows a Regular Expression used to match floating point numbers, with or without an exponent:
![Screenshot](https://github.com/Jcparkyn/nodexr/blob/dev/RegexNodes/images/Screenshot_floatingPoint_2.png?raw=true)

## How To Use
Drag-and-drop nodes from the left panel to insert them into the main window. The final result/output of your nodes must be connected to the _Output_ node and is displayed at the top left.

The main concept is that the "nesting" behaviour of regex is expressed by connecting one node to the input of another, but items in sequence are connected using the *Previous* input at the top left of each node. Expressions can alternatively be connected in sequence (concatenated) using the *Concatenate* node.
The output expression will be empty unless one or more nodes are connected to the _Output_ node.

## Nodes
Information about each node can be found by clicking the **(i)** button next to its title.

To use any Regex functionality that cannot be implemented with the provided nodes, create a *Text* node with 'escape' disabled to input parts of the expression manually.

## Replacement
Use the bottom 3 panels to test a string for searching and/or replacement. Any valid Regex replacement string can be used here, including named and/or numbered group references. The bottom right panel shows the result after replacement.
