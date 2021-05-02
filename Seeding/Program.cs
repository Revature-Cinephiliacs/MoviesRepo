using System;
using System.Threading.Tasks;

namespace Seeding
{
    class Program
    {
        static async Task Main(string[] args)
        {
            await Seeding.SeedDbFromCSV();
        }
    }
}
