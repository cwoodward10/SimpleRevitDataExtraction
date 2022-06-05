import { DatabaseModel } from './models/DatabaseModel';

export async function GetRevitDataFromDatabase() {
    const res = await fetch('http://localhost:8080/allrevitdata');
    if (!res.ok) {
        return {
            walls: [],
            floors: [],
            plumbingFixtures: []
        }
    }

    const raw = await res.json();
    return raw as DatabaseModel;
}