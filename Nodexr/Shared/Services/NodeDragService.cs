using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using System;
using System.Threading.Tasks;
using System.Linq;
using Nodexr.Shared.Nodes;
using Nodexr.Shared.NodeInputs;
using System.Collections.Generic;

namespace Nodexr.Shared.Services
{
    public interface INodeDragService
    {
        void OnStartNodeDrag(INode nodeToDrag, MouseEventArgs e);
        void OnDrop(MouseEventArgs e);
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
            jsRuntime.InvokeVoidAsync("addDotNetSingletonService", "DotNetNodeDragService", DotNetObjectReference.Create(this));
        }

        private INode nodeToDrag;
        private List<InputProcedural> nodeToDragOutputs;

        private Vector2L cursorStartPos;
        private Vector2L nodeStartPos;

        public void OnStartNodeDrag(INode nodeToDrag, MouseEventArgs e)
        {
            this.nodeToDrag = nodeToDrag;

            this.nodeToDragOutputs = nodeHandler.Tree.Nodes
                .SelectMany(node => node.GetAllInputs()
                    .OfType<InputProcedural>()
                    .Where(input => input.ConnectedNode == nodeToDrag)).ToList();

            cursorStartPos = e.GetClientPos();
            nodeStartPos = nodeToDrag.Pos;
        }

        public async Task OnStartCreateNodeDrag(INode nodeToDrag, DragEventArgs e)
        {
            this.nodeToDrag = nodeToDrag;
            cursorStartPos = e.GetClientPos();
            var scaledPos = await jsRuntime.InvokeAsync<float[]>("panzoom.clientToGraphPos", e.ClientX, e.ClientY);
            int x = (int)scaledPos[0];
            int y = (int)scaledPos[1];
            
            this.nodeToDrag.Pos = new Vector2L(x - 75, y - 15);
        }

        [JSInvokable]
        public void DragNode(double posX, double posY)
        {
            var dragOffset = (new Vector2L((long)posX, (long)posY) - cursorStartPos) / ZoomHandler.Zoom;
            nodeToDrag.Pos = nodeStartPos + dragOffset;
            nodeToDrag.OnLayoutChanged(this, EventArgs.Empty);
            foreach (var input in nodeToDrag.GetAllInputs().OfType<InputProcedural>())
            {
                input.Refresh();
            }

            foreach (var input in nodeToDragOutputs)
            {
                input.Refresh();
            }
        }

        public void OnDrop(MouseEventArgs e)
        {
            //Console.WriteLine("Dropping node");
            if (nodeToDrag != null)
            {
                //TODO: Refactor this
                if (!nodeHandler.Tree.Nodes.Contains(nodeToDrag))
                {
                    nodeToDrag.Pos += (e.GetClientPos() - cursorStartPos) / ZoomHandler.Zoom;
                    nodeHandler.Tree.AddNode(nodeToDrag);
                }
                nodeToDrag = null;
            }
        }

        public void CancelDrag()
        {
            nodeToDrag = null;
        }
    }
}
