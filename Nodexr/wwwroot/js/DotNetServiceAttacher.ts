interface dotNetObjectRef {
    invokeMethodAsync: Function;
}

window['addDotNetSingletonService'] = (name: string, dotNetServiceRef: dotNetObjectRef) => {
    window[name] = dotNetServiceRef;
}