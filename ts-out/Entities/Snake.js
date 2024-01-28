"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Snake = void 0;
const Entity_1 = require("./Entity");
const SnakeController_1 = require("../Components/SnakeController");
const SnakeRenderer_1 = require("../Components/SnakeRenderer");
const Vector_1 = require("../Core/Vector");
const index_1 = require("../index");
class Snake extends Entity_1.Entity {
    constructor() {
        super();
        this.pos = new Vector_1.Vector2(index_1.canvas.width / 2, index_1.canvas.height / 2);
        const sc = new SnakeController_1.SnakeController(this);
        super.addComponent(sc);
        const sr = new SnakeRenderer_1.SnakeRenderer(this);
        super.addComponent(sr);
    }
}
exports.Snake = Snake;
