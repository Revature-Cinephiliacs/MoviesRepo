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
                var movieTags = _repo.GetMovieTags(movieId);
                var tagNames = new List<string>();
                foreach (var movieTag in movieTags)
                {
                    var tag = _repo.GetTag(movieTag.TagName);
                    if(tag != null && tag.IsBanned == false)
                    {
                        tagNames.Add(tag.TagName);
                    }
                }

                return Mapper.MovieToMovieDTO(movie, _repo.GetRating(movie.RatingId ?? 0), _repo.GetMovieActorNames(movieId)
                    , _repo.GetMovieDirectorNames(movieId), _repo.GetMovieGenreNames(movieId)
                    , _repo.GetMovieLanguageNames(movieId), tagNames);
            }

            ApiHelper.MovieObject movieObject = await ApiHelper.MovieProcessor.LoadMovieAsync(movieId);
            if(movieObject == null || movieObject.imdbID != movieId)
            {
                return null;
            }
            return Mapper.MovieObjectToMovieDTO(movieObject);
        }

        public List<string> SearchMovies(Dictionary<string, string[]> filters)
        {
            List<Movie> movies = _repo.GetAllMovies();
            foreach (var filter in filters)
            {
                switch (filter.Key.ToLower())
                {
                    case "tags":
                    case "tag":
                        FilterMoviesByTags(movies, filter.Value);
                    break;
                    case "rating":
                        FilterMoviesByRatings(movies, filter.Value);
                    break;
                    case "actors":
                    case "actor":
                        FilterMoviesByActors(movies, filter.Value);
                    break;
                    case "directors":
                    case "director":
                        FilterMoviesByDirectors(movies, filter.Value);
                    break;
                    case "genres":
                    case "genre":
                        FilterMoviesByGenres(movies, filter.Value);
                    break;
                    case "languages":
                    case "language":
                        FilterMoviesByLanguages(movies, filter.Value);
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

        /// <summary>
        /// Removes any movies from the list argument that are not tagged
        /// with all of the provided tag names.
        /// </summary>
        /// <param name="movies"></param>
        /// <param name="tagName"></param>
        private void FilterMoviesByTags(List<Movie> movies, string[] tagNames)
        {
            foreach (var tagName in tagNames)
            {
                for (int i = 0; i < movies.Count; i++)
                {
                    List<MovieTag> movieTags = _repo.GetMovieTags(movies[i].ImdbId);
                    if(movieTags.FirstOrDefault(mt => mt.TagName == tagName
                        && mt.VoteSum > 0) == null)
                    {
                        movies.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        /// <summary>
        /// Removes all movies from the list argument that do not have the
        /// rating that is specified in the argument.
        /// </summary>
        /// <param name="movies"></param>
        /// <param name="ratingName"></param>
        private void FilterMoviesByRatings(List<Movie> movies, string[] ratingNames)
        {
            foreach (var ratingName in ratingNames)
            {
                for (int i = 0; i < movies.Count; i++)
                {
                    var ratingId = _repo.GetRating(ratingName).RatingId;
                    if(movies[i].RatingId != ratingId)
                    {
                        movies.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        /// <summary>
        /// Removes all movies from the list argument that do not have all of the
        /// actors that are specified in the argument.
        /// </summary>
        /// <param name="movies"></param>
        /// <param name="actorName"></param>
        private void FilterMoviesByActors(List<Movie> movies, string[] actorNames)
        {
            foreach (var actorName in actorNames)
            {
                for (int i = 0; i < movies.Count; i++)
                {
                    List<string> movieActorNames = _repo.GetMovieActorNames(movies[i].ImdbId);
                    if(!movieActorNames.Contains(actorName))
                    {
                        movies.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        /// <summary>
        /// Removes all movies from the list argument that do not have all of the
        /// directors that are specified in the argument.
        /// </summary>
        /// <param name="movies"></param>
        /// <param name="directorName"></param>
        private void FilterMoviesByDirectors(List<Movie> movies, string[] directorNames)
        {
            foreach (var directorName in directorNames)
            {
                for (int i = 0; i < movies.Count; i++)
                {
                    List<string> movieDirectorNames = _repo.GetMovieDirectorNames(movies[i].ImdbId);
                    if(!movieDirectorNames.Contains(directorName))
                    {
                        movies.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        /// <summary>
        /// Removes all movies from the list argument that do not have all of the
        /// genres that are specified in the argument.
        /// </summary>
        /// <param name="movies"></param>
        /// <param name="genreName"></param>
        private void FilterMoviesByGenres(List<Movie> movies, string[] genreNames)
        {
            foreach (var genreName in genreNames)
            {
                for (int i = 0; i < movies.Count; i++)
                {
                    List<string> movieGenreNames = _repo.GetMovieGenreNames(movies[i].ImdbId);
                    if(!movieGenreNames.Contains(genreName))
                    {
                        movies.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

        /// <summary>
        /// Removes all movies from the list argument that do not have all of the
        /// languages that are specified in the argument.
        /// </summary>
        /// <param name="movies"></param>
        /// <param name="languageName"></param>
        private void FilterMoviesByLanguages(List<Movie> movies, string[] languageNames)
        {
            foreach (var languageName in languageNames)
            {
                for (int i = 0; i < movies.Count; i++)
                {
                    List<string> movieLanguageNames = _repo.GetMovieLanguageNames(movies[i].ImdbId);
                    if(!movieLanguageNames.Contains(languageName))
                    {
                        movies.RemoveAt(i);
                        i--;
                    }
                }
            }
        }

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
                _repo.UpdateMovieTagUser(movieTagUser);
            }
            else
            {
                _repo.AddMovieTagUser(movieTagUser);
            }
            return true;
        }

        public bool SetTagBanStatus(string tagName, bool isBan)
        {
            if(!_repo.TagExists(tagName))
            {
                return false;
            }
            Tag tag = _repo.GetTag(tagName);
            tag.IsBanned = isBan;
            _repo.UpdateTag(tag);
            return true;
        }

        public bool UpdateMovie(string movieId, MovieDTO movieDTO)
        {
            if(movieDTO.ImdbId != null && movieDTO.ImdbId != movieId)
            {
                return false;
            }

            if(!_repo.MovieExists(movieId))
            {
                return CreateMovie(movieDTO);
            }
            
            Movie movie = _repo.GetMovie(movieId);

            movie.Title = movieDTO.Title;
            movie.ReleaseCountry = movieDTO.ReleaseCountry;
            movie.RuntimeMinutes = movieDTO.RuntimeMinutes;
            movie.Plot = movieDTO.Plot;
            movie.PosterUrl = movieDTO.PosterURL;

            if(String.IsNullOrEmpty(movieDTO.RatingName))
            {
                movie.RatingId = null;
            }
            else
            {
                if(!_repo.RatingExists(movieDTO.RatingName))
                {
                    Rating newRating = new Rating();
                    newRating.RatingName = movieDTO.RatingName;
                    _repo.AddRating(newRating);
                }
                movie.RatingId = _repo.GetRating(movieDTO.RatingName).RatingId;
            }

            if(String.IsNullOrEmpty(movieDTO.ReleaseDate))
            {
                movie.ReleaseDate = null;
                movie.IsReleased = null;
            }
            else
            {
                movie.ReleaseDate = DateTime.ParseExact(movieDTO.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                movie.IsReleased = true;
            }

            _repo.UpdateMovie(movie);

            _repo.ClearMovieActors(movieId);
            if(movieDTO.MovieActors != null)
            {
                foreach (var movieActorName in movieDTO.MovieActors)
                {
                    if(!_repo.AddMovieActor(movieId, movieActorName))
                    {
                        return false;
                    }
                }
            }

            _repo.ClearMovieDirectors(movieId);
            if(movieDTO.MovieDirectors != null)
            {
                foreach (var movieDirectorName in movieDTO.MovieDirectors)
                {
                    if(!_repo.AddMovieDirector(movieId, movieDirectorName))
                    {
                        return false;
                    }
                }
            }

            _repo.ClearMovieGenres(movieId);
            if(movieDTO.MovieGenres != null)
            {
                foreach (var movieGenreName in movieDTO.MovieGenres)
                {
                    if(!_repo.AddMovieGenre(movieId, movieGenreName))
                    {
                        return false;
                    }
                }
            }

            _repo.ClearMovieLanguages(movieId);
            if(movieDTO.MovieLanguages != null)
            {
                foreach (var movieLanguageName in movieDTO.MovieLanguages)
                {
                    if(!_repo.AddMovieLanguage(movieId, movieLanguageName))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public bool CreateMovie(MovieDTO movieDTO)
        {
            if(movieDTO.ImdbId == null || _repo.MovieExists(movieDTO.ImdbId))
            {
                return false;
            }

            Movie movie = new Movie();
            movie.ImdbId = movieDTO.ImdbId;
            
            AppendMoviesOptionalProperties(movie, movieDTO);

            _repo.AddMovie(movie);
            
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

        /// <summary>
        /// Updates all of the optional, non-List properties of the Movie argument
        /// to the properties supplied in the MovieDTO argument. Any null property
        /// of the MovieDTO remains unchanged in the Movie object.
        /// </summary>
        /// <param name="movie"></param>
        /// <param name="movieDTO"></param>
        public void AppendMoviesOptionalProperties(Movie movie, MovieDTO movieDTO)
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
            if(!String.IsNullOrEmpty(movieDTO.PosterURL))
            {
                movie.PosterUrl = movieDTO.PosterURL;
            }
        }

        public async Task<bool> AppendMovie(string movieId, MovieDTO movieDTO)
        {
            if(!_repo.MovieExists(movieId))
            {
                ApiHelper.MovieObject movieObject = await ApiHelper.MovieProcessor
                    .LoadMovieAsync(movieId);
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

            Movie movie = _repo.GetMovie(movieId);

            if(movie == null)
            {
                return false;
            }

            AppendMoviesOptionalProperties(movie, movieDTO);

            _repo.UpdateMovie(movie);
            
            if(movieDTO.MovieActors != null)
            {
                foreach (var movieActorName in movieDTO.MovieActors)
                {
                    if(!_repo.MovieActorExists(movieId, movieActorName))
                    {
                        if(!_repo.AddMovieActor(movieId, movieActorName))
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
                    if(!_repo.MovieDirectorExists(movieId, movieDirectorName))
                    {
                        if(!_repo.AddMovieDirector(movieId, movieDirectorName))
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
                    if(!_repo.MovieGenreExists(movieId, movieGenreName))
                    {
                        if(!_repo.AddMovieGenre(movieId, movieGenreName))
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
                    if(!_repo.MovieLanguageExists(movieId, movieLanguageName))
                    {
                        if(!_repo.AddMovieLanguage(movieId, movieLanguageName))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public bool DeleteMovie(string movieId)
        {
            if(!_repo.MovieExists(movieId))
            {
                return false;
            }
            _repo.DeleteMovie(movieId);
            return true;
        }

        public bool FollowMovie(string movieId, string userId)
        {
            if(!_repo.MovieExists(movieId))
            {
                return false;
            }

// Call the User microservice to make sure the user exists

            _repo.AddFollowingMovie(movieId, userId);
            return true;
        }

        public bool UnfollowMovie(string movieId, string userId)
        {
            if(!_repo.MovieExists(movieId))
            {
                return false;
            }

// Call the User microservice to make sure the user exists

            _repo.DeleteFollowingMovie(movieId, userId);
            return true;
        }

        public List<string> GetFollowingMovies(string userId)
        {
            return _repo.GetFollowingMovies(userId);
        }
    }
}
