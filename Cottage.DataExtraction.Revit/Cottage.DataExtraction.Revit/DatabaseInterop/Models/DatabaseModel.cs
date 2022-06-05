using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cottage.DataExtraction.Revit.DatabaseInterop.Models
{
    internal class DatabaseModel
    {
        public List<ElementModel> Walls { get; set; }
        public List<ElementModel> Floors { get; set; }
        public List<ElementModel> PlumbingFixtures { get; set; }

        private DatabaseModel() { }

        public static DatabaseModel CreateDatabaseModelFromElements(IEnumerable<ElementModel> walls, IEnumerable<ElementModel> floors, IEnumerable<ElementModel> plumbingfixtures)
        {
            return new DatabaseModel()
            {
                Walls = walls.ToList(),
                Floors = floors.ToList(),
                PlumbingFixtures = plumbingfixtures.ToList(),
            };
        }
    }
}
