import { Entity } from './Entity';
import { PlayerComponent } from '../Components/PlayerComponent';
import TDFriction from '../Components/TDFriction';

export class Player extends Entity {

    constructor() {
        super();

        const pc = new PlayerComponent(this);
        super.addComponent(pc);

        const friction = new TDFriction(this);
        super.addComponent(friction);
    }
}