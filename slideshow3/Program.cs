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

            var slides2 = Slides(pp);
            var resultSlides = new List<Slide>();

            //slides.ForEach(x => x.Score = TripleScore(slides.IndexOf(x), slides));
            //slides = slides.OrderBy(x => x.Score).ToList();
            var from = 0;
            var maxScore = 0;
            var maxSlides = new List<Slide>();
            var repeatCounter = 0;
            var rnd = new Random();
            int sc = 0;
            var nume = 100;
            for (int i = 0; i < nume; i++)
            {
                var slides = slides2.Skip(slides2.Count / nume * i).Take(slides2.Count / nume).ToList();


                while (!(Console.KeyAvailable && Console.ReadKey(true).Key == ConsoleKey.Spacebar))
                {
                    var changed = Evaluate2(from, slides);
                    if (repeatCounter > 100)
                    {
                        sc = AllScore(slides);
                        //Console.WriteLine(sc);
                        repeatCounter = 0;
                    }

                    if (!changed)
                    {
                        from++;
                    }
                    else
                    {
                        from = rnd.Next(0, slides.Count - 10);
                    }

                    if (from >= slides.Count - 5)
                    {
                        from = 0;
                    }

                    repeatCounter++;

                    if (sc > maxScore)
                    {
                        maxSlides = slides;
                        maxScore = sc;
                        Console.Clear();
                        Console.WriteLine(sc);
                    }
                }

                maxScore = 0;
                resultSlides.AddRange(maxSlides);
                maxSlides.Clear();
                Console.WriteLine($"{i}/{nume}");
            }
            
            var scrs = AllScore(resultSlides);
            Console.WriteLine(scrs);
            /*var groups = GroupTogether(slides);
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
            }*/

            Write($"out/{pout}", resultSlides);
            Console.WriteLine("Score {0}", AllScore(resultSlides));
            Console.ReadKey();
        }

        private static int AllScore(List<Slide> slides)
        {
            var score = 0;
            for (var i = 0; i < slides.Count - 1; i++)
            {
                score += GetMinScore(slides[i], slides[i + 1]);
            }

            return score;
        }

        private static bool Evaluate2(int from, List<Slide> slides)
        {
            for (var i = slides.Count - 1; i > from + 2; i--)
            {
                if (FutureBetter(from, i, slides))
                {
                    Insert(from, i, slides);
                    return true;
                }
            }

            return false;
        }

        private static bool Evaluate(int index, List<Slide> slides)
        {
            var minIndex = index; //slides.IndexOf(slides.OrderBy(x => x.Score).Skip(skip).Take(1).First());
            for (var i = 0; i < slides.Count; i++)
            {
                if (minIndex != i)
                {
                    var currentScore = slides[i].Score;
                    if (currentScore < slides[minIndex].Score)
                    {
                        var couldBeScore = GetMiddleScore(i, minIndex, slides);
                        if (couldBeScore > currentScore)
                        {
                            Swap(minIndex, i, slides);
                            return true;
                        }
                    }
                }
            }

            return false;
        }
        
        private static int GetMiddleScore(int i, int minIndex, List<Slide> slides)
        {
            var down = 0;
            var up = 0;
            if (i > 0)
            {
                down = GetMinScore(slides[i - 1], slides[minIndex]);
            }

            if (i < slides.Count - 1)
            {
                up = GetMinScore(slides[minIndex], slides[i + 1]);
            }

            return up + down;
        }

        private static int PossibleScore(int from, int i, List<Slide> slides)
        {
            return PossibleLeft(from, slides) + GetMiddleScore(i, from, slides);


            /*var pos1 = PossibleLeft(from, slides);
            var pos2 = GetMiddleScore(i, from, slides);
            return pos1 + pos2;
            **/

            //return GetMinScore(slides[i - 1], slides[0]) + GetMinScore(slides[i], slides[0]);
        }

        public static int CurrentScore(int from, int i, List<Slide> slides)
        {
            return TripleScore(from, slides) + DoubleScore(i, slides);

            /*var cur1 = TripleScore(from, slides);
            var cur2 = DoubleScore(i, slides);
            return cur1 + cur2;*/

            //return GetMinScore(slides[0], slides[1]) + GetMinScore(slides[i - 1], slides[i]);
        }

        public static bool FutureBetter(int from, int i, List<Slide> slides)
        {
            return PossibleScore(from, i, slides) > CurrentScore(from, i, slides);
        }

        private static int PossibleLeft(int from, List<Slide> slides)
        {
            return @from > 0 ? GetMinScore(slides[@from - 1], slides[@from + 1]) : 0;

            /*
            if (from > 0)
            {
                var left = GetMinScore(slides[from - 1], slides[from + 1]);
                return left;
            }

            return 0;*/
        }

        private static int TripleScore(int i, List<Slide> slides)
        {
            var down = 0;
            var up = 0;
            if (i > 0)
            {
                down = GetMinScore(slides[i - 1], slides[i]);
            }

            if (i < slides.Count - 1)
            {
                up = GetMinScore(slides[i], slides[i + 1]);
            }

            return up + down;
        }

        private static int DoubleScore(int i, List<Slide> slides)
        {
            return GetMinScore(slides[i], slides[i - 1]);
            /*if (i == 0)//slides.Count - 1)
            {
                return 0; //GetMinScore(slides[i], slides[i - 1]);
            }

            return GetMinScore(slides[i], slides[i - 1]);*/
        }

        private static void Insert(int from, int i, List<Slide> slides)
        {
            slides.Insert(i, slides[from]);
            slides.RemoveAt(from);
            /*slides[j].Score = TripleScore(j, slides);
            if (j - 1 >= 0)
                slides[j - 1].Score = TripleScore(j - 1, slides);
            if (j + 1 < slides.Count)
                slides[j + 1].Score = TripleScore(j + 1, slides);
            var indexOf = slides.IndexOf(tmp);*/
            /*slides[indexOf].Score = TripleScore(indexOf, slides);
            if (indexOf - 1 >= 0)
                slides[indexOf - 1].Score = TripleScore(indexOf - 1, slides);*/
        }

        private static void Swap(int i, int j, List<Slide> slides)
        {
            var tmp = slides[i];
            slides[i] = slides[j];
            slides[j] = tmp;
            slides[i].Score = TripleScore(i, slides);
            if(i - 1 >= 0)
                slides[i - 1].Score = TripleScore(i - 1, slides);
            if(i + 1 < slides.Count)
                slides[i + 1].Score = TripleScore(i + 1, slides);
            slides[j].Score = TripleScore(j, slides);
            if(j - 1 >= 0)
                slides[j - 1].Score = TripleScore(j - 1, slides);
            if(j + 1 < slides.Count)
                slides[j + 1].Score = TripleScore(j + 1, slides);
        }

        private static int GetMinScore(Slide a, Slide b)
        {
            var count1 = a.Tags.Count(x => b.Tags.Contains(x));
            var count2 = a.Tags.Count - count1;
            var count3 = b.Tags.Count - count1;
            return Math.Min(Math.Min(count1, count2), count3);
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
