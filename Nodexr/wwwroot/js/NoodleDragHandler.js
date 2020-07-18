var prevX = 0;
var prevY = 0;
var startX, startY = 0;
var noodleElement = null;
var isValid = false;

window.tempNoodle = {

    startNoodleDrag: function (_startX, _startY, _endX, _endY) {
        noodleElement = document.getElementById("tempNoodle");
        startX = _startX;
        startY = _startY;
        this.setInvalid();
        this.updatePath();
    },

    dragNoodle: function (event) {
        this.updatePath();
    },

    updatePath: function () {
        if (noodleElement != null) {
            let [endX, endY] = window.panzoom.clientToGraphPos(event.clientX, event.clientY);
            this.setPath(this.getNoodlePath(startX, startY, endX, endY));
        }
    },

    setPath: function (path) {
        if (noodleElement != null) {
            noodleElement.setAttribute("d", path);
        }
    },

    setValid: function () {
        if (!isValid) {
            isValid = true;
            if (noodleElement != null) {
                noodleElement.classList.remove("noodle-invalid");
            }
        }
    },

    setInvalid: function () {
        isValid = false;
        if (noodleElement != null) {
            noodleElement.classList.add("noodle-invalid");
        }
    },

    getNoodlePath: function (startX, startY, endX, endY) {
        var ctrlLength = (5 + 0.4 * Math.abs(endX - startX) + 0.2 * Math.abs(endY - startY));
        var result = `M ${startX} ${startY} C ${startX + ctrlLength} ${startY} ${endX - ctrlLength} ${endY} ${endX} ${endY}`;
        //console.log(result);
        return result;
    }
}