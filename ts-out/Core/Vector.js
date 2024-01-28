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
    //Creates a new Vector2 instance with the same values as the given vector
    static cpy = (a) => new Vector2(a.x, a.y);
    cpy = () => Vector2.cpy(this);
    //Gets the magnitude of a vector
    static mag = (a) => Math.sqrt(a.x * a.x + a.y * a.y);
    mag = () => Vector2.mag(this);
    //Return a normalized vector
    static norm = (a) => {
        let length = Math.sqrt(a.x * a.x + a.y * a.y);
        if (length !== 0)
            return new Vector2(a.x / length, a.y / length);
        return a.cpy();
    };
    norm = () => Vector2.norm(this);
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
    //Lerp between two vectors
    static lerp = (start, end, t) => {
        //Ensure t is between 1 and 0
        t = Math.max(0, Math.min(1, t));
        return new Vector2((1 - t) * start.x + t * end.x, (1 - t) * start.y + t * end.y);
    };
}
exports.Vector2 = Vector2;
