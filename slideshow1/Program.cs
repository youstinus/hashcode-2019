using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using GeneticSharp.Domain;
using GeneticSharp.Domain.Chromosomes;
using GeneticSharp.Domain.Crossovers;
using GeneticSharp.Domain.Fitnesses;
using GeneticSharp.Domain.Mutations;
using GeneticSharp.Domain.Populations;
using GeneticSharp.Domain.Randomizations;
using GeneticSharp.Domain.Selections;
using GeneticSharp.Domain.Terminations;

namespace slideshow1
{
    public class MyProblemFitness : IFitness
    {
        public double Evaluate(IChromosome chromosome)
        {
            // Evaluate the fitness of chromosome.
            var genes = chromosome.GetGenes();
            Program.Slide a;
            Program.Slide b;
            double mainScore = 0;
            for (var i = 0; i < Program._slides.Count - 1; i++)
            {
                a = Program._slides[(int)genes[i].Value];
                b = Program._slides[(int)genes[i + 1].Value];
                mainScore += Score(a, b);
            }

            var score = mainScore;
            //Console.WriteLine("{0:F2}", score / 2000);
            return score;
        }

        private double Score(Program.Slide a, Program.Slide b)
        {
            var count1 = a.Tags.Count(x => b.Tags.Contains(x));
            var count2 = a.Tags.Count - count1;
            var count3 = b.Tags.Count - count1;
            return Math.Min(Math.Min(count1, count2), count3);
        }
    }

    public class MyProblemChromosome : ChromosomeBase
    {
        // Change the argument value passed to base constructor to change the length of your chromosome
        public MyProblemChromosome() : base(Program._slides.Count)
        {
            // CreateGenes();
            var m_numberOfCities = Program._slides.Count;
            // uncomment
            var rnd = new Random(Convert.ToInt32(DateTime.Now.Second));
            var nextas = rnd.Next(1000);
            if (nextas > 1000)
            {
                
                for (int i = 0; i < m_numberOfCities; i++)
                {
                    // uncomment
                    ReplaceGene(i, new Gene(i));
                }
            }
            else
            {
                var citiesIndexes = RandomizationProvider.Current.GetUniqueInts(m_numberOfCities, 0, m_numberOfCities);
                for (int i = 0; i < m_numberOfCities; i++)
                {
                    // uncomment
                    ReplaceGene(i, new Gene(citiesIndexes[i]));
                }
            }
            
        }

        public override Gene GenerateGene(int geneIndex)
        {
            // Generate a gene base on my problem chromosome representation
            return new Gene(RandomizationProvider.Current.GetInt(0, Program._slides.Count));

        }

        public override IChromosome CreateNew()
        {
            return new MyProblemChromosome();
        }
    }

    public class Program
    {
        public static List<Slide> _slides;
        public static List<Slide> _slides2;
        public static List<Photo> _photos;
        public static double _score;

        static void Main(string[] args)
        {
            Start();

            // testing tag count
            /*var test = new Test();
            test.TestPhotos(_photos);*/

            Console.ReadKey();
        }

