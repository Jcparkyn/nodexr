
export function initContentEditable(div: HTMLElement, instance: DotNetObjectRef, textToDisplay: string): void {
    div.innerText = textToDisplay;
    div.addEventListener("input", function () {
        instance.invokeMethodAsync("GetUpdatedTextFromJavascript", div.innerText);
    });

    try {
        div.contentEditable = "plaintext-only";
    }
    catch (e) {
        setupFallbackPlaintextOnly(div);
    }
    moveCursorToEnd(div);
};

function moveCursorToEnd(elem: HTMLElement) {
    let s = window.getSelection();
    let r = document.createRange();
    r.setStart(elem, 1);
    r.setEnd(elem, 1);
    s.removeAllRanges();
    s.addRange(r);
};

function setupFallbackPlaintextOnly(elem: HTMLElement) {
    elem.contentEditable = "true";
    forcePlaintextPaste(elem);

    elem.addEventListener("drop", e => {
        e.preventDefault();
        return false;
    });
};

function forcePlaintextPaste(elem: HTMLElement) {
    elem.addEventListener("paste", e => {
        e.preventDefault();
        if (e.clipboardData && e.clipboardData.getData) {
            var text = e.clipboardData.getData("text/plain");
            document.execCommand("insertText", false, text);
        }
    });
};