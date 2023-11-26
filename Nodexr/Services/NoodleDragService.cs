namespace Nodexr.Services;
using Microsoft.JSInterop;
using Blazored.Toast.Services;
using BlazorNodes.Core;

public interface INoodleDragService
{
    NoodleDragService.NoodleDataCustom TempNoodle { get; }

    void CancelDrag();
    void OnDropNoodle(IInputPort nodeInput);
    void OnStartNoodleDrag(INodeOutput nodeToDrag);
    void OnStartNoodleDrag(INodeOutput nodeToDrag, Vector2 noodleEndPos);
}

public class NoodleDragService(IToastService toastService, IJSRuntime jsRuntime) : INoodleDragService
{
    private INodeOutput? nodeToDrag;
    public NoodleDataCustom TempNoodle { get; } = new NoodleDataCustom() { Connected = false };

    public void OnStartNoodleDrag(INodeOutput nodeToDrag)
    {
        OnStartNoodleDrag(nodeToDrag, nodeToDrag.OutputPos);
    }

    public void OnStartNoodleDrag(INodeOutput nodeToDrag, Vector2 noodleEndPos)
    {
        this.nodeToDrag = nodeToDrag;
        TempNoodle.Connected = true;

        // TODO: refactor to avoid synchronous JS interop
        ((IJSInProcessRuntime)jsRuntime).Invoke<object>("tempNoodle.startNoodleDrag",
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

        // TODO: refactor to avoid synchronous JS interop
        ((IJSInProcessRuntime)jsRuntime).InvokeVoid("tempNoodle.endDrag");

        TempNoodle.Refresh();
    }

    public class NoodleDataCustom : INoodleData
    {
        public Vector2 StartPos { get; set; }
        public Vector2 EndPos { get; set; }
        public bool Connected { get; set; }

        public event Action? NoodleChanged;

        public void Refresh()
        {
            NoodleChanged?.Invoke();
        }
    }
}
