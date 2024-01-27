"use strict";
var __importDefault = (this && this.__importDefault) || function (mod) {
    return (mod && mod.__esModule) ? mod : { "default": mod };
};
Object.defineProperty(exports, "__esModule", { value: true });
exports.Player = void 0;
const Entity_1 = require("./Entity");
const PlayerComponent_1 = require("../Components/PlayerComponent");
const TDFriction_1 = __importDefault(require("../Components/TDFriction"));
class Player extends Entity_1.Entity {
    constructor() {
        super();
        const pc = new PlayerComponent_1.PlayerComponent(this);
        super.addComponent(pc);
        const friction = new TDFriction_1.default(this);
        super.addComponent(friction);
    }
}
exports.Player = Player;
