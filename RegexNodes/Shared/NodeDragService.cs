using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using RegexNodes.Shared.Components;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace RegexNodes.Shared
{
    public interface INodeDragService
    {
        INode NodeToDrag { get; set; }
        NodeDragService.DragType CurDragType { get; set; }
        NodeDragService.NoodleDataCustom TempNoodle { get; }
        void OnStartNodeDrag(INode nodeToDrag, DragEventArgs e);
        void OnStartNoodleDrag(INodeOutput nodeToDrag, DragEventArgs e);
        void OnStartNoodleDrag(INodeOutput nodeToDrag, DragEventArgs e, Vector2L _cursorStartPos);
        void OnDrag(DragEventArgs e);
        void OnDrop(DragEventArgs e);
        void OnDropNoodle(InputProcedural nodeInput);
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
        }

        public enum DragType : int
        {
            None,
            Node,
            Noodle,
        }

        public INode NodeToDrag { get; set; }
        public DragType CurDragType { get; set; } = DragType.None;

        public NoodleDataCustom TempNoodle { get; private set; } = new NoodleDataCustom() { Enabled = false };

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

        public void OnStartNoodleDrag(INodeOutput nodeToDrag, DragEventArgs e)
        {
            OnStartNoodleDrag(nodeToDrag, e, nodeToDrag.OutputPos);
        }

        public void OnStartNoodleDrag(INodeOutput nodeToDrag, DragEventArgs e, Vector2L noodleEndPos)
        {
            CurDragType = DragType.Noodle;
            NodeToDrag = (Node)nodeToDrag;
            TempNoodle.StartPos = nodeToDrag.OutputPos;
            TempNoodle.EndPos = nodeToDrag.OutputPos;

            TempNoodle.Enabled = true;
            nodeHandler.OnRequireNoodleRefresh?.Invoke();

            Console.WriteLine("Start Noodle Drag");
            jsRuntime.InvokeAsync<object>("tempNoodle.startNoodleDrag",
                nodeToDrag.OutputPos.x, nodeToDrag.OutputPos.y,
                noodleEndPos.x, noodleEndPos.y);
            
            //TempNoodle.Refresh();
        }

        [Obsolete("OnDrag handled in javascript.")]
        public void OnDrag(DragEventArgs e)
        {

            if (CurDragType == DragType.Noodle)
            {
                Vector2L endPoint = NodeToDrag.OutputPos + (e.GetClientPos() - cursorStartPos) / ZoomHandler.Zoom;
                TempNoodle.EndPos = endPoint;
            }
        }

        public void OnDrop(DragEventArgs e)
        {
            if (CurDragType == DragType.Node && NodeToDrag != null)
            {
                NodeToDrag.Pos += (e.GetClientPos() - cursorStartPos) / ZoomHandler.Zoom;
            }
            
            TempNoodle.Enabled = false;
            //TempNoodle.Valid = false;
            NodeToDrag = null;
            CurDragType = DragType.None;
        }

        public void OnDropNoodle(InputProcedural nodeInput)
        {
            Console.WriteLine("OnDropNoodle");
            //TODO: Properly check for cyclic dependencies
            if (CurDragType != DragType.Noodle || NodeToDrag.GetInputsRecursive().Contains(nodeInput))
            {
                Console.WriteLine("Can't drop here!");
                NodeToDrag = null;
                return;
            }
            nodeInput.ConnectedNode = NodeToDrag;
            NodeToDrag = null;
            //NoodleEnd = null;
        }

        public class NoodleDataCustom : INoodleData
        {
            public Vector2L StartPos { get; set; }

            public Vector2L EndPos { get; set; }

            public bool Enabled { get; set; }
        }
    }
}
