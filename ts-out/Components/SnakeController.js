"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.SnakeController = void 0;
const index_1 = require("../index");
const stepPlaceDist = 100;
class SnakeController {
    parent;
    prevPos;
    points = [];
    curDistTraveled = 0;
    curDistProgress = 0;
    constructor(parent) {
        this.parent = parent;
        this.prevPos = this.parent.pos.cpy();
        this.points.push(this.parent.pos.cpy());
    }
    update() {
        //Move player to mouse position
        this.prevPos = this.parent.pos.cpy();
        this.parent.pos = index_1.mousePos;
        //Calculate distance traveled
        this.curDistTraveled += this.prevPos.add(this.parent.pos.mul(-1)).mag();
        this.curDistProgress = this.curDistTraveled / stepPlaceDist;
        //Add new point when traveled enough distance
        while (this.curDistTraveled > stepPlaceDist) {
            this.curDistTraveled -= stepPlaceDist;
            let deltaPos = this.points[this.points.length - 1].cpy();
            deltaPos = this.parent.pos
                .add(deltaPos.mul(-1))
                .norm()
                .mul(stepPlaceDist)
                .add(this.points[this.points.length - 1]);
            this.points.push(deltaPos);
        }
    }
    draw() {
        index_1.ctx.fillStyle = 'red';
        for (let i = 0; i < this.points.length; i++) {
            index_1.ctx.fillRect(this.points[i].x, this.points[i].y, 10, 10);
        }
    }
}
exports.SnakeController = SnakeController;
