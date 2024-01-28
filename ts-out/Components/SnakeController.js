"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.SnakeController = void 0;
const index_1 = require("../index");
const Vector_1 = require("../Core/Vector");
const stepPlaceDist = 100;
class SnakeController {
    parent;
    prevPos;
    prevPosPoints = [];
    curDistTraveled = 0;
    curDistProgress = 0;
    timeMod = 0;
    length = 5;
    constructor(parent) {
        this.parent = parent;
        //Initialize previous position handling
        this.prevPos = this.parent.pos.cpy();
        this.prevPosPoints.push(this.parent.pos.cpy());
    }
    update() {
        //Move player to mouse position
        this.prevPos = this.parent.pos.cpy();
        this.parent.pos = index_1.mousePos;
        //Calculate distance traveled
        this.timeMod = this.prevPos.add(this.parent.pos.mul(-1)).mag();
        this.curDistTraveled += this.timeMod;
        this.curDistProgress = this.curDistTraveled / stepPlaceDist;
        //Add new point when traveled enough distance
        while (this.curDistTraveled > stepPlaceDist) {
            this.curDistTraveled -= stepPlaceDist;
            let deltaPos = this.prevPosPoints[this.prevPosPoints.length - 1].cpy();
            deltaPos = this.parent.pos
                .add(deltaPos.mul(-1))
                .norm()
                .mul(stepPlaceDist)
                .add(this.prevPosPoints[this.prevPosPoints.length - 1]);
            this.prevPosPoints.push(deltaPos);
        }
        //Maintain length of snake
        if (this.prevPosPoints.length > this.length) {
            this.prevPosPoints.shift();
        }
    }
    draw() {
        //Draw previous poisition points
        index_1.ctx.fillStyle = 'red';
        for (let i = 0; i < this.prevPosPoints.length; i++) {
            index_1.ctx.beginPath();
            index_1.ctx.fillStyle = 'red';
            index_1.ctx.arc(this.prevPosPoints[i].x, this.prevPosPoints[i].y, 5, 0, Math.PI * 2);
            index_1.ctx.fill();
            index_1.ctx.closePath();
        }
        //Draw snake body
        for (let i = 0; i < this.prevPosPoints.length - 1; i++) {
            index_1.ctx.beginPath();
            index_1.ctx.fillStyle = 'green';
            let ballPos = Vector_1.Vector2.lerp(this.prevPosPoints[i], this.prevPosPoints[i + 1], this.curDistProgress);
            index_1.ctx.arc(ballPos.x, ballPos.y, 30, 0, Math.PI * 2);
            index_1.ctx.fill();
            index_1.ctx.closePath();
        }
    }
}
exports.SnakeController = SnakeController;
