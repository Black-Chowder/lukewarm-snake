import { IDraw, IUpdate } from './Component';
import { Entity } from '../Entities/Entity';
import { mousePos, ctx } from '../index';
import { Vector2 } from '../Core/Vector';

const stepPlaceDist: number = 100;

export class SnakeController implements IUpdate, IDraw {
    parent: Entity;

    prevPos: Vector2;

    points: Vector2[] = [];
    curDistTraveled: number = 0;
    curDistProgress: number = 0;

    constructor(parent: Entity) {
        this.parent = parent;

        this.prevPos = this.parent.pos.cpy();
        this.points.push(this.parent.pos.cpy());
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

            let deltaPos: Vector2 = this.points[this.points.length - 1].cpy();
            deltaPos = this.parent.pos
                .add(deltaPos.mul(-1))
                .norm()
                .mul(stepPlaceDist)
                .add(this.points[this.points.length - 1]);

            this.points.push(deltaPos);
        }
    }

    draw(): void {
        ctx.fillStyle = 'red';
        for (let i = 0; i < this.points.length; i++) {
            ctx.fillRect(
                this.points[i].x,
                this.points[i].y,
                10,
                10
            );
        }
    }
}