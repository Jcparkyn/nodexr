
//Create draggable dividers between windows
window.jsFunctions = {
    blazorHasRendered: function () {
        Split(['#viewport-searchtext', '#viewport-replacetext', '#viewport-replaceresult'], {
            sizes: [40, 25, 35],
            gutterSize: 7,
            snapOffset: 0,
            minSize: [250, 250, 250],
        })
        Split(['#mainpanels', '#bottompanels'], {
            sizes: [90, 10],
            direction: 'vertical',
            gutterSize: 7,
            snapOffset: 0,
            elementStyle: (dimension, size, gutterSize) => ({
                'flex-basis': `calc(${size}% - ${gutterSize}px)`,
            }),
            gutterStyle: (dimension, gutterSize) => ({
                'flex-basis': `${gutterSize}px`,
            }),
        })
    }
}