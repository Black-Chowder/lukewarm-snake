"use strict";
Object.defineProperty(exports, "__esModule", { value: true });
exports.Entity = void 0;
const Vector_1 = require("../Core/Vector");
class Entity {
    pos;
    deltaPos;
    exists = true;
    components;
    updateComponents;
    drawComponents;
    constructor(params) {
        this.pos = params?.pos ?? Vector_1.Vector2.zero();
        this.deltaPos = Vector_1.Vector2.zero();
        this.components = [];
        this.updateComponents = [];
        this.drawComponents = [];
    }
    addComponent(component) {
        this.components.push(component);
        //If component is updatable, add to updatable list
        if ('update' in component && typeof component.update === 'function') {
            this.updateComponents.push(component);
        }
        //If component is drawable, add to drawable list
        if ('draw' in component && typeof component.draw === 'function') {
            this.drawComponents.push(component);
        }
    }
    update() {
        this.updateComponents.forEach(val => val.update());
        this.pos = this.pos.add(this.deltaPos);
    }
    draw() {
        this.drawComponents.forEach(val => val.draw());
    }
}
exports.Entity = Entity;
