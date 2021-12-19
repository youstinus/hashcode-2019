using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace slideshow2
{
    class Program
    {
        static void Main(string[] args)
        {
            Start();
        }

        private static void Start()
        {
            Console.WriteLine("a,b,c,d,e");
            var pp = new List<Photo>();
            var pout = "any";
            bool more = true;
            while (more)
            {
                switch (pout = Console.ReadLine())
                {
                    case "a":
                        pp = Read("in/a_example.txt");
                        pout = "a1.txt";
                        more = false;
                        break;
                    case "b":
                        pp = Read("in/b_lovely_landscapes.txt");
                        more = false;
                        pout = "b1.txt";
                        break;
                    case "c":
                        pp = Read("in/c_memorable_moments.txt");
                        more = false;
                        pout = "c1.txt";
                        break;
                    case "d":
                        pp = Read("in/d_pet_pictures.txt");
                        more = false;
                        pout = "d1.txt";
                        break;
                    case "e":
                        pp = Read("in/e_shiny_selfies.txt");
                        more = false;
                        pout = "e1.txt";
                        break;
                    default:
                        more = true;
                        break;
                }
            }

            var slides = Slides(pp);
            var groups = GroupTogether(slides);
            var resultSlides = new List<Slide>();
            var leftovers = new List<Group>();
            var result = Calculate(groups, leftovers);
            result.ToList(resultSlides);
            var lefties = new List<Slide>();
            leftovers.ForEach(x => x.ToList(lefties));
            while (leftovers.Count > 0)
            {
                leftovers = new List<Group>();
                result = Calculate(GroupTogether(lefties), leftovers);
                result.ToList(resultSlides);
                lefties = new List<Slide>();
                leftovers.ForEach(x => x.ToList(lefties));
            }

            Write($"out/{pout}", resultSlides);
            Console.WriteLine("Done");
            Console.ReadKey();
        }
        
        static Group Calculate(List<Group> groups, List<Group> leftovers)
        {
            while (true)
            {
                var biggerGroup = new List<Group>();
                while (groups.Count > 1)
                {
                    var first = groups.First();
                    var second = GetBestMatch(first, groups);
                    biggerGroup.Add(second);
                    Console.Clear();
                    Console.WriteLine(groups.Count);
                }

                if(groups.Count == 1)
                    leftovers.Add(groups.First());

                if (biggerGroup.Count == 1) return biggerGroup.First();
                groups = biggerGroup;
            }
        }

        static Group GetBestMatch(Group first, List<Group> groups)
        {
            Group best = groups[1];
            var index = 1;
            var score = GetScore(out var arrange, first, best);
            for (var i = 2; i < groups.Count; i++)
            {
                var arr = 0;
                var score1 = GetScore(out arr, first, groups[i]);
                if (score1 > score)
                {
                    score = score1;
                    arrange = arr;
                    index = i;
                }
            }

            var second = groups[index];
            var group = new Group();
            switch (arrange)
            {
                case 1:
                    group.Left = first;
                    group.Right = second;
                    break;
                case 2:
                    group.Left = second;
                    group.Right = first;
                    break;
                case 3:
                    first.FlipRecursive();
                    group.Left = first;
                    group.Right = second;
                    break;
                case 4:
                    second.FlipRecursive();
                    group.Left = first;
                    group.Right = second;
                    break;
            }

            groups.RemoveAt(index);
            groups.RemoveAt(0);

            return group;
        }

        static int GetScore(out int arrange, Group a, Group b)
        {
            var aLeft = a.LeftRecursive();
            var aRight = a.RightRecursive();
            var bLeft = b.LeftRecursive();
            var bRight = b.RightRecursive();

            var a1 = GetMinScore(aRight, bLeft);
            var a2 = GetMinScore(aLeft, bRight);
            var a3 = GetMinScore(aLeft, bLeft);
            var a4 = GetMinScore(aRight, bRight);

            var min12 = Math.Min(a1, a2);
            var min34 = Math.Min(a3, a4);

            if (min12 >= min34)
            {
                if (min12 == a1)
                {
                    arrange = 1;
                    return a1;
                }

                arrange = 2;
                return a2;
            }

            if (min34 == a3)
            {
                arrange = 3;
                return a3;
            }

            arrange = 4;
            return a4;
        }

        static int GetMinScore(Slide a, Slide b)
        {
            var count1 = a.Tags.Count(x => b.Tags.Contains(x));
            var count2 = a.Tags.Count - count1;
            var count3 = b.Tags.Count - count1;
            return Math.Min(Math.Min(count1, count2), count3);
        }

        static List<Group> GroupTogether(List<Slide> slides)
        {
            return slides.Select(x => new Group() {Data = x}).ToList();
        }

        static List<Slide> Slides(List<Photo> data)
        {
            var hor = new List<Slide>();
            var ver = new List<Slide>();

            var horList = new List<Photo>();
            var verList = new List<Photo>();

            foreach (var photo in data)
            {
                if (photo.Type == 0)
                {
                    hor.Add(new Slide()
                    {
                        Id1 = photo.Number,
                        Tags = photo.Tags
                    });
                }
                else
                {
                    verList.Add(photo);
                }
            }

            verList = verList.OrderByDescending(x => x.Tags.Count).ToList();
            hor = hor.OrderByDescending(x => x.Tags.Count).ToList();

            for (var i = 0; i < verList.Count / 2; i++)
            {
                var index = verList.Count - i - 1;
                ver.Add(new Slide()
                {
                    Id1 = verList[i].Number,
                    Id2 = verList[index].Number,
                    Tags = verList[i].Tags.Concat(verList[index].Tags).Distinct().ToList()
                });
            }

            var slidesAll = hor.Concat(ver).OrderByDescending(x => x.Tags.Count).ToList();
            //Console.WriteLine("Done slides {0}", slidesAll.First().Tags.Count);
            return slidesAll;
        }

        static List<Photo> Read(string path)
        {
            var list = new List<Photo>();
            int lines = 0;
            using (var reader = new StreamReader(File.Open(path, FileMode.Open), Encoding.ASCII))
            {
                string line = null;
                lines = Convert.ToInt32(reader.ReadLine());
                var counter = 0;
                while ((line = reader.ReadLine()) != null)
                {
                    int type = -1;
                    var parts = line.Split(' ');
                    switch (parts[0])
                    {
                        case "H":
                            type = 0;
                            break;
                        case "V":
                            type = 1;
                            break;
                        default:
                            Console.WriteLine("Something wrong");
                            break;
                    }

                    var tagCount = Convert.ToInt32(parts[1]);
                    var tags = new List<string>();
                    for (var i = 2; i < tagCount + 2; i++)
                    {
                        tags.Add(parts[i]);
                    }

                    var photo = new Photo()
                    {
                        TagCount = tagCount,
                        Type = type,
                        Tags = tags,
                        Number = counter
                    };
                    list.Add(photo);
                    counter++;
                }
            }

            return list;
        }

        static void Write(string path, List<Slide> list)
        {
            // HasBiggerScore(path);
            //if (HasBiggerScore(path)) return;

            using (var writer = new StreamWriter(File.Open(path, FileMode.Create), Encoding.ASCII))
            {
                var count = list.Count;
                writer.WriteLine(count); // how many horizontal + vertical/2
                for (var i = 0; i < count; i++)
                {
                    if (list[i].Id2 == null)
                    {
                        writer.WriteLine(list[i].Id1);
                    }
                    else
                    {
                        writer.WriteLine("{0} {1}", list[i].Id1, list[i].Id2);
                    }
                }
            }
        }
    }
}
