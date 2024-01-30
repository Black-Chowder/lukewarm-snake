import { Entity } from './Entity';
import { Vector2 } from '../Core/Vector';
import { IUpdate, IDraw } from '../Components/Component';
import { player, ctx } from '../index';
import { CircleCollision } from '../Core/CollisionUtils';
import { SNAKE_BODY_RADIUS } from '../Components/SnakeController';

const FOOD_RADIUS: number = 25;

export class Food extends Entity {

    constructor(pos: Vector2) {
        super();
        
        this.pos = pos;

        const fh = new FoodHandler(this);
        super.addComponent(fh);
    }
}

class FoodHandler implements IUpdate, IDraw {
    parent: Food;

    constructor(parent: Food) {
        this.parent = parent;
    }

    update(): void {
        //If colliding with snake head, have snake eat it
        if (CircleCollision(this.parent.pos, FOOD_RADIUS, player.pos, SNAKE_BODY_RADIUS)) {
            player.snakeController.eat();
            this.parent.exists = false;
        }
    }

    draw(): void {
        ctx.fillStyle = 'yellow';
        ctx.beginPath();
        ctx.arc(this.parent.pos.x, this.parent.pos.y, FOOD_RADIUS, 0, Math.PI * 2);
        ctx.fill();
        ctx.closePath();
    }
}