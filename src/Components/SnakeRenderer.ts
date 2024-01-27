import { IDraw } from './Component';
import { Entity } from '../Entities/Entity';
import { ctx } from '../index';

export class SnakeRenderer implements IDraw {
    parent: Entity;

    constructor(parent: Entity) {
        this.parent = parent;
    }

    draw(): void {
        ctx.beginPath();
        ctx.fillStyle = '#000000';
        ctx.arc(this.parent.pos.x, this.parent.pos.y, 30, 0, Math.PI * 2);
        ctx.closePath();
        ctx.fill();
    }
}