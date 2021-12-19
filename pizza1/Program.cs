using System;
using System.CodeDom;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace pizza1
{
    class Program
    {
        static void Main(string[] args)
        {
            var pizza = ReadFile("b_small.in");
            WriteOut(pizza);
            Solve(pizza);


            Console.ReadKey();
        }

        private static void Solve(Pizza p)
        {
            var min = p.MinIngPerSlice;
            var max = p.MaxCellsPerSlice;
            for (var i = 0; i < p.Rows; i++)
            {
                for (var j = 0; j < p.Columns; j++)
                {
                    var nextR = j == p.Columns - 1 ? 0 : j + 1;
                    var nextC = nextR == 0 ? i + 1 : i;
                    for (var k = nextR; k < p.Rows; k++)
                    {
                        for (var l = nextC; l < p.Columns; l++)
                        {

                            // get first square. proceed from next available point
                            // mark available points somewhere
                            // make function recursive
                            // save correct squares somewhere
                        }
                    }
                }
            }
        }

        private static int GetAreaSize(int r1, int c1, int r2, int c2)
        {
            return (Math.Abs(r2 - r1) + 1) * (Math.Abs(c2 - c1) + 1);
        }

        private static int[,] GetArea(int[,] pad, int r1, int c1, int r2, int c2)
        {
            var r = Math.Abs(r2 - r1) + 1;
            var c = Math.Abs(c2 - c1) + 1;
            var pad0 = new int[r, c];
            var k = 0;
            var l = 0;
            for (var i = Math.Min(r1, r2); i < Math.Max(r1, r2); i++)
            {
                for (var j = Math.Min(c1, c2); j < Math.Max(c1, c2); j++)
                {
                    pad0[k, l] = pad[i, j];
                    l++;
                }

                k++;
            }

            return pad0;
        }

        // Is Less Than Max
        private static bool IsLess(int max, int area)
        {
            return area <= max;
        }

        // Is More Or Equals tomatoes and mushrooms 
        private static bool IsMore(int[,] area, int min)
        {
            var r = area.GetLength(0);
            var c = area.GetLength(1);
            var sum0 = 0;
            var sum1 = 0;

            for (var i = 0; i < r; i++)
            {
                for (var j = 0; j < c; j++)
                {
                    switch (area[i, j])
                    {
                        case 0:
                            sum0++;
                            break;
                        case 1:
                            sum1++;
                            break;
                        default:
                            break;
                    }
                    if (sum0 >= min && sum1 >= min)
                    {
                        return true;
                    }
                }
                // here also?
            }

            return false;
        }

        private static Pizza ReadFile(string path)
        {
            using (var r = new StreamReader($"data/{path}"))
            {
                // 0 - M, 1 - T
                var line = r.ReadLine();
                var numbers = line.Split(' ');
                var rows = int.Parse(numbers[0]);
                var cols = int.Parse(numbers[1]);
                var min = int.Parse(numbers[2]);
                var max = int.Parse(numbers[3]);
                var pad = new int[rows,cols];
                var rowIndex = 0;
                var colIndex = 0;
                while ((line = r.ReadLine()) != null)
                {
                  
                    foreach (var ch in line)
                    {
                        switch (ch)
                        {
                            case 'T':
                                pad[rowIndex, colIndex] = 0;
                                break;
                            case 'M':
                                pad[rowIndex, colIndex] = 1;
                                break;
                            default:
                                throw new InvalidOperationException("input error");
                        }

                        colIndex++;
                    }

                    colIndex = 0;
                    rowIndex++;
                }

                rowIndex = 0;
                return new Pizza()
                {
                    Rows = rows,
                    Columns = cols,
                    MinIngPerSlice = min,
                    MaxCellsPerSlice = max,
                    Pad = pad
                };
            }
        }

        private static void WriteOut(Pizza pizza)
        {
            Console.WriteLine("Rows-{0} Cols-{1} MinEach-{2} MaxAll-{3}", pizza.Rows, pizza.Columns, pizza.MinIngPerSlice, pizza.MaxCellsPerSlice);
            for (var j = 0; j < pizza.Rows; j++)
            {
                for (var i = 0; i < pizza.Columns; i++)
                {
                    Console.Write(pizza.Pad[j,i] == 0 ? 'T' : 'M');
                }

                Console.WriteLine();
            }
        }
    }
}
