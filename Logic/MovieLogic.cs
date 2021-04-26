using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Model;
using Repository;
using Repository.Models;

namespace Logic
{
    public class MovieLogic : IMovieLogic
    {
        private readonly RepoLogic _repo;

        public MovieLogic(RepoLogic repo)
        {
            _repo = repo;
        }

        public async Task<MovieDTO> GetMovie(string movieId)
        {
            if(_repo.MovieExists(movieId))
            {
                Movie movie = _repo.GetMovie(movieId);
                return Mapper.MovieToMovieDTO(movie);
            }

            ApiHelper.MovieObject movieObject = await ApiHelper.MovieProcessor.LoadMovieAsync(movieId);
            if(movieObject == null || movieObject.imdbID != movieId)
            {
                return null;
            }
            return Mapper.MovieObjectToMovieDTO(movieObject);
        }

        public List<string> SearchMovies(Dictionary<string, string> filters)
        {
            List<Movie> movies = _repo.GetAllMovies();
            foreach (var filter in filters)
            {
                Console.WriteLine(filter.Key + ": " + filter.Value);
                switch (filter.Key)
                {
                    case "tag":
                        FilterMoviesByTag(movies, filter.Value);
                    break;
                    case "rating":
                        FilterMoviesByRating(movies, filter.Value);
                    break;
                    case "actor":
                        FilterMoviesByActor(movies, filter.Value);
                    break;
                    case "director":
                        FilterMoviesByDirector(movies, filter.Value);
                    break;
                    case "genre":
                        FilterMoviesByGenre(movies, filter.Value);
                    break;
                    case "language":
                        FilterMoviesByLanguage(movies, filter.Value);
                    break;
                }
                if(movies.Count == 0)
                {
                    return new List<string>();
                }
            }

            List<string> movieIds = new List<string>();
            foreach (var movie in movies)
            {
                movieIds.Add(movie.ImdbId);
            }
            return movieIds;
        }

        private void FilterMoviesByTag(List<Movie> movies, string tagName)
        {
            for (int i = 0; i < movies.Count; i++)
            {
                if(movies[i].MovieTags.FirstOrDefault(mt => mt.TagName == tagName
                    && mt.VoteSum > 0) == null)
                {
                    movies.RemoveAt(i);
                    i--;
                }
            }
        }

        private void FilterMoviesByRating(List<Movie> movies, string ratingName)
        {

            for (int i = 0; i < movies.Count; i++)
            {
                if(movies[i].Rating.RatingName != ratingName)
                {
                    movies.RemoveAt(i);
                    i--;
                }
            }
        }

        private void FilterMoviesByActor(List<Movie> movies, string actorName)
        {
            for (int i = 0; i < movies.Count; i++)
            {
                if(movies[i].MovieActors.FirstOrDefault(ma => ma.Actor.ActorName == actorName) == null)
                {
                    movies.RemoveAt(i);
                    i--;
                }
            }
        }

        private void FilterMoviesByDirector(List<Movie> movies, string directorName)
        {
            for (int i = 0; i < movies.Count; i++)
            {
                if(movies[i].MovieDirectors.FirstOrDefault(ma => ma.Director.DirectorName == directorName) == null)
                {
                    movies.RemoveAt(i);
                    i--;
                }
            }
        }

        private void FilterMoviesByGenre(List<Movie> movies, string genreName)
        {
            for (int i = 0; i < movies.Count; i++)
            {
                if(movies[i].MovieGenres.FirstOrDefault(ma => ma.Genre.GenreName == genreName) == null)
                {
                    movies.RemoveAt(i);
                    i--;
                }
            }
        }

        private void FilterMoviesByLanguage(List<Movie> movies, string languageName)
        {
            for (int i = 0; i < movies.Count; i++)
            {
                if(movies[i].MovieLanguages.FirstOrDefault(ma => ma.Language.LanguageName == languageName) == null)
                {
                    movies.RemoveAt(i);
                    i--;
                }
            }
        }

