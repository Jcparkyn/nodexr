var prevX = 0;
var prevY = 0;
var noodleElement = null;
//var img = new Image();
//img.src = "http://placehold.it/150/000000/ffffff?text=GHOST";

window.tempNoodle = {

    startNoodleDrag: function () {
        if (!noodleElement) {
            noodleElement = document.getElementById("tempNoodle");
        }
        
        tempNoodle.setInvalid();
    },

    //testDragStart: function (event) {
    //    console.log("DHSFFGSDG");
    //    console.log("DHSFFGSDG, " + event.target.name);
    //    event.dataTransfer.setDragImage(img, 0, 0);
    //},

    dragNoodle: function (event) {
        if (event.offsetX != prevX || event.offsetY != prevY) {
            //event.dataTransfer.setDragImage(img, 0, 0);
            prevX = event.offsetX;
            prevY = event.offsetY;
            //console.log("drag");
            var pathPrev = noodleElement.getAttribute("d");
            var pathPrevSplit = pathPrev.split(" ");
            var startX = parseFloat(pathPrevSplit[1]);
            var startY = parseFloat(pathPrevSplit[2]);
            var endX = startX + event.offsetX - 6;
            var endY = startY + event.offsetY - 6;
            noodleElement.setAttribute("d", this.getNoodlePath(startX, startY, endX, endY));
        }
    },

    setValid: function () {
        noodleElement.classList.remove("noodle-invalid");
    },
    setInvalid: function () {
        noodleElement.classList.add("noodle-invalid");
    },

    getNoodlePath: function (startX, startY, endX, endY) {
        var ctrlLength = (5 + 0.4 * Math.abs(endX - startX) + 0.2 * Math.abs(endY - startY));
        var result = `M ${startX} ${startY} C ${startX + ctrlLength} ${startY} ${endX - ctrlLength} ${endY} ${endX} ${endY}`;
        //console.log(result);
        return result;
    }
}