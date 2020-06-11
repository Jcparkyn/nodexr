//Handle copying text to the user's clipboard.
//From https://chrissainty.com/copy-to-clipboard-in-blazor/

window.clipboardCopy = {
    copyText: function (text) {
        navigator.clipboard.writeText(text).then(function () {
            //alert("Copied to clipboard!");
        })
            .catch(function (error) {
                alert(error);
            });
    }
};