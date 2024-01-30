import { Vector2 } from "./Vector";

export function CircleCollision(circ1Pos: Vector2, circ1Rad: number, circ2Pos: Vector2, circ2Rad: number): boolean {
    const dist: number = Math.sqrt((circ1Pos.x - circ2Pos.x) ** 2 + (circ1Pos.y - circ2Pos.y) ** 2);
    const sumOfRadii: number = circ1Rad + circ2Rad;

    return dist < sumOfRadii;
}