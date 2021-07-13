window.clipboardCopy = {
    copyText: function (text, message) {
        navigator.clipboard.writeText(text).then(() => {
            if (!!message) {
                alert(message);
            }
        })
            .catch(ex => alert(ex));
    }
};
