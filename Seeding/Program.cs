using System;
using System.Threading.Tasks;

namespace Seeding
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Input file path:");
            await Seeding.SeedDbFromCSV(Console.ReadLine());
        }
    }
}
