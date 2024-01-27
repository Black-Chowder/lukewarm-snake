import { IUpdate } from './Component';
import { Entity } from '../Entities/Entity';

export default class TDFriction implements IUpdate {
    parent: Entity;

    constructor(parent: Entity) {
        this.parent = parent;
    }

    update(): void {
        this.parent.deltaPos = this.parent.deltaPos.mul(0.9);
    }
}