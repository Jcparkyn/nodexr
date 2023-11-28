window["jsFunctions"] = {
    blazorHasRendered: function () {
        Split(['#viewport-searchtext', '#viewport-replacetext'], {
            sizes: [50, 50],
            gutterSize: 7,
            snapOffset: 0,
            minSize: [250, 250],
        });
        Split(['#mainpanels', '#bottompanels'], {
            sizes: [85, 15],
            direction: 'vertical',
            gutterSize: 7,
            snapOffset: 0,
            elementStyle: (dimension, size, gutterSize) => ({
                'height': `calc(${size}% - ${gutterSize}px)`,
            }),
            gutterStyle: (dimension, gutterSize) => ({
                'flex-basis': `${gutterSize}px`,
            }),
        });
    }
};
