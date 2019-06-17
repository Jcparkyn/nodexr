//window.zoomHandler.addEventListener("onload", window.zoomHandler.init);
var posX = 0;
var posY = 0;
var zoomLvl = 1.0;
var targetDiv = undefined;

window.panzoom = {

    zoom: function (event) {
        event.preventDefault();
        var element = event.target;
        this.zoomAt(event.offsetX, event.offsetY, 1 - event.deltaY * 0.001);
        //console.log("zooming");
        this.setZoom();
    },

    zoomAt: function (x, y, amount) {
        zoomLvl *= amount;
        var dX = (x - posX) * (1-amount);
        var dY = (y - posY) * (1-amount);
        posX += dX;
        posY += dY;
    },

    startPan: function (event) {
        //console.log("start pan");
        event.preventDefault();
        //var element = document.getElementById("nodegraph").parentElement;
        window.onmousemove = this.pan;
        window.onmouseup = function () {
            panzoom.endPan(targetDiv);
        };
    },

    endPan: function (element) {
        //console.log("end pan");
        window.onmouseup = null;
        window.onmousemove = null;
    },

    pan: function (event) {
        //console.log("pan");
        event.preventDefault();
        posX += event.movementX;
        posY += event.movementY;
        panzoom.setZoom();
    },

    setZoom: function () {
        if (!targetDiv) {
            targetDiv = document.getElementById("nodegraph");
        }
        targetDiv.style.transform = `translate(${posX}px, ${posY}px) scale(${zoomLvl})`;
        //targetDiv.style.transform = `matrix(${zoomLvl}, 0, 0, ${zoomLvl}, ${posX}, ${posY})`;
    },
}