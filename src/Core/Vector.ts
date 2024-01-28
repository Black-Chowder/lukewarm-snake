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

    //Creates a new Vector2 instance with the same values as the given vector
    static cpy = (a: Vector2): Vector2 => new Vector2(a.x, a.y);
    cpy = (): Vector2 => Vector2.cpy(this);

    //Gets the magnitude of a vector
    static mag = (a: Vector2): number => Math.sqrt(a.x * a.x + a.y * a.y);
    mag = (): number => Vector2.mag(this);

    //Return a normalized vector
    static norm = (a: Vector2): Vector2 => {
        let length = Math.sqrt(a.x * a.x + a.y * a.y);

        if (length !== 0) 
            return new Vector2(a.x / length, a.y / length);
        return a.cpy();
    }
    norm = (): Vector2 => Vector2.norm(this);

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