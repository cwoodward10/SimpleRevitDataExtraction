using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Cottage.DataExtraction.Revit.DatabaseInterop.Models;
using Cottage.DataExtraction.Revit.RevitUtilities;

using Autodesk.Revit.DB;

using Npgsql;

namespace Cottage.DataExtraction.Revit.DatabaseInterop
{
    internal static class DatabaseUtilities
    {
        const string connStr = "Server=localhost;Port=5432;User Id=postgres;Password=secret;Database=cottagedataextraction;Pooling=false;";

        const string databaseName = "cottagedataextraction";
        const string wallTableName = "walls";
        const string floorTableName = "floors";
        const string plumbingFixtureTableName = "plumbingfixtures";

        public static DatabaseModel CreateDatabaseModelFromRevitElements(Document doc,
                                                                         List<Wall> walls,
                                                                         List<Floor> floors,
                                                                         List<FamilyInstance> plumbingFixtures)
        {
            IEnumerable<ElementModel> dbWalls = walls.GroupBy(w => w.WallType.Id).Select(wallGroup =>
            {
                WallType wallType = doc.GetElement(wallGroup.Key) as WallType;
                string fullTypeName = wallType.FamilyName + ": " + wallType.Name;
                return new ElementModel(fullTypeName, wallGroup.GetAggregateLength());
            });

            IEnumerable<ElementModel> dbFloors = floors.GroupBy(f => f.FloorType.Id).Select(floorGroup =>
            {
                FloorType floorType = doc.GetElement(floorGroup.Key) as FloorType;
                string fullTypeName = floorType.FamilyName + ": " + floorType.Name;
                return new ElementModel(fullTypeName, floorGroup.GetAggregateArea());
            });

            IEnumerable<ElementModel> dbPlumbingFixtures = plumbingFixtures.GroupBy(pf => pf.GetTypeId()).Select(pfGroup =>
            {
                FamilySymbol familyType = doc.GetElement(pfGroup.Key) as FamilySymbol;
                if (familyType == null)
                {
                    return null;
                }

                string fullTypeName = familyType.FamilyName + ": " + familyType.Name;
                return new ElementModel(fullTypeName, pfGroup.Count());
            }).Where(em => em != null);
            
            return DatabaseModel.CreateDatabaseModelFromElements(dbWalls, dbFloors, dbPlumbingFixtures);
        }

        public static void TransmitDatabaseModel(DatabaseModel databaseModel)
        {
            CreateDatabaseIfNotExists();
            PopulateTables(databaseModel);
        }

        public static void CreateDatabaseIfNotExists()
        {
            string connStrNoDb = "Server=localhost;Port=5432;User Id=postgres;Password=secret;Pooling=false;";
            NpgsqlConnection connDropAndCreateDb = new NpgsqlConnection(connStrNoDb);
            connDropAndCreateDb.Open();
            try
            {
                // try to create the table if not exists
                NpgsqlCommand cmdDropDatabase = new NpgsqlCommand($@"DROP DATABASE IF EXISTS {databaseName};", connDropAndCreateDb);
                cmdDropDatabase.ExecuteNonQuery();
                NpgsqlCommand cmdCreateDatabase = new NpgsqlCommand($@"CREATE DATABASE {databaseName};", connDropAndCreateDb);
                cmdCreateDatabase.ExecuteNonQuery();
                connDropAndCreateDb.Close();
            }
            catch (Exception e)
            {
                connDropAndCreateDb.Close();
                throw e;
            }

            NpgsqlConnection connCreateTables = new NpgsqlConnection(connStr);
            connCreateTables.Open();
            using (NpgsqlTransaction trxCreateTables = connCreateTables.BeginTransaction())
            {
                try
                {
                    NpgsqlCommand cmdCreateWallsTable = new NpgsqlCommand($@"
                        CREATE TABLE IF NOT EXISTS {wallTableName} (
                            name text PRIMARY KEY,
                            length decimal
                        )
                    ", connCreateTables);

                    NpgsqlCommand cmdCreateFloorsTable = new NpgsqlCommand($@"
                        CREATE TABLE IF NOT EXISTS {floorTableName} (
                            name text PRIMARY KEY,
                            area decimal
                        )
                    ", connCreateTables);

                    NpgsqlCommand cmdCreatePlumbingFixtureTable = new NpgsqlCommand($@"
                        CREATE TABLE IF NOT EXISTS {plumbingFixtureTableName} (
                            name text PRIMARY KEY,
                            count int
                        )
                    ", connCreateTables);

                    cmdCreateWallsTable.ExecuteNonQuery();
                    cmdCreateFloorsTable.ExecuteNonQuery();
                    cmdCreatePlumbingFixtureTable.ExecuteNonQuery();

                    trxCreateTables.Commit();
                    connCreateTables.Close();
                }
                catch (Exception e)
                {
                    trxCreateTables.Rollback();
                    connCreateTables.Close();
                    throw e;
                }
            }
        }

        public static void PopulateTables(DatabaseModel databaseModel)
        {
            NpgsqlConnection connPopulateTables = new NpgsqlConnection(connStr);
            connPopulateTables.Open();
            using (NpgsqlTransaction trxPopulateTables = connPopulateTables.BeginTransaction())
            {
                try
                {
                    foreach (ElementModel elementModel in databaseModel.Walls)
                    {
                        NpgsqlCommand cmd = new NpgsqlCommand($@"
                            INSERT INTO {wallTableName} (name, length) 
                            VALUES ('{elementModel.Name}', {elementModel.Value})
                            ", connPopulateTables);
                        cmd.ExecuteNonQuery();
                    }

                    foreach (ElementModel elementModel in databaseModel.Walls)
                    {
                        NpgsqlCommand cmd = new NpgsqlCommand($@"
                            INSERT INTO {floorTableName} (name, area) 
                            VALUES ('{elementModel.Name}', {elementModel.Value})
                            ", connPopulateTables);
                        cmd.ExecuteNonQuery();
                    }

                    foreach (ElementModel elementModel in databaseModel.Walls)
                    {
                        NpgsqlCommand cmd = new NpgsqlCommand($@"
                            INSERT INTO {plumbingFixtureTableName} (name, count) 
                            VALUES ('{elementModel.Name}', {elementModel.Value})
                            ", connPopulateTables);
                        cmd.ExecuteNonQuery();
                    }

                    trxPopulateTables.Commit();
                    connPopulateTables.Close();
                }
                catch (Exception e)
                {
                    trxPopulateTables.Rollback();
                    connPopulateTables.Close();
                    throw e;
                }
            }
        }
    }
}
