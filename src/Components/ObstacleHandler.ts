import { IUpdate, IDraw } from './Component';
import { Vector2 } from '../Core/Vector';
import { Entity } from '../Entities/Entity';
import { player, ctx } from '../index';

export class ObstacleHandler implements IUpdate, IDraw {
    parent: Entity;
    heading: Vector2;
    radius: number;

    constructor(parent: Entity, heading: Vector2, radius: number) {
        this.parent = parent;
        this.heading = heading;
        this.radius = radius;
    }

    update(): void {
        //Move obstacle
        this.parent.deltaPos = this.heading.mul(player.snakeController.timeMod);
    }

    draw(): void {
        
        //Draw main body
        ctx.beginPath();
        ctx.fillStyle = 'black';
        ctx.arc(this.parent.pos.x, this.parent.pos.y, this.radius, 0, Math.PI * 2);
        ctx.fill();
        ctx.closePath();
    }
}