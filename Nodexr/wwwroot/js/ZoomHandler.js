class PanZoom {
    constructor() {
        this.posX = 0;
        this.posY = 0;
        this.zoomLvl = 1.0;
        this.targetDiv = undefined;
        this.zoom = (event) => {
            event.preventDefault();
            const scalingAmounts = {
                0: 1,
                1: 20,
                2: 1000,
            };
            let delta = -event.deltaY * 0.0007 * scalingAmounts[event.deltaMode];
            this.zoomAt(window.innerWidth * 0.45, window.innerHeight * 0.35, 1 + delta);
            this.setZoom();
        };
        this.zoomAt = (x, y, amount) => {
            let maxZoom = 3;
            let minZoom = 0.3;
            this.zoomLvl *= amount;
            if (this.zoomLvl > maxZoom) {
                amount = maxZoom / (this.zoomLvl / amount);
                this.zoomLvl = maxZoom;
            }
            if (this.zoomLvl < minZoom) {
                amount = minZoom / (this.zoomLvl / amount);
                this.zoomLvl = minZoom;
            }
            window.DotNet.invokeMethodAsync('Nodexr', 'SetZoom', this.zoomLvl);
            let dX = (x - this.posX) * (1 - amount);
            let dY = (y - this.posY) * (1 - amount);
            this.posX += dX;
            this.posY += dY;
        };
        this.startPan = (event) => {
            event.preventDefault();
            window.addEventListener("mousemove", this.pan);
            window.addEventListener("mouseup", this.endPan);
        };
        this.endPan = () => {
            window.removeEventListener("mousemove", this.pan);
            window.removeEventListener("mouseup", this.endPan);
        };
        this.pan = (event) => {
            event.preventDefault();
            this.posX += event.movementX;
            this.posY += event.movementY;
            this.setZoom();
        };
        this.setZoom = () => {
            if (!this.targetDiv) {
                this.targetDiv = document.getElementById("nodegraph");
            }
            this.targetDiv.style.transform = `translate(${this.posX}px, ${this.posY}px) scale(${this.zoomLvl})`;
        };
        this.clientToGraphPos = (clientX, clientY) => {
            if (!this.targetDiv) {
                this.targetDiv = document.getElementById("nodegraph");
            }
            let rect = this.targetDiv.getBoundingClientRect();
            let x = (clientX - rect.left) / this.zoomLvl;
            let y = (clientY - rect.top) / this.zoomLvl;
            return [x, y];
        };
    }
}
window['panzoom'] = new PanZoom();
//# sourceMappingURL=ZoomHandler.js.map