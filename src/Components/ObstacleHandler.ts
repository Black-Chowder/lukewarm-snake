import { IUpdate, IDraw } from './Component';
import { Vector2 } from '../Core/Vector';
import { Entity } from '../Entities/Entity';
import { player, ctx } from '../index';
import { SNAKE_BODY_RADIUS } from './SnakeController';
import { CircleCollision } from '../Core/CollisionUtils';

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

        if (CircleCollision(this.parent.pos, this.radius, player.pos, SNAKE_BODY_RADIUS)) {
            console.log("Hurt!");
            this.parent.exists = false;
        }
    }

    draw(): void {
        
        //Draw tail
        ctx.fillStyle = 'blue';
        ctx.beginPath();
        let startPos: Vector2 = this.parent.pos.add(Vector2.fromAngle(this.heading.angle() + Math.PI / 2).mul(this.radius));
        let middlePos: Vector2 = this.parent.pos.add(Vector2.fromAngle(this.heading.angle() + Math.PI).mul(this.radius * 3));
        let endPos: Vector2 = this.parent.pos.add(Vector2.fromAngle(this.heading.angle() - Math.PI / 2).mul(this.radius));
        ctx.moveTo(startPos.x, startPos.y);
        ctx.lineTo(middlePos.x, middlePos.y);
        ctx.lineTo(endPos.x, endPos.y);
        ctx.closePath();
        ctx.fill();

        //Draw main body
        ctx.beginPath();
        ctx.fillStyle = 'black';
        ctx.arc(this.parent.pos.x, this.parent.pos.y, this.radius, 0, Math.PI * 2);
        ctx.fill();
        ctx.closePath();
    }
}