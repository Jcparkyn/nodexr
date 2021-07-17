using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Nodexr.Shared.Nodes;
using Nodexr.Shared.NodeInputs;
using System;
using System.Threading.Tasks;
using System.Linq;
using Blazored.Toast.Services;
using BlazorNodes.Core;

namespace Nodexr.Services
{
    public interface INoodleDragService
    {
        NoodleDragService.NoodleDataCustom TempNoodle { get; }

        void CancelDrag();
        void OnDropNoodle(IInputPort nodeInput);
        void OnStartNoodleDrag(INodeOutput nodeToDrag);
        void OnStartNoodleDrag(INodeOutput nodeToDrag, Vector2 noodleEndPos);
    }

    public class NoodleDragService : INoodleDragService
    {
        private INodeOutput nodeToDrag;
        public NoodleDataCustom TempNoodle { get; } = new NoodleDataCustom() { Connected = false };

        private readonly IToastService toastService;
        private readonly IJSRuntime jsRuntime;

        public NoodleDragService(IToastService toastService, IJSRuntime jsRuntime)
        {
            this.toastService = toastService;
            this.jsRuntime = jsRuntime;
        }

        public void OnStartNoodleDrag(INodeOutput nodeToDrag)
        {
            OnStartNoodleDrag(nodeToDrag, nodeToDrag.OutputPos);
        }

        public void OnStartNoodleDrag(INodeOutput nodeToDrag, Vector2 noodleEndPos)
        {
            this.nodeToDrag = nodeToDrag;
            TempNoodle.Connected = true;

            jsRuntime.InvokeAsync<object>("tempNoodle.startNoodleDrag",
                nodeToDrag.OutputPos.x, nodeToDrag.OutputPos.y);

            TempNoodle.Refresh();
        }

        public void OnDropNoodle(IInputPort nodeInput)
        {
            TempNoodle.Connected = false;

            if (nodeToDrag is null) return;

            if (nodeToDrag is INodeViewModel node && node.IsDependentOn(nodeInput))
            {
                toastService.ShowError(
                    "Cyclical dependencies would create a rift in the space-time continuum and are therefore not allowed. " +
                    "If you want to use the same node multiple times in a row, connect it to multiple inputs of a 'Concatenate' node.",
                    "Can't connect these nodes");
            }
            else if (!nodeInput.TrySetConnectedNode(nodeToDrag))
            {
                toastService.ShowError("Input and output type do not match.");
            }

            nodeToDrag = null;
        }

        public void CancelDrag()
        {
            nodeToDrag = null;
            TempNoodle.Connected = false;

            jsRuntime.InvokeVoidAsync("tempNoodle.endDrag");

            TempNoodle.Refresh();
        }

        public class NoodleDataCustom : INoodleData
        {
            public Vector2 StartPos { get; set; }
            public Vector2 EndPos { get; set; }
            public bool Connected { get; set; }

            public event EventHandler NoodleChanged;

            public void Refresh()
            {
                NoodleChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}
