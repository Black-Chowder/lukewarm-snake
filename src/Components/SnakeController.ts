import { IUpdate } from './Component';
import { Entity } from '../Entities/Entity';
import { mousePos } from '../index';

export class SnakeController implements IUpdate {
    parent: Entity;

    constructor(parent: Entity) {
        this.parent = parent;
    }

    update(): void {
        this.parent.pos = mousePos;
    }
}