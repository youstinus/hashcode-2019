using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace slideshow2
{
    class Photo
    {
        public int Number { get; set; }
        public List<string> Tags { get; set; }
        public int TagCount { get; set; }
        public int Type { get; set; } // 0 - horizontal, 1 - vertical

        public override string ToString()
        {
            return Number.ToString();
        }
    }
}
