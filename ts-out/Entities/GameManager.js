"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.GameManager = void 0;
const Entity_1 = require("./Entity");
class GameManager extends Entity_1.Entity {
    gmh;
    constructor() {
        super();
        this.gmh = new GameManagerHandler();
        super.addComponent(this.gmh);
    }
}
exports.GameManager = GameManager;
class GameManagerHandler {
    obstacleSpawnRate = 4;
    constructor() {
    }
    update() {
    }
}
