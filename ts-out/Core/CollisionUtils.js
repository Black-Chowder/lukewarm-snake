"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.CircleCollision = void 0;
function CircleCollision(circ1Pos, circ1Rad, circ2Pos, circ2Rad) {
    const dist = Math.sqrt((circ1Pos.x - circ2Pos.x) ** 2 + (circ1Pos.y - circ2Pos.y) ** 2);
    const sumOfRadii = circ1Rad + circ2Rad;
    return dist < sumOfRadii;
}
exports.CircleCollision = CircleCollision;
