export class Vector2 {
    x: number;
    y: number;

    constructor(x: number, y: number) {
        this.x = x;
        this.y = y;
    }

    //Quick vector shortcuts
    static zero = (): Vector2 => new Vector2(0, 0);
    static one = (): Vector2 => new Vector2(1, 1);

    //Add functionality
    //Second parameter can be another vector or a constant
    static add = (a: Vector2, b: Vector2 | number): Vector2 => {
        if (b instanceof Vector2){
            return new Vector2(a.x + b.x, a.y + b.y);
        }
        return new Vector2(a.x + (b as number), a.y + (b as number));
    }
    add = (other: Vector2): Vector2 => Vector2.add(this, other);

    //Multiply functionality
    //Second parameter can be vector or magnitude
    static mul = (a: Vector2, b: Vector2 | number): Vector2 => {
        if (b instanceof Vector2) {
            return new Vector2(a.x * b.x, a.y * b.y);
        }
        return new Vector2(a.x * (b as number), a.y * (b as number));
    }
    mul = (other: Vector2 | number): Vector2 => Vector2.mul(this, other);
}