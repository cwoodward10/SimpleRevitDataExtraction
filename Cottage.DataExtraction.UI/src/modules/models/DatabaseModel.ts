import { ElementModel } from './ElementModel';

export interface DatabaseModel {
    walls: ElementModel[],
    floors: ElementModel[],
    plumbingFixtures: ElementModel[]
}