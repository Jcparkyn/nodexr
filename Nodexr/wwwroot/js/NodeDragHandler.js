class NodeDragHandler {
    constructor() {
        this.startNodeDrag = () => {
            //console.log("starting node drag");
            document.addEventListener('mousemove', window['NodeDragHandler'].dragNode);
            document.addEventListener('mouseup', () => document.removeEventListener('mousemove', window['NodeDragHandler'].dragNode));
        };
        this.dragNode = (event) => {
            window['DotNetNodeDragService'].invokeMethodAsync("DragNode", event.clientX, event.clientY);
        };
    }
}
window["NodeDragHandler"] = new NodeDragHandler();
//# sourceMappingURL=NodeDragHandler.js.map