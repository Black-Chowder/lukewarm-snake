import { Entity } from './Entity';
import { IUpdate } from '../Components/Component';

export class GameManager extends Entity {
    gmh: GameManagerHandler;

    constructor() {
        super();

        this.gmh = new GameManagerHandler();
        super.addComponent(this.gmh);
    }
}

class GameManagerHandler implements IUpdate {
    obstacleSpawnRate: number = 4;

    constructor() {

    }

    update(): void {

    }
}