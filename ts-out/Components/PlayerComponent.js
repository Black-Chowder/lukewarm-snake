"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.PlayerComponent = void 0;
const index_1 = require("../index");
class PlayerComponent {
    parent;
    constructor(parent) {
        this.parent = parent;
    }
    update() {
        if (index_1.keysDown['w']) {
            this.parent.deltaPos.y -= 2;
        }
        if (index_1.keysDown['s']) {
            this.parent.deltaPos.y += 2;
        }
        if (index_1.keysDown['a']) {
            this.parent.deltaPos.x -= 2;
        }
        if (index_1.keysDown['d']) {
            this.parent.deltaPos.x += 2;
        }
    }
    draw() {
        index_1.ctx.fillStyle = 'black';
        index_1.ctx.fillRect(this.parent.pos.x, this.parent.pos.y, 50, 50);
    }
}
exports.PlayerComponent = PlayerComponent;
