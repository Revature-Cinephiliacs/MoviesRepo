using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Model;
using Repository.Models;

namespace Logic
{
    public static class Mapper
    {
        public static MovieDTO MovieObjectToMovieDTO(ApiHelper.MovieObject movieObject)
        {
            var movieDTO = new MovieDTO()
            {
                ImdbId = movieObject.imdbID,
                Title = movieObject.Title,
                ReleaseCountry = movieObject.Country,
                Plot = movieObject.Plot,
                PosterURL = movieObject.Poster,
                RatingName = movieObject.Rated
            };

            try {
                movieDTO.ReleaseDate = DateTime.Parse(movieObject.Released).ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
                movieDTO.IsReleased = true;
            }
            catch {
                movieDTO.ReleaseDate = null;
                movieDTO.IsReleased = false;
            }

            try {
                string runtimeMinutes = movieObject.RunTime.Split(' ')[0];
                movieDTO.RuntimeMinutes = short.Parse(runtimeMinutes);
            }
            catch {
                movieDTO.RuntimeMinutes = null;
            }
            
            movieDTO.MovieActors = movieObject.Actors.Split(',').ToList();
            for (int i = 0; i < movieDTO.MovieActors.Count; i++)
            {
                movieDTO.MovieActors[i] = movieDTO.MovieActors[i].Trim();
            }
            
            movieDTO.MovieDirectors = movieObject.Director.Split(',').ToList();
            for (int i = 0; i < movieDTO.MovieDirectors.Count; i++)
            {
                movieDTO.MovieDirectors[i] = movieDTO.MovieDirectors[i].Trim();
            }
            
            movieDTO.MovieGenres = movieObject.Genre.Split(',').ToList();
            for (int i = 0; i < movieDTO.MovieGenres.Count; i++)
            {
                movieDTO.MovieGenres[i] = movieDTO.MovieGenres[i].Trim();
            }
            
            movieDTO.MovieLanguages = movieObject.Language.Split(',').ToList();
            for (int i = 0; i < movieDTO.MovieLanguages.Count; i++)
            {
                movieDTO.MovieLanguages[i] = movieDTO.MovieLanguages[i].Trim();
            }
            
            movieDTO.MovieTags = new List<string>();

            return movieDTO;
        }

        public static MovieDTO MovieToMovieDTO(Movie movie)
        {
            var movieDTO = new MovieDTO()
            {
                ImdbId = movie.ImdbId,
                Title = movie.Title,
                ReleaseCountry = movie.ReleaseCountry,
                RuntimeMinutes = movie.RuntimeMinutes,
                IsReleased = movie.IsReleased,
                Plot = movie.Plot,
                PosterURL = movie.PosterUrl,
                RatingName = movie.Rating.RatingName
            };

            if(movie.ReleaseDate == null)
            {
                movieDTO.ReleaseDate = null;
            }
            else
            {
                DateTime releaseDate = movie.ReleaseDate ?? DateTime.Now;
                movieDTO.ReleaseDate = releaseDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }

            movieDTO.MovieActors = new List<string>();
            foreach (var movieActor in movie.MovieActors)
            {
                movieDTO.MovieActors.Add(movieActor.Actor.ActorName);
            }
            
            movieDTO.MovieDirectors = new List<string>();
            foreach (var movieDirector in movie.MovieDirectors)
            {
                movieDTO.MovieDirectors.Add(movieDirector.Director.DirectorName);
            }
            
            movieDTO.MovieGenres = new List<string>();
            foreach (var movieGenre in movie.MovieGenres)
            {
                movieDTO.MovieGenres.Add(movieGenre.Genre.GenreName);
            }
            
            movieDTO.MovieLanguages = new List<string>();
            foreach (var movieLanguage in movie.MovieLanguages)
            {
                movieDTO.MovieLanguages.Add(movieLanguage.Language.LanguageName);
            }
            
            movieDTO.MovieTags = new List<string>();
            foreach (var movieTag in movie.MovieTags)
            {
                movieDTO.MovieTags.Add(movieTag.TagName);
            }

            return movieDTO;
        }

        public static Movie MovieDTOToMovie(MovieDTO movieDTO)
        {
            var movie = new Movie()
            {
                ImdbId = movieDTO.ImdbId,
                Title = movieDTO.Title,
                ReleaseCountry = movieDTO.ReleaseCountry,
                RuntimeMinutes = movieDTO.RuntimeMinutes,
                IsReleased = movieDTO.IsReleased,
                Plot = movieDTO.Plot,
                PosterUrl = movieDTO.PosterURL
            };

            if(String.IsNullOrEmpty(movieDTO.ReleaseDate))
            {
                movie.ReleaseDate = null;
            }
            else
            {
                movie.ReleaseDate = DateTime.ParseExact(movieDTO.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            }

            return movie;
        }

    }
}
