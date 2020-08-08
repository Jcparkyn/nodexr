//Handle copying text to the user's clipboard.
//From https://chrissainty.com/copy-to-clipboard-in-blazor/

(<any>window).clipboardCopy = {
    copyText: function (text, message) {
        navigator.clipboard.writeText(text).then(() => {
            if (!!message) {
                alert(message);
            }
        })
            .catch(ex => alert(ex));
    }
};
