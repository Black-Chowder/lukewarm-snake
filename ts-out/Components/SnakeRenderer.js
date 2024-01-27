"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.SnakeRenderer = void 0;
const index_1 = require("../index");
class SnakeRenderer {
    parent;
    constructor(parent) {
        this.parent = parent;
    }
    draw() {
        index_1.ctx.beginPath();
        index_1.ctx.fillStyle = '#000000';
        index_1.ctx.arc(this.parent.pos.x, this.parent.pos.y, 30, 0, Math.PI * 2);
        index_1.ctx.closePath();
        index_1.ctx.fill();
    }
}
exports.SnakeRenderer = SnakeRenderer;
