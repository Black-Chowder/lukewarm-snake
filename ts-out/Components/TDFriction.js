"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
class TDFriction {
    parent;
    constructor(parent) {
        this.parent = parent;
    }
    update() {
        this.parent.deltaPos = this.parent.deltaPos.mul(0.9);
    }
}
exports.default = TDFriction;
