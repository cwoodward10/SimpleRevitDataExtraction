using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

namespace Cottage.DataExtraction.Revit.ModelQuery
{
    internal static class ModelQueryUtilities
    {
        public static IEnumerable<Wall> GetModelWalls(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            return collector.WhereElementIsNotElementType().OfClass(typeof(Wall)).Cast<Wall>();
        }

        public static IEnumerable<Floor> GetModelFloors (Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);
            return collector.WhereElementIsNotElementType().OfClass(typeof(Floor)).Cast<Floor>();
        }

        public static IEnumerable<FamilyInstance> GetModelPlumbingFixtures(Document doc)
        {
            FilteredElementCollector collector = new FilteredElementCollector(doc);

            ElementCategoryFilter categoryRule = new ElementCategoryFilter(BuiltInCategory.OST_PlumbingFixtures);
            return collector.WhereElementIsNotElementType().OfClass(typeof(FamilyInstance)).WherePasses(categoryRule).Cast<FamilyInstance>();
        }
    }
}
