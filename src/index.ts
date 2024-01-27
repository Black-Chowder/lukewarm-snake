import { Vector2 } from "./Core/Vector";
import { Entity } from "./Entities/Entity";
import { Player } from "./Entities/Player";
import { Snake } from './Entities/Snake';

export const canvas = document.getElementById('canvas') as HTMLCanvasElement;
export const ctx = canvas.getContext('2d') as CanvasRenderingContext2D;

export const entities: Entity[] = [];

function main() {
    //Init stuff
    entities.push(new Player());
    entities.push(new Snake());

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


main();