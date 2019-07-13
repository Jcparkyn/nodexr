var dragService = null;

function initNodeDropHandler(dotNetRef) {
    dragService = dotNetRef;
}

//function dropNode(e) {
//    dragService.invokeMethodAsync('DropNodeJS', e.offsetX, e.offsetY);
//}