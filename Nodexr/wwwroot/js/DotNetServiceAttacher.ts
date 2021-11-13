interface DotNetObjectRef {
    invokeMethodAsync: Function;
}

window['addDotNetSingletonService'] = (name: string, dotNetServiceRef: DotNetObjectRef) => {
    window[name] = dotNetServiceRef;
}