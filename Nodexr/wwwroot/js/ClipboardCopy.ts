//Handle copying text to the user's clipboard.
//From https://chrissainty.com/copy-to-clipboard-in-blazor/

(<any>window).clipboardCopy = {
    copyText: async function (text, message) {
        try {
            await navigator.clipboard.writeText(text);
            if (!!message) {
                alert(message);
            }
        }
        catch (ex) {
            alert(ex);
        }
    }
};
