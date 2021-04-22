using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Repository.Models;

namespace Logic
{
    public static class Mapper
    {

        public static MovieDTO MovieToMovieDto(Movie movie)
        {
            var movieDto = new MovieDTO()
            {
             ImdbId = movie.ImdbId ,
            Title = movie.Title ,
            ReleaseDate = movie.ReleaseDate, 
            ReleaseCountry = movie.ReleaseCountry,
             RuntimeMinutes = movie.RuntimeMinutes,
             IsReleased = movie.IsReleased,
             Plot = movie.Plot,
            };

            return movieDto;

        }
        public static Movie MovieDtoToMovie(MovieDTO movieDto)
        {
            var movie = new Movie()
            {
                ImdbId = movieDto.ImdbId ,
                Title = movieDto.Title ,
                ReleaseDate = movieDto.ReleaseDate, 
                ReleaseCountry = movieDto.ReleaseCountry,
                RuntimeMinutes = movieDto.RuntimeMinutes,
                IsReleased = movieDto.IsReleased,
                Plot = movieDto.Plot,
            };

            return movie;

        }

    }
}
