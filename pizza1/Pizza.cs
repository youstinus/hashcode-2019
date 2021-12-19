using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pizza1
{
    internal class Pizza
    {
        public int Rows { get; set; }
        public int Columns { get; set; }
        public int MinIngPerSlice { get; set; }
        public int MaxCellsPerSlice { get; set; }
        public int[,] Pad { get; set; }

        public Pizza()
        {

        }
    }
}
