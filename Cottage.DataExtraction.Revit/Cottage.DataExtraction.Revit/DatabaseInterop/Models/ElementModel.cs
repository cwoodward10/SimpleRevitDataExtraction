using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Cottage.DataExtraction.Revit.DatabaseInterop.Models
{
    internal class ElementModel
    {
        public string Name { get; set; }
        public double Value { get; set; }

        public ElementModel(string name, double value)
        {
            Name = name;
            Value = value;
        }
    }
}