        /// <summary>
        /// Adds a User's Tag Vote for a Movie to the database.
        /// Creates the Movie if it does not exist, based on the
        /// information from the public movie API.
        /// Adds the Tag to the database if it does not exist.
        /// Adds the MovieTag to the database if it does not
        /// exist.
        /// Returns true if successful; false otherwise.
        /// </summary>
        /// <param name="taggingDTO"></param>
        /// <returns></returns>
        public async Task<bool> TagMovie(TaggingDTO taggingDTO)
        {
            if(!_repo.MovieExists(taggingDTO.MovieId))
            {
                ApiHelper.MovieObject movieObject = await ApiHelper.MovieProcessor
                    .LoadMovieAsync(taggingDTO.MovieId);
                if(movieObject == null)
                {
                    return false;
                }
                MovieDTO movieDTO = Mapper.MovieObjectToMovieDTO(movieObject);
                if(!CreateMovie(movieDTO))
                {
                    return false;
                }
            }

// Call the User microservice to make sure the user exists

            MovieTagUser movieTagUser = new MovieTagUser();
            movieTagUser.ImdbId = taggingDTO.MovieId;
            movieTagUser.TagName = taggingDTO.TagName;
            movieTagUser.UserId = taggingDTO.UserId;
            movieTagUser.IsUpvote = taggingDTO.IsUpvote;
            if(_repo.MovieTagUserExists(movieTagUser))
            {
                return _repo.UpdateMovieTagUser(movieTagUser);
            }
            else
            {
                return _repo.AddMovieTagUser(movieTagUser);
            }
        }

        public bool BanTag(string tagName)
        {
            if(!_repo.TagExists(tagName))
            {
                return false;
            }
            Tag tag = new Tag();
            tag.TagName = tagName;
            tag.IsBanned = true;
            return _repo.UpdateTag(tag);
        }

