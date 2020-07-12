using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;
using System.Linq;
using RegexNodes.Shared.Nodes;
using RegexNodes.Shared.NodeInputs;

namespace RegexNodes.Shared.Services
{
    public interface INodeDragService
    {
        INode NodeToDrag { get; set; }
        void OnStartNodeDrag(INode nodeToDrag, DragEventArgs e);
        void OnDrop(DragEventArgs e);
        Task OnStartCreateNodeDrag(INode nodeToDrag, DragEventArgs e);
        void CancelDrag();
    }

    public class NodeDragService : INodeDragService
    {
        readonly INodeHandler nodeHandler;
        readonly IJSRuntime jsRuntime;
        public NodeDragService(INodeHandler nodeHandler, IJSRuntime jsRuntime)
        {
            this.nodeHandler = nodeHandler;
            this.jsRuntime = jsRuntime;
        }

        public INode NodeToDrag { get; set; }

        Vector2L cursorStartPos;

        public void OnStartNodeDrag(INode nodeToDrag, DragEventArgs e)
        {
            NodeToDrag = nodeToDrag;
            cursorStartPos = e.GetClientPos();
        }

        public async Task OnStartCreateNodeDrag(INode nodeToDrag, DragEventArgs e)
        {
            NodeToDrag = nodeToDrag;
            cursorStartPos = e.GetClientPos();
            var scaledPos = await jsRuntime.InvokeAsync<float[]>("panzoom.clientToGraphPos", e.ClientX, e.ClientY);
            int x = (int)scaledPos[0];
            int y = (int)scaledPos[1];
            
            NodeToDrag.Pos = new Vector2L(x - 75, y - 15);
        }

        public void OnDrop(DragEventArgs e)
        {
            if (NodeToDrag != null)
            {
                NodeToDrag.Pos += (e.GetClientPos() - cursorStartPos) / ZoomHandler.Zoom;
                if (!nodeHandler.Tree.Nodes.Contains(NodeToDrag))
                {
                    nodeHandler.Tree.AddNode(NodeToDrag);
                }
                NodeToDrag = null;
            }
        }

        public void CancelDrag()
        {
            NodeToDrag = null;
        }
    }
}
