import { Entity } from './Entity';
import { Vector2 } from '../Core/Vector';
import { ObstacleHandler } from '../Components/ObstacleHandler';
import { player } from '../index';

export class Obstacle extends Entity {

    constructor(pos: Vector2, heading: Vector2, radius: number) {
        super();

        this.pos = pos;

        let oh = new ObstacleHandler(this, heading, radius);
        super.addComponent(oh);
    }
}