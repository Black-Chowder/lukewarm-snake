"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.ObstacleHandler = void 0;
const index_1 = require("../index");
class ObstacleHandler {
    parent;
    heading;
    radius;
    constructor(parent, heading, radius) {
        this.parent = parent;
        this.heading = heading;
        this.radius = radius;
    }
    update() {
        //Move obstacle
        this.parent.deltaPos = this.heading.mul(index_1.player.snakeController.timeMod);
    }
    draw() {
        //Draw main body
        index_1.ctx.beginPath();
        index_1.ctx.fillStyle = 'black';
        index_1.ctx.arc(this.parent.pos.x, this.parent.pos.y, this.radius, 0, Math.PI * 2);
        index_1.ctx.fill();
        index_1.ctx.closePath();
    }
}
exports.ObstacleHandler = ObstacleHandler;
