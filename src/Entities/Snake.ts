import { Entity } from './Entity';
import { SnakeController } from '../Components/SnakeController';
import { SnakeRenderer } from '../Components/SnakeRenderer';

export class Snake extends Entity {

    constructor() {
        super();

        const sc = new SnakeController(this);
        super.addComponent(sc);

        const sr = new SnakeRenderer(this);
        super.addComponent(sr);
    }
}