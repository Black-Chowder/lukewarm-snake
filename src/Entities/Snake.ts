import { Entity } from './Entity';
import { SnakeController } from '../Components/SnakeController';
import { SnakeRenderer } from '../Components/SnakeRenderer';
import { Vector2 } from '../Core/Vector';
import { canvas } from '../index';

export class Snake extends Entity {
    snakeController: SnakeController;

    constructor() {
        super();
        this.pos = new Vector2(canvas.width / 2, canvas.height / 2);

        this.snakeController = new SnakeController(this);
        super.addComponent(this.snakeController);

        const sr = new SnakeRenderer(this);
        super.addComponent(sr);
    }
}