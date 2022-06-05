import { Client } from 'pg';
import { dbConfig } from '../config/dbConfig'

export async function GetDataFromDatabase() {
    const client = new Client({
        user: dbConfig.USER,
        password: dbConfig.PASSWORD,
        host: dbConfig.HOST,
        database: dbConfig.DATABASE,
    })
    await client.connect()

    const wallsRaw = await client.query('SELECT * from walls');
    const floorsRaw = await client.query('SELECT * from floors');
    const plumbingFixturesRaw = await client.query('SELECT * from plumbingfixtures');

    await client.end()

    return {
        walls: wallsRaw.rows,
        floors: floorsRaw.rows,
        plumbingFixtures: plumbingFixturesRaw.rows
    };
}