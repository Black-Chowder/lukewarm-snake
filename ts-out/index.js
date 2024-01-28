"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.mousePos = exports.keysDown = exports.entities = exports.ctx = exports.canvas = void 0;
const Vector_1 = require("./Core/Vector");
const Player_1 = require("./Entities/Player");
const Snake_1 = require("./Entities/Snake");
exports.canvas = document.getElementById('canvas');
exports.ctx = exports.canvas.getContext('2d');
exports.entities = [];
function main() {
    //Init stuff
    exports.entities.push(new Player_1.Player());
    exports.entities.push(new Snake_1.Snake());
    //Main loop
    function loop() {
        //Logic Loop
        for (let i = exports.entities.length - 1; i >= 0; i--) {
            exports.entities[i].update();
            if (!exports.entities[i].exists)
                exports.entities.splice(i, 1);
        }
        //Draw Loop
        exports.ctx.fillStyle = 'cornflowerblue';
        exports.ctx.fillRect(0, 0, exports.canvas.width, exports.canvas.height);
        exports.entities.forEach(val => {
            val.draw();
        });
        requestAnimationFrame(loop);
    }
    loop();
}
//Input handling
exports.keysDown = {};
document.addEventListener('keydown', e => {
    const key = e.key.toLowerCase();
    if (!(key in exports.keysDown))
        exports.keysDown[key] = true;
    else
        exports.keysDown[key] = true;
});
document.addEventListener('keyup', e => {
    exports.keysDown[e.key.toLowerCase()] = false;
});
exports.mousePos = Vector_1.Vector2.zero();
document.addEventListener('mousemove', e => {
    exports.mousePos.x = e.x;
    exports.mousePos.y = e.y;
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
