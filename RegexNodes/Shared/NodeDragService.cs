using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using RegexNodes.Shared.Components;
using System;
using System.Threading.Tasks;

namespace RegexNodes.Shared
{
    public interface INodeDragService
    {
        INode NodeToDrag { get; set; }
        NodeDragService.DragType CurDragType { get; set; }
        //TempNoodleEnd NoodleEnd { get; set; }
        NoodleSvg TempNoodle { get; set; }
        void OnStartNodeDrag(INode nodeToDrag, DragEventArgs e);
        void OnStartNoodleDrag(INode nodeToDrag, DragEventArgs e);
        void OnDrag(DragEventArgs e);
        void OnDrop(DragEventArgs e);
        void OnDropNoodle(InputProcedural nodeInput);
        //void OnDeleteNode();
        Task OnStartCreateNodeDrag(INode nodeToDrag, DragEventArgs e);
    }

    public class NodeDragService : INodeDragService
    {
        readonly INodeHandler nodeHandler;
        readonly IJSRuntime jsRuntime;
        public NodeDragService(INodeHandler nodeHandler, IJSRuntime jsRuntime)
        {
            this.nodeHandler = nodeHandler;
            this.jsRuntime = jsRuntime;
            //jsRuntime.InvokeAsync<object>("initNodeDropHandler", DotNetObjectRef.Create(this));
        }

        public enum DragType : int
        {
            None,
            Node,
            Noodle,
        }

        public INode NodeToDrag { get; set; }
        public DragType CurDragType { get; set; } = DragType.None;

        //public TempNoodleEnd NoodleEnd { get; set; } = new TempNoodleEnd();
        public NoodleSvg TempNoodle { get; set; }
        Vector2L cursorStartPos;


        public void OnStartNodeDrag(INode nodeToDrag, DragEventArgs e)
        {
            NodeToDrag = nodeToDrag;
            CurDragType = DragType.Node;
            cursorStartPos = e.GetClientPos();
        }

        public async Task OnStartCreateNodeDrag(INode nodeToDrag, DragEventArgs e)
        {
            NodeToDrag = nodeToDrag;
            CurDragType = DragType.Node;
            cursorStartPos = e.GetClientPos();
            var scaledPos = await jsRuntime.InvokeAsync<float[]>("panzoom.clientToGraphPos", e.ClientX, e.ClientY);
            int x = (int)scaledPos[0];
            int y = (int)scaledPos[1];
            
            NodeToDrag.Pos = new Vector2L(x-75, y-15);
        }

        public void OnStartNoodleDrag(INode nodeToDrag, DragEventArgs e)
        {
            CurDragType = DragType.Noodle;
            NodeToDrag = nodeToDrag;
            //NoodleEnd.Pos = NodeToDrag.Pos + new Vector2L(150, 17);
            TempNoodle.SetStartPoint(nodeToDrag.OutputPos);
            TempNoodle.SetEndPoint(nodeToDrag.OutputPos);

            
            TempNoodle.Enabled = true;

            cursorStartPos = e.GetClientPos();
            Console.WriteLine("Start Noodle Drag");
            jsRuntime.InvokeAsync<object>("tempNoodle.startNoodleDrag");
            //TempNoodle.Refresh();
        }

        public void OnDrag(DragEventArgs e)
        {

            if (CurDragType == DragType.Noodle)
            {
                //NoodleEnd.Pos = NodeToDrag.Pos + new Vector2L(150, 17) + (e.GetClientPos() - cursorStartPos) / ZoomHandler.Zoom;
                //TempNoodle.Refresh();
                Vector2L endPoint = NodeToDrag.OutputPos + (e.GetClientPos() - cursorStartPos) / ZoomHandler.Zoom;
                TempNoodle.SetEndPoint(endPoint);
            }
        }

        public void OnDrop(DragEventArgs e)
        {
            if (CurDragType == DragType.Node)
            {
                NodeToDrag?.MoveBy((e.GetClientPos() - cursorStartPos) / ZoomHandler.Zoom);
            }
            else if (CurDragType == DragType.Noodle)
            {
                //TempNoodle.Refresh();
            }
            TempNoodle.Enabled = false;
            TempNoodle.Valid = false;
            NodeToDrag = null;
            CurDragType = DragType.None;
        }

        //[JSInvokable]
        //public void DropNodeJS(long xPos, long yPos)
        //{
        //    if (CurDragType == DragType.Node)
        //    {
        //        //NodeToDrag?.MoveBy(xPos, yPos);
        //        Console.WriteLine("Drop from JS: " + xPos + ", " + yPos);
        //        NodeToDrag.Pos = new Vector2L(xPos, yPos);
        //        nodeHandler.OnNodeCountChanged();
        //    }
        //    TempNoodle.Enabled = false;
        //    TempNoodle.Valid = false;
        //    NodeToDrag = null;
        //    CurDragType = DragType.None;
        //}

        public void OnDropNoodle(InputProcedural nodeInput)
        {
            Console.WriteLine("OnDropNoodle");
            if (CurDragType == DragType.Node || NodeToDrag.NodeInputs.Contains(nodeInput))
            {
                Console.WriteLine("Can't drop here!");
                NodeToDrag = null;
                return;
            }
            nodeInput.InputNode = NodeToDrag;
            NodeToDrag = null;
            //NoodleEnd = null;
        }

        //public void OnDeleteNode()
        //{
        //    if (CurDragType == DragType.Node)
        //    {
        //        nodeHandler.DeleteNode(NodeToDrag);
        //    }
        //}
    }
}
