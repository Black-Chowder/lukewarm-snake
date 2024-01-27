"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Vector2 = void 0;
class Vector2 {
    x;
    y;
    constructor(x, y) {
        this.x = x;
        this.y = y;
    }
    //Quick vector shortcuts
    static zero = () => new Vector2(0, 0);
    static one = () => new Vector2(1, 1);
    //Add functionality
    //Second parameter can be another vector or a constant
    static add = (a, b) => {
        if (b instanceof Vector2) {
            return new Vector2(a.x + b.x, a.y + b.y);
        }
        return new Vector2(a.x + b, a.y + b);
    };
    add = (other) => Vector2.add(this, other);
    //Multiply functionality
    //Second parameter can be vector or magnitude
    static mul = (a, b) => {
        if (b instanceof Vector2) {
            return new Vector2(a.x * b.x, a.y * b.y);
        }
        return new Vector2(a.x * b, a.y * b);
    };
    mul = (other) => Vector2.mul(this, other);
}
exports.Vector2 = Vector2;
