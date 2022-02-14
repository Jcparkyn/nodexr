namespace Nodexr.Components;

using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Components.Web;
using Microsoft.JSInterop;
using Nodexr.Utils;
using Nodexr.NodeInputs;
using BlazorNodes.Core;

public partial class NodeGraph
{
    private bool shouldRender = false;
    protected override bool ShouldRender() => shouldRender;
    private Vector2 clickStartPos; //Used to check whether an onclick event was actually a drag
    private static Type InputViewModelProvider(INodeInput input)
    {
        return input switch
        {
            NodeInputs.InputCheckbox => typeof(InputCheckboxView),
            InputCheckboxNullable => typeof(InputCheckboxNullableView),
            InputCollection => typeof(InputCollectionView),
            InputDropdown => typeof(InputDropdownView),
            InputNumber => typeof(InputNumberView),
            InputProcedural => typeof(InputProceduralView),
            InputRange => typeof(InputRangeView),
            InputString => typeof(InputStringView),
            _ => throw new ArgumentOutOfRangeException($"No view class defined for type {input.GetType()}")
        };
    }

    protected override void OnInitialized()
    {
        //NodeHandler.OnNodeCountChanged += StateHasChanged;
        NodeHandler.OnRequireNodeGraphRefresh += (sender, e) => Refresh();
    }

    private async Task StartPan(MouseEventArgs e)
    {
        clickStartPos = e.GetClientPos();
        await JSRuntime.InvokeVoidAsync("panzoom.startPan");
    }

    private void Refresh()
    {
        shouldRender = true;
        StateHasChanged();
        shouldRender = false;
    }

    private void OnDrop(DragEventArgs e)
    {
        NodeDragService.OnDrop(e);
        Refresh();
    }

    private void OnKeyDown(KeyboardEventArgs e)
    {
        if (e.Key == "Delete" || e.Key == "Backspace")
        {
            NodeHandler.DeleteSelectedNodes();
        }
        else if (e.Key == "Escape")
        {
            DeselectNode();
        }
    }

    private void OnClick(MouseEventArgs e)
    {
        const int dragThreshold = 4; //Length in px to consider a drag (instead of a click)
        var mouseOffset = e.GetClientPos() - clickStartPos;
        if (mouseOffset.GetLength() <= dragThreshold)
        {
            DeselectNode();
        }
    }

    private void DeselectNode()
    {
        NodeHandler.Tree.DeselectAllNodes();
    }
}
