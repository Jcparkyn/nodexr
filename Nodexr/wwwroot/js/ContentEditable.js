
window.contentEditable = {
    getInnerHtml: function (element) {
        return element.innerHTML;
    },

    initContentEditable: function (div, instance, textToDisplay) {
        div.innerText = textToDisplay;
        div.addEventListener("input", function () {
            instance.invokeMethodAsync("GetUpdatedTextFromJavascript", div.innerText);
        });

        try {
            div.contentEditable = "plaintext-only";
        }
        catch (e) {
            contentEditable.setupFallbackPlaintextOnly(div);
        }
        contentEditable.moveCursorToEnd(div);
    },

    moveCursorToEnd: function (elem) {
        let s = window.getSelection();
        let r = document.createRange();
        r.setStart(elem, 1);
        r.setEnd(elem, 1);
        s.removeAllRanges();
        s.addRange(r);
    },

    setupFallbackPlaintextOnly: function (elem) {
        elem.contentEditable = "true";
        contentEditable.forcePlaintextPaste(elem);

        elem.addEventListener("drop", function (e) {
            e.preventDefault();
            return false;
        });
    },

    forcePlaintextPaste: function (elem) {
        elem.addEventListener("paste", function (e) {
            e.preventDefault();
            if (e.clipboardData && e.clipboardData.getData) {
                var text = e.clipboardData.getData("text/plain");
                document.execCommand("insertText", false, text);
            } else if (window.clipboardData && window.clipboardData.getData) {
                var text = window.clipboardData.getData("Text");
                contentEditable.insertTextAtCursor(text);
            }
        });
    },

    insertTextAtCursor: function(text) {
        var sel, range;
        if (window.getSelection) {
            sel = window.getSelection();
            if (sel.getRangeAt && sel.rangeCount) {
                range = sel.getRangeAt(0);
                range.deleteContents();
                range.insertNode(document.createTextNode(text));
            }
        } else if (document.selection && document.selection.createRange) {
            document.selection.createRange().text = text;
        }
    },
}