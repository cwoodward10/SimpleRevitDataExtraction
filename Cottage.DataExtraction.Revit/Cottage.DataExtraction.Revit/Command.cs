#region Namespaces

using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;

using Autodesk.Revit.ApplicationServices;
using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;

using Cottage.DataExtraction.Revit.ModelQuery;
using Cottage.DataExtraction.Revit.DatabaseInterop.Models;

#endregion

namespace Cottage.DataExtraction.Revit
{
    [Transaction(TransactionMode.Manual)]
    public class Command : IExternalCommand
    {
        public Result Execute(
          ExternalCommandData commandData,
          ref string message,
          ElementSet elements)
        {
            // basic revit document information
            UIApplication uiapp = commandData.Application;
            UIDocument uidoc = uiapp.ActiveUIDocument;
            Application app = uiapp.Application;
            Document doc = uidoc.Document;

            try
            {
                // get information from Revit
                List<Wall> walls = ModelQueryUtilities.GetModelWalls(doc).ToList();
                List<Floor> floors = ModelQueryUtilities.GetModelFloors(doc).ToList();
                List<FamilyInstance> plumbingFixtures = ModelQueryUtilities.GetModelPlumbingFixtures(doc).ToList();

                // convert to database friendly info & send to postgresql
                DatabaseModel databaseModel = DatabaseInterop.DatabaseUtilities.CreateDatabaseModelFromRevitElements(doc, walls, floors, plumbingFixtures);
                DatabaseInterop.DatabaseUtilities.TransmitDatabaseModel(databaseModel);

                TaskDialog successDialog = new TaskDialog("Success!");
                successDialog.MainContent = "Revit Data has been successfully stored in the Database.";
                successDialog.Show();

                return Result.Succeeded;
            }
            catch (Exception e)
            {
                TaskDialog errorDialog = new TaskDialog("Error Extracting Data");
                errorDialog.MainContent = e.Message;
                errorDialog.Show();

                return Result.Failed;
            }
        }
    }
}
