class NoodleDragHandler {
    constructor() {
        this.noodleElement = null;
        this.isValid = false;
        this.startX = 0;
        this.startY = 0;
        this.endX = 0;
        this.endY = 0;
        this.startNoodleDrag = (startX, startY) => {
            this.noodleElement = document.getElementById("tempNoodle");
            this.startX = this.endX = startX;
            this.startY = this.endY = startY;
            window.addEventListener("dragover", this.dragNoodle);
            this.updatePath();
            this.setInvalid();
        };
        this.dragNoodle = (event) => {
            [this.endX, this.endY] = window.panzoom.clientToGraphPos(event.clientX, event.clientY);
            this.updatePath();
        };
        this.endDrag = () => {
            window.removeEventListener("dragover", this.dragNoodle);
        };
        this.updatePath = () => {
            if (this.noodleElement != null) {
                this.setPath(this.startX, this.startY, this.endX, this.endY);
            }
        };
        this.setPath = (startX, startY, endX, endY) => {
            let path = NoodleDragHandler.getNoodlePath(startX, startY, endX, endY);
            if (this.noodleElement != null) {
                this.noodleElement.setAttribute("d", path);
            }
        };
        this.setValid = () => {
            if (!this.isValid) {
                this.isValid = true;
                if (this.noodleElement != null) {
                    this.noodleElement.classList.remove("noodle-invalid");
                }
            }
        };
        this.setInvalid = () => {
            this.isValid = false;
            if (this.noodleElement != null) {
                this.noodleElement.classList.add("noodle-invalid");
            }
        };
    }
}
NoodleDragHandler.getNoodlePath = (startX, startY, endX, endY) => {
    var ctrlLength = (5 + 0.4 * Math.abs(endX - startX) + 0.2 * Math.abs(endY - startY));
    var result = `M ${startX} ${startY} C ${startX + ctrlLength} ${startY} ${endX - ctrlLength} ${endY} ${endX} ${endY}`;
    return result;
};
window['tempNoodle'] = new NoodleDragHandler();
//# sourceMappingURL=NoodleDragHandler.js.map