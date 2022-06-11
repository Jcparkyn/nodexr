
export const startDrag = (callbackObj: DotNetObjectRef, methodName: string) => {
    const dragCallback = (event: MouseEvent) => {
        callbackObj.invokeMethodAsync(methodName, event.clientX, event.clientY);
    };
    document.addEventListener("mousemove", dragCallback);
    document.addEventListener("mouseup", () => {
        document.removeEventListener("mousemove", dragCallback);
    }, { once: true });
}
