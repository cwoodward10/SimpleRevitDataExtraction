using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Autodesk.Revit.DB;

namespace Cottage.DataExtraction.Revit.RevitUtilities
{
    internal static class UtilityExtensions
    {
        public static double GetAggregateLength(this IEnumerable<Wall> walls)
        {
            IEnumerable<Parameter> lengthParameters = walls.Select(w => w.LookupParameter("Length")).Where(p => p != null);
            return lengthParameters.Sum(p => p.AsDouble());
        }

        public static double GetAggregateArea(this IEnumerable<Floor> floors)
        {
            IEnumerable<Parameter> areaParameters = floors.Select(w => w.LookupParameter("Area")).Where(p => p != null);
            return areaParameters.Sum(p => p.AsDouble());
        }
    }
}
