

window.jsFunctions = {
    blazorHasRendered: function () {
        console.log("splitting windows");
        Split(['#viewport-searchtext', '#viewport-replacetext', '#viewport-replaceresult'], {
            sizes: [40, 30, 30],
            gutterSize: 7,
            snapOffset: 0,
            minSize: [250, 250, 250],
        })
        Split(['#mainpanels', '#bottompanels'], {
            sizes: [85, 15],
            direction: 'vertical',
            gutterSize: 7,
            snapOffset: 0,
        })
    }
}