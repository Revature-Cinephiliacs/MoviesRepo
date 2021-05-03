using System;
using System.Threading.Tasks;

namespace Seeding
{
    class Program
    {
        static async Task Main(string[] args)
        {
            Console.WriteLine("Enter the file line number to start at (Default 1): ");
            await Seeding.SeedDbFromCSV(Console.ReadLine());
        }
    }
}