        private static void Start()
        {
            Console.WriteLine("a,b,c,d,e");
            var pp = new List<Photo>();
            var pout = "any";
            bool more = true;
            bool evolve = false;
            while (more)
            {
                switch (pout = Console.ReadLine())
                {
                    case "a":
                        pp = Read("in/a_example.txt");
                        more = false;
                        break;
                    case "b":
                        pp = Read("in/b_lovely_landscapes.txt");
                        more = false;
                        break;
                    case "c":
                        pp = Read("in/c_memorable_moments.txt");
                        more = false;
                        break;
                    case "d":
                        pp = Read("in/d_pet_pictures.txt");
                        more = false;
                        break;
                    case "e":
                        pp = Read("in/e_shiny_selfies.txt");
                        more = false;
                        break;
                    case "a1":
                        pp = Read("in/a_example.txt");
                        pp = ReadSlidesAndData("in/a1.txt", pp);
                        pout = "a";
                        more = false;
                        evolve = true;
                        break;
                    case "b1":
                        pp = Read("in/b_lovely_landscapes.txt");
                        pp = ReadSlidesAndData("in/b1.txt", pp);
                        pout = "b";
                        more = false;
                        evolve = true;
                        break;
                    case "c1":
                        pp = Read("in/c_memorable_moments.txt");
                        pp = ReadSlidesAndData("in/c1.txt", pp);
                        pout = "c";
                        more = false;
                        evolve = true;
                        break;
                    case "d1":
                        pp = Read("in/d_pet_pictures.txt");
                        pp = ReadSlidesAndData("in/d1.txt", pp);
                        pout = "d";
                        more = false;
                        evolve = true;
                        break;
                    case "e1":
                        pp = Read("in/e_shiny_selfies.txt");
                        pp = ReadSlidesAndData("in/e1.txt", pp);
                        pout = "e";
                        more = false;
                        evolve = true;
                        break;
                    default:
                        more = true;
                        break;
                }
            }

            _photos = pp;
            var outPath = pout;
            _slides = !evolve ? Slides3(_photos) : Slides33(_photos);

            var selection = new EliteSelection(); // EliteSelection();RouletteWheelSelection
            var crossover = new PositionBasedCrossover();//PositionBasedCrossover(); //OrderedCrossover(); // positionbased,  alternatingpositionc, partialy mapped, AlternatingPositionCrossover
            var mutation = new TworsMutation(); //PartialShuffleMutation();//ReverseSequenceMutation(); TworsMutation(); // //
            var fitness = new MyProblemFitness();
            var chromosome = new MyProblemChromosome();
            var population = new Population(10, 10, chromosome);

            var ga = new GeneticAlgorithm(population, fitness, selection, crossover, mutation);
            ga.MutationProbability = 0.15f;
            //ga.CrossoverProbability = 0f;
            
            ga.Termination = new OrTermination(new GenerationNumberTermination(5000), new TimeEvolvingTermination(TimeSpan.FromMinutes(10)));
            Console.WriteLine("GA running...");

            ga.GenerationRan += delegate
            {
                DrawSampleName(pout);

                var bestChromosome = ga.Population.BestChromosome;
                //Console.WriteLine("Termination: {0}", terminationName);
                Console.WriteLine("Generations: {0}", ga.Population.GenerationsNumber);
                Console.WriteLine("Fitness: {0,10}", bestChromosome.Fitness);
                Console.WriteLine("Time: {0}", ga.TimeEvolving);
                Console.WriteLine("Speed (gen/sec): {0:0.00}", ga.Population.GenerationsNumber / ga.TimeEvolving.TotalSeconds);
                //sampleController.Draw(bestChromosome);
            };

            // for GA
            ga.Start();
            _score = ga.BestChromosome.Fitness ?? 0;
            Console.WriteLine("Best solution found has {0} fitness.", ga.BestChromosome.Fitness);
            
            /*var slidesAll = Slides3(pp);
            var slides = Slides4(slidesAll);*/

            // for GA
            _slides2 = new List<Slide>();
            foreach (var gene in ga.BestChromosome.GetGenes())
            {
                _slides2.Add(_slides[(int)gene.Value]);
            }

            // for normal
            //_slides2 = slides;

            Console.WriteLine($"Score {CountScore(_slides2)}");
            Write2($"out/{outPath}.txt", _slides2);

            Console.WriteLine("Done");
        }

        private static void DrawSampleName(string selectedSampleName)
        {
            Console.Clear();

            Console.ForegroundColor = ConsoleColor.DarkGreen;
            Console.WriteLine("GeneticSharp - ConsoleApp");
            Console.WriteLine();
            Console.WriteLine(selectedSampleName);
            Console.ResetColor();
        }


        static List<Slide> Slides4(List<Slide> slides)
        {
            var max = 50;
            var main = new List<Slide>();
            for (var i = 0; i < max; i++)
            {
                var slidePart = slides.Skip(i * (slides.Count / max)).Take(slides.Count / max).ToList();
                main = main.Concat(Slides5(slidePart)).ToList();
                Console.WriteLine(i);
            }

            return main;
        }

