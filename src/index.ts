import { Vector2 } from "./Core/Vector";
import { Entity } from "./Entities/Entity";
import { Snake } from './Entities/Snake';
import { Obstacle } from './Entities/Obstacle';

export const canvas = document.getElementById('canvas') as HTMLCanvasElement;
export const ctx = canvas.getContext('2d') as CanvasRenderingContext2D;

export const entities: Entity[] = [];

export let player: Snake;

function main() {
    //Init stuff
    player = new Snake();
    entities.push(player);
    entities.push(new Obstacle(
        new Vector2(canvas.width / 4, canvas.height / 4), 
        new Vector2(1, 1).norm().mul(0.2),
        25
    ));

    //Main loop
    function loop(){
        //Logic Loop
        for (let i = entities.length - 1; i >= 0; i--) {
            entities[i].update();

            if (!entities[i].exists)
                entities.splice(i, 1);
        }

        //Draw Loop
        ctx.fillStyle = 'cornflowerblue';
        ctx.fillRect(0, 0, canvas.width, canvas.height);
        entities.forEach(val => {
            val.draw();
        });

        
        requestAnimationFrame(loop);
    }
    loop();
}


//Input handling
export const keysDown: { [key: string]: boolean} = {};
document.addEventListener('keydown', e => {
    const key = e.key.toLowerCase();
    if (!(key in keysDown))
        keysDown[key] = true;
    else
        keysDown[key] = true;
});
document.addEventListener('keyup', e => {
    keysDown[e.key.toLowerCase()] = false;
});

export const mousePos: Vector2 = Vector2.zero();
document.addEventListener('mousemove', e => {
    mousePos.x = e.x;
    mousePos.y = e.y;
});

//TEMPORARY: Only begin program on mouse click
//TODO: Delete this and create proper start screen
let gameBegun = false;
document.addEventListener('click', e => {
    if (!gameBegun) {
        main();
        gameBegun = true;
    }
});