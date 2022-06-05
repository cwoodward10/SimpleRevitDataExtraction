import { DatabaseModel } from './models/DatabaseModel';
import { ElementModel } from './models/ElementModel';

export async function GetRevitDataFromDatabase() {
    const res = await fetch('http://localhost:8080/allrevitdata');

    const baseModel = {
        walls: [],
        floors: [],
        plumbingFixtures: []
    } as DatabaseModel;

    if (!res.ok) {
        return baseModel;
    }

    const raw = await res.json();

    if (raw.walls != null && (raw.walls as any[]).length > 0) {
        baseModel.walls = raw.walls.map((w: any) => {
            return {
                name: w.name, 
                value: Number.parseFloat(w.length)
            } as ElementModel;
        })
    }

    if (raw.floors != null && (raw.floors as any[]).length > 0) {
        baseModel.floors = raw.floors.map((f: any) => {
            return {
                name: f.name, 
                value: Number.parseFloat(f.area)
            } as ElementModel;
        })
    }

    if (raw.plumbingFixtures != null && (raw.plumbingFixtures as any[]).length > 0) {
        baseModel.plumbingFixtures = raw.plumbingFixtures.map((pf: any) => {
            return {
                name: pf.name, 
                value: Number.parseFloat(pf.count)
            } as ElementModel;
        })
    }

    return baseModel as DatabaseModel;
}