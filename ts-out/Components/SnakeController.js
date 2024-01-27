"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.SnakeController = void 0;
const index_1 = require("../index");
class SnakeController {
    parent;
    constructor(parent) {
        this.parent = parent;
    }
    update() {
        this.parent.pos = index_1.mousePos;
    }
}
exports.SnakeController = SnakeController;