        public bool UpdateMovie(MovieDTO movieDTO)
        {
            if(!_repo.MovieExists(movieDTO.ImdbId))
            {
                return CreateMovie(movieDTO);
            }
            
            Movie movie = _repo.GetMovie(movieDTO.ImdbId);

            UpdateMoviesOptionalProperties(movie, movieDTO);
            _repo.UpdateMovie(movie);

            if(movieDTO.MovieActors != null)
            {
                _repo.ClearMovieActors(movieDTO.ImdbId);
                foreach (var movieActorName in movieDTO.MovieActors)
                {
                    if(!_repo.AddMovieActor(movieDTO.ImdbId, movieActorName))
                    {
                        return false;
                    }
                }
            }

            if(movieDTO.MovieDirectors != null)
            {
                _repo.ClearMovieDirectors(movieDTO.ImdbId);
                foreach (var movieDirectorName in movieDTO.MovieDirectors)
                {
                    if(!_repo.AddMovieDirector(movieDTO.ImdbId, movieDirectorName))
                    {
                        return false;
                    }
                }
            }
            if(movieDTO.MovieGenres != null)
            {
                _repo.ClearMovieGenres(movieDTO.ImdbId);
                foreach (var movieGenreName in movieDTO.MovieGenres)
                {
                    if(!_repo.AddMovieGenre(movieDTO.ImdbId, movieGenreName))
                    {
                        return false;
                    }
                }
            }
            if(movieDTO.MovieLanguages != null)
            {
                _repo.ClearMovieLanguages(movieDTO.ImdbId);
                foreach (var movieLanguageName in movieDTO.MovieLanguages)
                {
                    if(!_repo.AddMovieLanguage(movieDTO.ImdbId, movieLanguageName))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Creates a new Movie entry from the information within
        /// the MovieDTO argument.
        /// Returns true if successful.
        /// </summary>
        /// <param name="movieDTO"></param>
        /// <returns></returns>
        public bool CreateMovie(MovieDTO movieDTO)
        {
            Movie movie = new Movie();
            movie.ImdbId = movieDTO.ImdbId;
            
            UpdateMoviesOptionalProperties(movie, movieDTO);

            if(!_repo.AddMovie(movie))
            {
                return false;
            }
            
            if(movieDTO.MovieActors != null)
            {
                foreach (var movieActorName in movieDTO.MovieActors)
                {
                    if(!_repo.AddMovieActor(movieDTO.ImdbId, movieActorName))
                    {
                        return false;
                    }
                }
            }
            if(movieDTO.MovieDirectors != null)
            {
                foreach (var movieDirectorName in movieDTO.MovieDirectors)
                {
                    if(!_repo.AddMovieDirector(movieDTO.ImdbId, movieDirectorName))
                    {
                        return false;
                    }
                }
            }
            if(movieDTO.MovieGenres != null)
            {
                foreach (var movieGenreName in movieDTO.MovieGenres)
                {
                    if(!_repo.AddMovieGenre(movieDTO.ImdbId, movieGenreName))
                    {
                        return false;
                    }
                }
            }
            if(movieDTO.MovieLanguages != null)
            {
                foreach (var movieLanguageName in movieDTO.MovieLanguages)
                {
                    if(!_repo.AddMovieLanguage(movieDTO.ImdbId, movieLanguageName))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public void UpdateMoviesOptionalProperties(Movie movie, MovieDTO movieDTO)
        {
            if(!String.IsNullOrEmpty(movieDTO.Title))
            {
                movie.Title = movieDTO.Title;
            }

            if(!String.IsNullOrEmpty(movieDTO.RatingName))
            {
                if(!_repo.RatingExists(movieDTO.RatingName))
                {
                    Rating newRating = new Rating();
                    newRating.RatingName = movieDTO.RatingName;
                    _repo.AddRating(newRating);
                }
                movie.RatingId = _repo.GetRating(movieDTO.RatingName).RatingId;
            }

            if(!String.IsNullOrEmpty(movieDTO.ReleaseDate))
            {
                movie.ReleaseDate = DateTime.ParseExact(movieDTO.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                movie.IsReleased = true;
            }
            if(!String.IsNullOrEmpty(movieDTO.ReleaseCountry))
            {
                movie.ReleaseCountry = movieDTO.ReleaseCountry;
            }
            if(movieDTO.RuntimeMinutes != null)
            {
                movie.RuntimeMinutes = movieDTO.RuntimeMinutes;
            }
            if(!String.IsNullOrEmpty(movieDTO.Plot))
            {
                movie.Plot = movieDTO.Plot;
            }
            if(!String.IsNullOrEmpty(movieDTO.PosterUrl))
            {
                movie.PosterUrl = movieDTO.PosterUrl;
            }
        }

        public async Task<bool> AppendMovie(MovieDTO movieDTO)
        {
            if(!_repo.MovieExists(movieDTO.ImdbId))
            {
                ApiHelper.MovieObject movieObject = await ApiHelper.MovieProcessor
                    .LoadMovieAsync(movieDTO.ImdbId);
                if(movieObject == null)
                {
                    return false;
                }
                MovieDTO newMovieDTO = Mapper.MovieObjectToMovieDTO(movieObject);
                if(!CreateMovie(newMovieDTO))
                {
                    return false;
                }
            }

            Movie movie = _repo.GetMovie(movieDTO.ImdbId);

            if(movie == null)
            {
                return false;
            }

            UpdateMoviesOptionalProperties(movie, movieDTO);

            _repo.UpdateMovie(movie);
            
            if(movieDTO.MovieActors != null)
            {
                foreach (var movieActorName in movieDTO.MovieActors)
                {
                    if(!_repo.MovieActorExists(movieDTO.ImdbId, movieActorName))
                    {
                        if(!_repo.AddMovieActor(movieDTO.ImdbId, movieActorName))
                        {
                            return false;
                        }
                    }
                }
            }
            if(movieDTO.MovieDirectors != null)
            {
                foreach (var movieDirectorName in movieDTO.MovieDirectors)
                {
                    if(!_repo.MovieDirectorExists(movieDTO.ImdbId, movieDirectorName))
                    {
                        if(!_repo.AddMovieDirector(movieDTO.ImdbId, movieDirectorName))
                        {
                            return false;
                        }
                    }
                }
            }
            if(movieDTO.MovieGenres != null)
            {
                foreach (var movieGenreName in movieDTO.MovieGenres)
                {
                    if(!_repo.MovieGenreExists(movieDTO.ImdbId, movieGenreName))
                    {
                        if(!_repo.AddMovieGenre(movieDTO.ImdbId, movieGenreName))
                        {
                            return false;
                        }
                    }
                }
            }
            if(movieDTO.MovieLanguages != null)
            {
                foreach (var movieLanguageName in movieDTO.MovieLanguages)
                {
                    if(!_repo.MovieLanguageExists(movieDTO.ImdbId, movieLanguageName))
                    {
                        if(!_repo.AddMovieLanguage(movieDTO.ImdbId, movieLanguageName))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Deletes the Movie from the database. Also deletes all associated
        /// entries in: Movie_Actor, Movie_Director, Movie_Genre, Movie_Language,
        /// Movie_Tag, Movie_Tag_User
        /// Returns true if successful.
        /// </summary>
        /// <param name="movieId"></param>
        /// <returns></returns>
        public bool DeleteMovie(string movieId)
        {
            if(!_repo.MovieExists(movieId))
            {
                return false;
            }
            return _repo.DeleteMovie(movieId);
        }
    }
}
