import { IUpdate, IDraw } from './Component';
import { Entity } from '../Entities/Entity';
import { ctx, keysDown } from '../index';

export class PlayerComponent implements IUpdate, IDraw {
    parent: Entity;

    constructor(parent: Entity) {
        this.parent = parent;
    }
    
    update(): void {
        if (keysDown['w']) {
            this.parent.deltaPos.y -= 2;
        }
        if (keysDown['s']) {
            this.parent.deltaPos.y += 2;
        }
        if (keysDown['a']) {
            this.parent.deltaPos.x -= 2;
        }
        if (keysDown['d']) {
            this.parent.deltaPos.x += 2;
        }
    }

    draw(): void {
        ctx.fillStyle = 'black';
        ctx.fillRect(this.parent.pos.x, this.parent.pos.y, 50, 50);
    }
}