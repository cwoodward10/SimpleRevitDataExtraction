import { GetDataFromDatabase } from '../utilities/databaseUtilities';
import { Router } from 'express';

export const router = Router();

// simple route
router.get("/", (req, res) => {
    res.json({ message: "Welcome to the Cottage.DataExtraction API Page." });
});

router.get("/allrevitdata", async (req, res) => {
    const rawdata = await GetDataFromDatabase();

    res.json({
        walls: rawdata.walls,
        floors: rawdata.floors,
        plumbingFixtures: rawdata.plumbingFixtures,
    });
})
