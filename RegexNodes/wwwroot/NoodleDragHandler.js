var prevX = 0;
var prevY = 0;
var startX, startY, endX, endY = 0;
var noodleElement = null;
//var img = new Image();
//img.src = "http://placehold.it/150/000000/ffffff?text=GHOST";

window.tempNoodle = {

    startNoodleDrag: function (_startX, _startY, _endX, _endY) {
        if (!noodleElement) {
            noodleElement = document.getElementById("tempNoodle");
        }
        startX = _startX;
        startY = _startY;
        endX = _endX - 6;
        endY = _endY - 6;
        tempNoodle.setInvalid();
    },

    dragNoodle: function (event) {
        if (noodleElement != null) {
            this.setPath(this.getNoodlePath(startX, startY, endX + event.offsetX, endY + event.offsetY));
        }
    },

    setPath: function (path) {
        noodleElement.setAttribute("d", path);
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