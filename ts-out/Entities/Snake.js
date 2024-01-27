"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Snake = void 0;
const Entity_1 = require("./Entity");
const SnakeController_1 = require("../Components/SnakeController");
const SnakeRenderer_1 = require("../Components/SnakeRenderer");
class Snake extends Entity_1.Entity {
    constructor() {
        super();
        const sc = new SnakeController_1.SnakeController(this);
        super.addComponent(sc);
        const sr = new SnakeRenderer_1.SnakeRenderer(this);
        super.addComponent(sr);
    }
}
exports.Snake = Snake;