        static List<Slide> Slides5(List<Slide> slides)
        {
            var sorted = new List<Slide>();
            var counte = slides.Count;
            for (int i = 0; i < counte; i++)
            {
                var min = 0;
                var jj = 0;
                Slide maxSlide = null;
                for (int j = 0; j < counte; j++)
                {
                    if (slides[i] != slides[j])
                    {
                        var score = GetScore2(slides[i], slides[j]);
                        if (min < score)
                        {
                            min = score;
                            maxSlide = slides[i];
                            jj = j;
                        }
                    }

                }

                if (maxSlide != null)
                {
                    sorted.Add(maxSlide);
                    slides[i] = slides[jj];
                    slides.RemoveAt(jj);
                    i = 0;
                    counte--;
                }
                else
                {
                    sorted.Add(slides[0]);
                    slides.RemoveAt(0);
                    i = 0;
                    counte--;
                }

                //Console.WriteLine("Juda");
            }

            return sorted.Concat(slides).ToList();
        }

        static List<Slide> Slides33(List<Photo> data)
        {
            var hor = new List<Slide>();

            for (var i=0;i<data.Count;i++)
            {
                if (data[i].Type == 0)
                {
                    hor.Add(new Slide()
                    {
                        Id1 = data[i].Number,
                        Tags = data[i].Tags
                    });
                }
                else
                {
                    hor.Add(new Slide()
                    {
                        Id1 = data[i].Number,
                        Id2 = data[i+1].Number,
                        Tags = data[i].Tags.Concat(data[i+1].Tags).Distinct().ToList()
                    });
                    i++;
                }
            }
            
            return hor;
        }

        static List<Slide> Slides3(List<Photo> data)
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

        static int GetScore(Photo a, Photo b)
        {
            var count1 = a.Tags.Count(x => b.Tags.Contains(x));
            var count2 = a.Tags.Count - count1;
            var count3 = b.Tags.Count - count1;
            return Math.Min(Math.Min(count1, count2), count3);
        }

        static int GetScore2(Slide a, Slide b)
        {
            var count1 = a.Tags.Count(x => b.Tags.Contains(x));
            var count2 = a.Tags.Count - count1;
            var count3 = b.Tags.Count - count1;
            return Math.Min(Math.Min(count1, count2), count3);
        }

        static List<Photo> Slides2(List<Photo> list)
        {
            // list.OrderByDescending(x => x.TagCount);
            var scores = new List<int>();
            var slides = new List<Photo>();
            var lengthFirst = list.Count;

            for (var i = 0; i < lengthFirst - 1; i++)
            {
                var ph = list[i];
                if (ph.Type == 0)
                {
                    var min = 0;
                    Photo minPh = null;
                    Photo minPh2 = null;
                    for (var j = i + 1; j < lengthFirst; j++)
                    {
                        var ph2 = list[j];
                        if (ph2.Type == 0 && ph.Number != ph2.Number)
                        {
                            var score = GetScore(ph, ph2);
                            //slides.Add(list[i]);
                            //slides.Add(list[j]);
                            // if(!scores.Contains(score))
                            if (min < score)
                            {
                                min = score;
                                minPh = ph;
                                minPh2 = ph2;
                            }

                            //Console.WriteLine(score);
                            //scores.Add(score);
                        }
                    }

                    if (minPh != null)
                        slides.Add(minPh);
                    if (minPh2 != null)
                        slides.Add(minPh2);
                    list.Remove(minPh);
                    list.Remove(minPh2);
                    lengthFirst -= 2;
                }

                //Console.Clear();
                //Console.Write(list.Count - i);
            }
            /*scores.Distinct();
            foreach (var a in scores)
            {
                Console.WriteLine(a);
            }*/

            return slides;
        }

        static List<Photo> Slides(List<Photo> list)
        {
            // list.OrderByDescending(x => x.TagCount);
            var scores = new List<int>();
            var slides = new List<Photo>();
            for (var i = 0; i < list.Count - 1; i++)
            {
                if (list[i].Type == 0)
                {
                    for (var j = i + 1; j < list.Count; j++)
                    {
                        if (list[j].Type == 0)
                        {
                            var score = GetScore(list[i], list[j]);
                            //slides.Add(list[i]);
                            //slides.Add(list[j]);
                            // if(!scores.Contains(score))
                            Console.WriteLine(score);
                            scores.Add(score);

                        }
                    }
                }

                //Console.Clear();
                //Console.Write(list.Count - i);
            }

            scores.Distinct();
            foreach (var a in scores)
            {
                Console.WriteLine(a);
            }

            return slides;
        }

