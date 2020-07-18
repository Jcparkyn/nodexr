window.tempNoodle = {

    prevX: 0,
    prevY: 0,
    startX: 0,
    startY: 0,
    endX: 0,
    endY: 0,
    noodleElement: null,
    isValid: false,

    startNoodleDrag: function (_startX, _startY, _endX, _endY) {
        tempNoodle.noodleElement = document.getElementById("tempNoodle");
        tempNoodle.startX = tempNoodle.endX = _startX;
        tempNoodle.startY = tempNoodle.endY = _startY;
        window.addEventListener("dragover", tempNoodle.dragNoodle);
        
        tempNoodle.updatePath()
        tempNoodle.setInvalid();
    },

    dragNoodle: function (event) {
        [tempNoodle.endX, tempNoodle.endY] = window.panzoom.clientToGraphPos(event.clientX, event.clientY);
        tempNoodle.updatePath();
    },

    endDrag: function () {
        window.removeEventListener("dragover", tempNoodle.dragNoodle);
    },

    updatePath: function () {
        if (tempNoodle.noodleElement != null) {
            tempNoodle.setPath(
                tempNoodle.startX,
                tempNoodle.startY,
                tempNoodle.endX,
                tempNoodle.endY);
        }
    },

    setPath: function (_startX, _startY, _endX, _endY) {
        let path = tempNoodle.getNoodlePath(_startX, _startY, _endX, _endY);
        if (tempNoodle.noodleElement != null) {
            tempNoodle.noodleElement.setAttribute("d", path);
        }
    },

    setValid: function () {
        if (!tempNoodle.isValid) {
            tempNoodle.isValid = true;
            if (tempNoodle.noodleElement != null) {
                tempNoodle.noodleElement.classList.remove("noodle-invalid");
            }
        }
    },

    setInvalid: function () {
        tempNoodle.isValid = false;
        if (tempNoodle.noodleElement != null) {
            tempNoodle.noodleElement.classList.add("noodle-invalid");
        }
    },

    getNoodlePath: function (startX, startY, endX, endY) {
        var ctrlLength = (5 + 0.4 * Math.abs(endX - startX) + 0.2 * Math.abs(endY - startY));
        var result = `M ${startX} ${startY} C ${startX + ctrlLength} ${startY} ${endX - ctrlLength} ${endY} ${endX} ${endY}`;
        //console.log(result);
        return result;
    }
}