//window.zoomHandler.addEventListener("onload", window.zoomHandler.init);
var posX = 0;
var posY = 0;
var zoomLvl = 1.0;
var targetDiv = undefined;

window.panzoom = {

    zoom: function (event) {
        event.preventDefault();
        var element = event.target;
        var mousePosRelative = this.clientToGraphPos(event.clientX, event.clientY);
        //console.log("Client: " + event.clientX + ", " + event.clientY);
        //console.log("Offset: " + event.offsetX + ", " + event.offsetY);
        //console.log("Local: " + mousePosRelative[0] + ", " + mousePosRelative[1]);

        this.zoomAt(window.innerWidth * 0.45, window.innerHeight*0.35, 1 - event.deltaY * 0.0007);
        //this.zoomAt(event.offsetX, event.offsetY, 1 - event.deltaY * 0.001);
        //console.log("zooming");
        this.setZoom();
    },

    zoomAt: function (x, y, amount) {
        var maxZoom = 3;
        var minZoom = 0.3;
        zoomLvl *= amount;
        if (zoomLvl > maxZoom) {
            amount = maxZoom / (zoomLvl / amount);
            zoomLvl = maxZoom;
        }
        if (zoomLvl < minZoom) {
            amount = minZoom / (zoomLvl / amount);
            zoomLvl = minZoom;
        }
        DotNet.invokeMethodAsync('RegexNodes', 'SetZoom', zoomLvl)
        var dX = (x - posX) * (1 - amount);
        var dY = (y - posY) * (1 - amount);
        //var dX = x * (1 - amount) * zoomLvl;
        //var dY = y * (1 - amount) * zoomLvl;
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
        //targetDiv.style.top = posY + "px";
        //targetDiv.style.left = posX + "px";
        targetDiv.style.transform = `translate(${posX}px, ${posY}px) scale(${zoomLvl})`; //translate(${posX}px, ${posY}px) 

        //targetDiv.style.transform = `matrix(${zoomLvl}, 0, 0, ${zoomLvl}, ${posX}, ${posY})`;
    },

    clientToGraphPos: function (clientX, clientY) {
        if (!targetDiv) {
            targetDiv = document.getElementById("nodegraph");
        }
        var rect = targetDiv.getBoundingClientRect();
        var x = (clientX - rect.left) / zoomLvl;
        var y = (clientY - rect.top) / zoomLvl;
        
        return [x,y];
    },
}