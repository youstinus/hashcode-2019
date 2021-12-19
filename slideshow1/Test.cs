using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace slideshow1
{
    class Test
    {
        public void TestPhotos(List<Program.Photo> photos)
        {
            var tags = new List<string>();
            var dic = new Dictionary<string, int>();
            foreach (var photo in photos)
            {
                foreach (var photoTag in photo.Tags)
                {
                    tags.Add(photoTag);
                    if (dic.ContainsKey(photoTag))
                    {
                        dic[photoTag] = dic[photoTag] + 1;
                    }
                    else
                    {
                        dic.Add(photoTag, 1);
                    }
                }
            }

            var newDic = dic.OrderByDescending(x => x.Value);
            var count = 0;
            foreach (var i in newDic)
            {
                Console.WriteLine("{0} - {1}", i.Key, i.Value);
                if (count > 20)
                {
                    break;
                }

                count++;
            }
        }
    }
}
