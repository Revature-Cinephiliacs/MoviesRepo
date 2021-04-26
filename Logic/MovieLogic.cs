using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

            ApiHelper.MovieObject movieObject = await ApiHelper.MovieProcessor.LoadMovie(movieId);
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
                    return null;
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

        public bool TagMovie(TaggingDTO taggingDTO)
        {
            if(!_repo.MovieExists(taggingDTO.MovieId))
            {
                return false;
            }

// Call the User microservice to make sure the user exists

            if(!_repo.TagExists(taggingDTO.TagName))
            {
                Tag tag = new Tag();
                tag.TagName = taggingDTO.TagName;
                tag.IsBanned = false;
                if(!_repo.AddTag(tag))
                {
                    return false;
                }
            }
            
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

        public Task<bool> UpdateMovie(MovieDTO movieDTO)
        {
            throw new NotImplementedException();
        }

        public Task<bool> AppendMovie(MovieDTO movieDTO)
        {
            throw new NotImplementedException();
        }
    }
}
