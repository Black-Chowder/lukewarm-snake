"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.SnakeController = exports.SNAKE_BODY_RADIUS = void 0;
const index_1 = require("../index");
const Vector_1 = require("../Core/Vector");
const stepPlaceDist = 10;
exports.SNAKE_BODY_RADIUS = 30;
const GROWTH_SEGMENTS = 10;
class SnakeController {
    parent;
    prevPos;
    prevPosPoints = [];
    curDistTraveled = 0;
    curDistProgress = 0;
    timeMod = 0;
    length = 10;
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
            //Maintain length of snake
            while (this.prevPosPoints.length > this.length) {
                this.prevPosPoints.shift();
            }
        }
    }
    eat() {
        this.length += GROWTH_SEGMENTS;
    }
    draw() {
        //Draw snake body
        for (let i = 0; i < this.prevPosPoints.length - 1; i++) {
            index_1.ctx.beginPath();
            index_1.ctx.fillStyle = 'green';
            let ballPos = Vector_1.Vector2.lerp(this.prevPosPoints[i], this.prevPosPoints[i + 1], this.curDistProgress);
            index_1.ctx.arc(ballPos.x, ballPos.y, exports.SNAKE_BODY_RADIUS, 0, Math.PI * 2);
            index_1.ctx.fill();
            index_1.ctx.closePath();
        }
    }
}
exports.SnakeController = SnakeController;
