import { IDraw, IUpdate } from './Component';
import { Entity } from '../Entities/Entity';
import { mousePos, ctx } from '../index';
import { Vector2 } from '../Core/Vector';

const stepPlaceDist: number = 100;

export class SnakeController implements IUpdate, IDraw {
    parent: Entity;

    prevPos: Vector2;
    prevPosPoints: Vector2[] = [];
    curDistTraveled: number = 0;
    curDistProgress: number = 0;

    length: number = 5;

    constructor(parent: Entity) {
        this.parent = parent;

        //Initialize previous position handling
        this.prevPos = this.parent.pos.cpy();
        this.prevPosPoints.push(this.parent.pos.cpy());
    }

    update(): void {
        //Move player to mouse position
        this.prevPos = this.parent.pos.cpy();
        this.parent.pos = mousePos;
        
        //Calculate distance traveled
        this.curDistTraveled += this.prevPos.add(this.parent.pos.mul(-1)).mag();
        this.curDistProgress = this.curDistTraveled / stepPlaceDist;

        //Add new point when traveled enough distance
        while (this.curDistTraveled > stepPlaceDist) {
            this.curDistTraveled -= stepPlaceDist;

            let deltaPos: Vector2 = this.prevPosPoints[this.prevPosPoints.length - 1].cpy();
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

    draw(): void {
        //Draw previous poisition points
        ctx.fillStyle = 'red';
        for (let i = 0; i < this.prevPosPoints.length; i++) {
            ctx.beginPath();
            ctx.fillStyle = 'red';
            ctx.arc(this.prevPosPoints[i].x, this.prevPosPoints[i].y, 5, 0, Math.PI * 2);
            ctx.fill();
            ctx.closePath();
        }

        //Draw snake body
        for (let i = 0; i < this.prevPosPoints.length - 1; i++) {
            ctx.beginPath();
            ctx.fillStyle = 'green';
            let ballPos: Vector2 = Vector2.lerp(this.prevPosPoints[i], this.prevPosPoints[i + 1], this.curDistProgress);
            ctx.arc(ballPos.x, ballPos.y, 30, 0, Math.PI * 2);
            ctx.fill();
            ctx.closePath();
        }
    }
}