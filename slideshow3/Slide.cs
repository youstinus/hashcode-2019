using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace slideshow2
{
    public class Slide
    {
        public int Id1 { get; set; }
        public int? Id2 { get; set; }
        public List<string> Tags { get; set; }
        public int Score { get; set; }
    }
}
