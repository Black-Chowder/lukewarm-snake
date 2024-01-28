"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.ObstacleHandler = void 0;
const Vector_1 = require("../Core/Vector");
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
        //Draw tail
        index_1.ctx.fillStyle = 'blue';
        index_1.ctx.beginPath();
        let startPos = this.parent.pos.add(Vector_1.Vector2.fromAngle(this.heading.angle() + Math.PI / 2).mul(this.radius));
        let middlePos = this.parent.pos.add(Vector_1.Vector2.fromAngle(this.heading.angle() + Math.PI).mul(this.radius * 3));
        let endPos = this.parent.pos.add(Vector_1.Vector2.fromAngle(this.heading.angle() - Math.PI / 2).mul(this.radius));
        index_1.ctx.moveTo(startPos.x, startPos.y);
        index_1.ctx.lineTo(middlePos.x, middlePos.y);
        index_1.ctx.lineTo(endPos.x, endPos.y);
        index_1.ctx.closePath();
        index_1.ctx.fill();
        //Draw main body
        index_1.ctx.beginPath();
        index_1.ctx.fillStyle = 'black';
        index_1.ctx.arc(this.parent.pos.x, this.parent.pos.y, this.radius, 0, Math.PI * 2);
        index_1.ctx.fill();
        index_1.ctx.closePath();
    }
}
exports.ObstacleHandler = ObstacleHandler;
