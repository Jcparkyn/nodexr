
class NodeDragHandler {

    public startNodeDrag = () => {
        //console.log("starting node drag");
        document.addEventListener('mousemove', window['NodeDragHandler'].dragNode);
        document.addEventListener('mouseup', () =>
            document.removeEventListener('mousemove', window['NodeDragHandler'].dragNode)
        );
    }

    public dragNode = (event: MouseEvent) => {
        window['DotNetNodeDragService'].invokeMethodAsync("DragNode", event.clientX, event.clientY);
    }
}

window["NodeDragHandler"] = new NodeDragHandler();
