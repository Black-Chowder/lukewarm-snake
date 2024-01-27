import { Vector2 } from '../Core/Vector';
import { IUpdate, IDraw } from '../Components/Component';

interface IEntity {
    pos: Vector2;
}

export abstract class Entity implements IEntity {
    pos: Vector2;
    deltaPos: Vector2;
    exists: boolean = true;

    private components: any[];
    private updateComponents: IUpdate[];
    private drawComponents: IDraw[];

    constructor(params?: IEntity){
        this.pos = params?.pos ?? Vector2.zero();
        this.deltaPos = Vector2.zero();
        
        this.components = [];
        this.updateComponents = [];
        this.drawComponents = [];
    }

    addComponent(component: any): void {
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

    update(): void {
        this.updateComponents.forEach(val => 
            val.update()
        );

        this.pos = this.pos.add(this.deltaPos);
    }

    draw(): void {
        this.drawComponents.forEach(val => 
            val.draw()
        );
    }
}