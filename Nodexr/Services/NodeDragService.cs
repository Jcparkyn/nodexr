namespace Nodexr.Services;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Nodexr.Utils;
using BlazorNodes.Core;

public interface INodeDragService
{
    void OnDrop(MouseEventArgs e);
    Task OnStartCreateNodeDrag(INodeViewModel nodeToDrag, DragEventArgs e);
    void CancelDrag();
}

public class NodeDragService(INodeHandler nodeHandler, IJSRuntime jsRuntime) : INodeDragService
{
    private INodeViewModel? nodeToDrag;
    private Vector2 cursorStartPos;

    public async Task OnStartCreateNodeDrag(INodeViewModel nodeToDrag, DragEventArgs e)
    {
        this.nodeToDrag = nodeToDrag;
        cursorStartPos = e.GetClientPos();
        float[] scaledPos = await jsRuntime.InvokeAsync<float[]>("panzoom.clientToGraphPos", e.ClientX, e.ClientY)
            .ConfigureAwait(false);
        int x = (int)scaledPos[0];
        int y = (int)scaledPos[1];

        this.nodeToDrag.Pos = new Vector2(x - 75, y - 15);
    }

    public void OnDrop(MouseEventArgs e)
    {
        if (nodeToDrag == null)
        {
            return;
        }
        //TODO: Refactor this to remove dependency on ZoomHandler
        nodeToDrag.Pos += (e.GetClientPos() - cursorStartPos) / ZoomHandler.Zoom;
        nodeHandler.Tree.AddNode(nodeToDrag);
        nodeToDrag = null;
    }

    public void CancelDrag()
    {
        nodeToDrag = null;
    }
}
