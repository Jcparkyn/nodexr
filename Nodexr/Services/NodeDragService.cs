namespace Nodexr.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Nodexr.Utils;
using BlazorNodes.Core;
using Nodexr.NodeInputs;

public interface INodeDragService
{
    void OnStartNodeDrag(INodeViewModel nodeToDrag, MouseEventArgs e);
    void OnDrop(MouseEventArgs e);
    Task OnStartCreateNodeDrag(INodeViewModel nodeToDrag, DragEventArgs e);
    void CancelDrag();
    void DragNode(double posX, double posY);
}

public class NodeDragService : INodeDragService
{
    private readonly INodeHandler nodeHandler;
    private readonly IJSRuntime jsRuntime;

    private INodeViewModel? nodeToDrag;
    private List<InputProcedural>? nodeToDragOutputs;

    private Vector2 cursorStartPos;
    private Vector2 lastCursorPos;

    private bool isDraggingNewNode;

    public NodeDragService(INodeHandler nodeHandler, IJSRuntime jsRuntime)
    {
        this.nodeHandler = nodeHandler;
        this.jsRuntime = jsRuntime;
    }

    public void OnStartNodeDrag(INodeViewModel nodeToDrag, MouseEventArgs e)
    {
        this.nodeToDrag = nodeToDrag;

        var selectedNodes = nodeHandler.Tree.GetSelectedNodes().ToList();
        nodeToDragOutputs = nodeHandler.Tree.Nodes
            .SelectMany(node => node.GetAllInputs()
                .OfType<InputProcedural>()
                .Where(input => selectedNodes.Contains(input.ConnectedNode as INodeOutput)))
            .ToList();

        cursorStartPos = lastCursorPos = e.GetClientPos();
    }

    public async Task OnStartCreateNodeDrag(INodeViewModel nodeToDrag, DragEventArgs e)
    {
        nodeToDragOutputs = null;
        this.nodeToDrag = nodeToDrag;
        isDraggingNewNode = true;
        cursorStartPos = lastCursorPos = e.GetClientPos();
        float[] scaledPos = await jsRuntime.InvokeAsync<float[]>("panzoom.clientToGraphPos", e.ClientX, e.ClientY)
            .ConfigureAwait(false);
        int x = (int)scaledPos[0];
        int y = (int)scaledPos[1];

        this.nodeToDrag.Pos = new Vector2(x - 75, y - 15);
    }

    public void DragNode(double posX, double posY)
    {
        if (nodeToDrag is null)
            return;

        var newCursorPos = new Vector2(posX, posY);
        var dragOffset = (newCursorPos - lastCursorPos) / ZoomHandler.Zoom;
        lastCursorPos = new Vector2(posX, posY);

        var nodesToMove = nodeHandler.Tree.GetSelectedNodes();
        foreach (var node in nodesToMove)
        {
            node.Pos += dragOffset;
            node.OnLayoutChanged(this, EventArgs.Empty);
        }
        var inputsToUpdate = nodesToMove.SelectMany(n => n.GetAllInputs().OfType<InputProcedural>());
        foreach (var input in inputsToUpdate)
        {
            input.Refresh();
        }

        nodeToDragOutputs?.ForEach(input => input.Refresh());
    }

    public void OnDrop(MouseEventArgs e)
    {
        //Console.WriteLine("Dropping node");
        if (nodeToDrag != null)
        {
            //TODO: Refactor this
            if (isDraggingNewNode)
            {
                isDraggingNewNode = false;
                nodeToDrag.Pos += (e.GetClientPos() - cursorStartPos) / ZoomHandler.Zoom;
                nodeHandler.Tree.AddNode(nodeToDrag);
            }
            nodeToDrag = null;
        }
    }

    public void CancelDrag()
    {
        nodeToDrag = null;
        isDraggingNewNode = false;
    }
}
