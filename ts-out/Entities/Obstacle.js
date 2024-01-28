"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Obstacle = void 0;
const Entity_1 = require("./Entity");
const ObstacleHandler_1 = require("../Components/ObstacleHandler");
class Obstacle extends Entity_1.Entity {
    constructor(pos, heading, radius) {
        super();
        this.pos = pos;
        let oh = new ObstacleHandler_1.ObstacleHandler(this, heading, radius);
        super.addComponent(oh);
    }
}
exports.Obstacle = Obstacle;
