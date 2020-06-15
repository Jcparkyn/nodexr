using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using RegexNodes.Shared.Components;
using RegexNodes.Shared;
using System;
using System.Threading.Tasks;
using System.Linq;

namespace RegexNodes.Shared
{
    public interface INoodleDragService
    {
        NoodleDragService.NoodleDataCustom TempNoodle { get; }

        void CancelDrag();
        void OnDropNoodle(InputProcedural nodeInput);
        void OnStartNoodleDrag(INodeOutput nodeToDrag, DragEventArgs e);
        void OnStartNoodleDrag(INodeOutput nodeToDrag, DragEventArgs e, Vector2L noodleEndPos);
    }

    public class NoodleDragService : INoodleDragService
    {
        public INode NodeToDrag { get; set; }
        public NoodleDataCustom TempNoodle { get; private set; } = new NoodleDataCustom() { Enabled = false };

        readonly INodeHandler nodeHandler;
        readonly IJSRuntime jsRuntime;
        public NoodleDragService(INodeHandler nodeHandler, IJSRuntime jsRuntime)
        {
            this.nodeHandler = nodeHandler;
            this.jsRuntime = jsRuntime;
        }

        public void OnStartNoodleDrag(INodeOutput nodeToDrag, DragEventArgs e)
        {
            OnStartNoodleDrag(nodeToDrag, e, nodeToDrag.OutputPos);
        }

        public void OnStartNoodleDrag(INodeOutput nodeToDrag, DragEventArgs e, Vector2L noodleEndPos)
        {
            NodeToDrag = nodeToDrag as Node;
            if(nodeToDrag is Node node)
            {
                NodeToDrag = node;
                TempNoodle.Enabled = true;
                //nodeHandler.OnRequireNoodleRefresh?.Invoke();

                //Console.WriteLine("Start Noodle Drag");
                jsRuntime.InvokeAsync<object>("tempNoodle.startNoodleDrag",
                    nodeToDrag.OutputPos.x, nodeToDrag.OutputPos.y,
                    noodleEndPos.x, noodleEndPos.y);

                TempNoodle.Refresh();
            }
        }

        public void OnDropNoodle(InputProcedural nodeInput)
        {
            Console.WriteLine("Drop noodle");
            TempNoodle.Enabled = false;

            //TODO: Properly check for cyclic dependencies
            if (NodeToDrag != null && !NodeToDrag.GetInputsRecursive().Contains(nodeInput))
            {
                nodeInput.ConnectedNode = NodeToDrag;
            }

            NodeToDrag = null;
        }

        public void CancelDrag()
        {
            NodeToDrag = null;
            TempNoodle.Enabled = false;
            //Hack to stop the noodle from being visible for a frame when a drag starts
            //TempNoodle.StartPos = (10000, 10000);
            //TempNoodle.EndPos = (10000, 10000);

            TempNoodle.Refresh();
        }

        public class NoodleDataCustom : INoodleData
        {
            public Vector2L StartPos { get; set; }
            public Vector2L EndPos { get; set; }
            public bool Enabled { get; set; }

            public event EventHandler Changed;

            public void Refresh()
            {
                Changed?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