        public class Slide
        {
            public int Id1 { get; set; }
            public int? Id2 { get; set; }
            public List<string> Tags { get; set; }
        }

        public class Photo
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

        static void Write(string path, List<Photo> list)
        {
            using (var writer = new StreamWriter(File.Open(path, FileMode.Create), Encoding.ASCII))
            {
                var count = list.Count(x => x.Type == 0) + list.Count(x => x.Type == 1) / 2;
                writer.WriteLine(count); // how many horizontal + vertical/2
                for (var i = 0; i < count; i++)
                {
                    if (list[i].Type == 0)
                    {
                        writer.WriteLine(list[i]);
                    }
                    else
                    {
                        writer.WriteLine("{0} {1}", list[i], list[i + 1]);
                        i++;
                    }
                }
            }
        }

        static void Write2(string path, List<Slide> list)
        {
            // HasBiggerScore(path);
            if (HasBiggerScore(path)) return;

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

        private static bool HasBiggerScore(string path)
        {
            var slides3 = new List<Slide>();
            using (var reader = new StreamReader(File.Open(path, FileMode.Open), Encoding.ASCII))
            {
                var line = reader.ReadLine();
                while ((line = reader.ReadLine()) != null)
                {
                    var parts = line.Split(' ').Select(x => Convert.ToInt32(x)).ToArray();
                    var slide = new Slide
                    {
                        Id1 = parts[0]
                    };

                    if (parts.Length == 2)
                    {
                        slide.Id2 = parts[1];
                    }
                    slides3.Add(slide);
                }
            }

            var over = CountScore(slides3) > _score;
            Console.WriteLine(over ? "Kept output file" : "Overridden output file");
            return over;
        }

        private static double CountScore(List<Slide> slides)
        {

            Photo a, b, c, d;
            double mainScore = 0;
            for (var i = 0; i < slides.Count - 1; i++)
            {
                a = _photos[slides[i].Id1];
                if (slides[i].Id2 != null)
                {
                    b = _photos[slides[i].Id2 ?? -1];
                    slides[i].Tags = a.Tags.Concat(b.Tags).Distinct().ToList();
                }
                else
                {
                    slides[i].Tags = a.Tags;
                }


                c = _photos[slides[i + 1].Id1];
                if (slides[i + 1].Id2 != null)
                {
                    d = _photos[slides[i + 1].Id2 ?? -1];
                    slides[i + 1].Tags = c.Tags.Concat(d.Tags).Distinct().ToList();
                }
                else
                {
                    slides[i + 1].Tags = c.Tags;
                }

                mainScore += Score(slides[i], slides[i + 1]);
            }
            //Console.WriteLine(mainScore);
            return mainScore;
        }

        private static double Score(Program.Slide a, Program.Slide b)
        {
            var count1 = a.Tags.Count(x => b.Tags.Contains(x));
            var count2 = a.Tags.Count - count1;
            var count3 = b.Tags.Count - count1;
            return Math.Min(Math.Min(count1, count2), count3);
        }

        private static List<Photo> ReadSlidesAndData(string pathSlides, List<Photo> unsorted)
        {
            var slides3 = new List<Slide>();
            var photos = new List<Photo>();
            using (var readerSlides = new StreamReader(File.Open(pathSlides, FileMode.Open), Encoding.ASCII))
            {
                var line = readerSlides.ReadLine();
                while ((line = readerSlides.ReadLine()) != null)
                {
                    var parts = line.Split(' ').Select(x => Convert.ToInt32(x)).ToArray();
                    var slide = new Slide
                    {
                        Id1 = parts[0]
                    };

                    if (parts.Length == 2)
                    {
                        slide.Id2 = parts[1];
                    }
                    slides3.Add(slide);
                }
            }

            foreach (var slide in slides3)
            {
                photos.Add(unsorted[slide.Id1]);
                if (slide.Id2 != null)
                {
                    photos.Add(unsorted[slide.Id2 ?? -1]);
                }
            }

            return photos;
        }
    }
}
