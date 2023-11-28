export const startDrag = (callbackObj, methodName) => {
    const dragCallback = (event) => {
        callbackObj.invokeMethodAsync(methodName, event.clientX, event.clientY);
    };
    document.addEventListener("mousemove", dragCallback);
    document.addEventListener("mouseup", () => {
        document.removeEventListener("mousemove", dragCallback);
    }, { once: true });
};
