"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Food = void 0;
const Entity_1 = require("./Entity");
const index_1 = require("../index");
const CollisionUtils_1 = require("../Core/CollisionUtils");
const SnakeController_1 = require("../Components/SnakeController");
const FOOD_RADIUS = 25;
class Food extends Entity_1.Entity {
    constructor(pos) {
        super();
        this.pos = pos;
        const fh = new FoodHandler(this);
        super.addComponent(fh);
    }
}
exports.Food = Food;
class FoodHandler {
    parent;
    constructor(parent) {
        this.parent = parent;
    }
    update() {
        const playerBody = index_1.player.snakeController.prevPosPoints;
        if ((0, CollisionUtils_1.CircleCollision)(this.parent.pos, FOOD_RADIUS, index_1.player.pos, SnakeController_1.SNAKE_BODY_RADIUS)) {
            index_1.player.snakeController.eat();
            this.parent.exists = false;
            console.log("Eaten");
        }
    }
    draw() {
        index_1.ctx.fillStyle = 'yellow';
        index_1.ctx.beginPath();
        index_1.ctx.arc(this.parent.pos.x, this.parent.pos.y, FOOD_RADIUS, 0, Math.PI * 2);
        index_1.ctx.fill();
        index_1.ctx.closePath();
    }
}
