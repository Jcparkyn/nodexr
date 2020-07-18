﻿using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Nodexr.Shared.Components;
using Nodexr.Shared;
using Nodexr.Shared.Nodes;
using Nodexr.Shared.NodeInputs;
using System;
using System.Threading.Tasks;
using System.Linq;
using Blazored.Toast.Services;

namespace Nodexr.Shared.Services
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
        public NoodleDataCustom TempNoodle { get; } = new NoodleDataCustom() { Enabled = false };

        private readonly IToastService toastService;
        private readonly IJSRuntime jsRuntime;

        public NoodleDragService(IToastService toastService, IJSRuntime jsRuntime)
        {
            this.toastService = toastService;
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

                jsRuntime.InvokeAsync<object>("tempNoodle.startNoodleDrag",
                    nodeToDrag.OutputPos.x, nodeToDrag.OutputPos.y,
                    noodleEndPos.x, noodleEndPos.y);

                TempNoodle.Refresh();
            }
        }

        public void OnDropNoodle(InputProcedural nodeInput)
        {
            TempNoodle.Enabled = false;

            if (NodeToDrag is null) return;

            if (NodeToDrag.IsDependentOn(nodeInput))
            {
                toastService.ShowError(
                    "Cyclical dependencies would create a rift in the space-time continuum and are therefore not allowed. " +
                    "If you want to use the same node multiple times in a row, connect it to multiple inputs of a 'Concatenate' node.",
                    "Can't connect these nodes");
            }
            else
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
            jsRuntime.InvokeVoidAsync("tempNoodle.endDrag");

            TempNoodle.Refresh();
        }

        public class NoodleDataCustom : INoodleData
        {
            public Vector2L StartPos { get; set; }
            public Vector2L EndPos { get; set; }
            public bool Enabled { get; set; }

            public event EventHandler NoodleChanged;

            public void Refresh()
            {
                NoodleChanged?.Invoke(this, EventArgs.Empty);
            }
        }
    }
}