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
        /// <summary>
        /// Returns a new MovieDTO object containing the information provided in
        /// the MovieObject
        /// </summary>
        /// <param name="movieObject"></param>
        /// <returns></returns>
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

        /// <summary>
        /// Returns a new MovieDTO object containing the information provided in
        /// the Movie object, Rating object, and name lists.
        /// </summary>
        /// <param name="movie"></param>
        /// <param name="rating"></param>
        /// <param name="actorNames"></param>
        /// <param name="directorNames"></param>
        /// <param name="genreNames"></param>
        /// <param name="languageNames"></param>
        /// <param name="tagNames"></param>
        /// <returns></returns>
        public static MovieDTO MovieToMovieDTO(Movie movie, Rating rating, List<string> actorNames
            , List<string> directorNames, List<string> genreNames, List<string> languageNames
            , List<string> tagNames)
        {
            MovieDTO movieDTO = new MovieDTO()
            {
                ImdbId = movie.ImdbId,
                Title = movie.Title,
                ReleaseCountry = movie.ReleaseCountry,
                RuntimeMinutes = movie.RuntimeMinutes,
                IsReleased = movie.IsReleased,
                Plot = movie.Plot,
                PosterURL = movie.PosterUrl
            };

            if(rating != null)
            {
                movieDTO.RatingName = rating.RatingName;
            }

            if(movie.ReleaseDate != null)
            {
                DateTime releaseDate = movie.ReleaseDate ?? DateTime.Now;
                movieDTO.ReleaseDate = releaseDate.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            }

            movieDTO.MovieActors = new List<string>();
            foreach (var actorName in actorNames)
            {
                movieDTO.MovieActors.Add(actorName);
            }
            
            movieDTO.MovieDirectors = new List<string>();
            foreach (var directorName in directorNames)
            {
                movieDTO.MovieDirectors.Add(directorName);
            }
            
            movieDTO.MovieGenres = new List<string>();
            foreach (var genreName in genreNames)
            {
                movieDTO.MovieGenres.Add(genreName);
            }
            
            movieDTO.MovieLanguages = new List<string>();
            foreach (var languageName in languageNames)
            {
                movieDTO.MovieLanguages.Add(languageName);
            }
            
            movieDTO.MovieTags = new List<string>();
            foreach (var tagName in tagNames)
            {
                movieDTO.MovieTags.Add(tagName);
            }

            return movieDTO;
        }

        /// <summary>
        /// Returns a new Movie object containing the information provided in
        /// the MovieDTO object
        /// </summary>
        /// <param name="movieDTO"></param>
        /// <returns></returns>
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
