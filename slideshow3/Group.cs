using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace slideshow2
{
    public class Group
    {
        public Group Left { get; set; }
        public Group Right { get; set; }
        public Slide Data { get; set; }

        public Slide RightRecursive()
        {
            return Data ?? Right.RightRecursive();
        }

        public Slide LeftRecursive()
        {
            return Data ?? Left.LeftRecursive();
        }

        public void FlipRecursive()
        {
            if (Left == null || Right == null)
                return;

            var tmp = Right;
            Right = Left;
            Left = tmp;
            Left.FlipRecursive();
            Right.FlipRecursive();
        }

        public void ToList(List<Slide> slides)
        {
            if (Left == null || Right == null) // and ?
            {
                slides.Add(Data);
            }
            else
            {
                Left.ToList(slides);
                Right.ToList(slides);
            }
        }
    }
}
