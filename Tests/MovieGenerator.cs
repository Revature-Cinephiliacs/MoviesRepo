using System;
using Model;

namespace Tests
{
    internal static class MovieGenerator
    {
        
        internal static MovieDTO GetRandomMovie()
        {
            MovieDTO movieDTO = new MovieDTO();
            

            // public DateTime? ReleaseDate { get; set; }
            // public short? RuntimeMinutes { get; set; }
            // public bool? IsReleased { get; set; }

            movieDTO.ImdbId = Guid.NewGuid().ToString();
            movieDTO.Title = Guid.NewGuid().ToString();
            movieDTO.ReleaseCountry = Guid.NewGuid().ToString();
            movieDTO.Plot = Guid.NewGuid().ToString();

            movieDTO. = GetRandomDateTime();
            movieDTO.short = Guid.NewGuid().ToString();
            movieDTO.bool = Guid.NewGuid().ToString();
        }

        private static DateTime GetRandomDateTime()
        {
            Random randomGen = new Random();
            DateTime dateTime = DateTime.Now;
            
            return dateTime.AddDays(-1*randomGen.Next(10000));
        }
        DateTime RandomDay()
        {
            DateTime start = new DateTime(1995, 1, 1);
            int range = (DateTime.Today - start).Days;           
            return start.AddDays(gen.Next(range));
        }
    }
}