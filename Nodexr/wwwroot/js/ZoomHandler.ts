
class PanZoom {

    posX: number = 0;
    posY: number = 0;
    zoomLvl: number = 1.0;
    targetDiv: HTMLElement = undefined;

    zoom = (event: WheelEvent) => {
        event.preventDefault();
        
        const scalingAmounts = {
            0: 1,
            1: 20,
            2: 1000,
        }
        let delta = -event.deltaY * 0.0007 * scalingAmounts[event.deltaMode];

        this.zoomAt(window.innerWidth * 0.45, window.innerHeight * 0.35, 1 + delta);
        this.setZoom();
    }

    zoomAt = (x: number, y: number, amount: number) => {
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
        (<any>window).DotNet.invokeMethodAsync('Nodexr', 'SetZoom', this.zoomLvl)
        let dX = (x - this.posX) * (1 - amount);
        let dY = (y - this.posY) * (1 - amount);
        
        this.posX += dX;
        this.posY += dY;
    }

    startPan = (event: Event) => {
        event.preventDefault();
        window.addEventListener("mousemove", this.pan);
        window.addEventListener("mouseup", this.endPan);
    }

    endPan = () => {
        window.removeEventListener("mousemove", this.pan);
        window.removeEventListener("mouseup", this.endPan);
    }

    pan = (event: MouseEvent) => {
        event.preventDefault();
        this.posX += event.movementX;
        this.posY += event.movementY;
        this.setZoom();
    }

    setZoom = () => {
        if (!this.targetDiv) {
            this.targetDiv = document.getElementById("nodegraph");
        }

        this.targetDiv.style.transform = `translate(${this.posX}px, ${this.posY}px) scale(${this.zoomLvl})`;
    }

    clientToGraphPos = (clientX: number, clientY: number): [number, number] => {
        if (!this.targetDiv) {
            this.targetDiv = document.getElementById("nodegraph");
        }
        let rect = this.targetDiv.getBoundingClientRect();
        let x = (clientX - rect.left) / this.zoomLvl;
        let y = (clientY - rect.top) / this.zoomLvl;
        
        return [x, y];
    }
}

window['panzoom'] = new PanZoom();