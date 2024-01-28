import { Entity } from './Entity';
import { SnakeController } from '../Components/SnakeController';
import { SnakeRenderer } from '../Components/SnakeRenderer';
import { Vector2 } from '../Core/Vector';
import { canvas } from '../index';

export class Snake extends Entity {

    constructor() {
        super();
        this.pos = new Vector2(canvas.width / 2, canvas.height / 2);

        const sc = new SnakeController(this);
        super.addComponent(sc);

        const sr = new SnakeRenderer(this);
        super.addComponent(sr);
    }
}