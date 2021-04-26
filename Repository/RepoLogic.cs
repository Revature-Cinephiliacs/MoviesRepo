using System;
using System.Collections.Generic;
using System.Linq;
using Repository.Models;

namespace Repository
{
    public class RepoLogic
    {
        private readonly Cinephiliacs_MovieContext _dbContext;

        public RepoLogic(Cinephiliacs_MovieContext dbContext)
        {
            _dbContext = dbContext;
        }

        public bool AddMovie(Movie movie)
        {
            _dbContext.Movies.Add(movie);
            if(_dbContext.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }

        public bool AddTag(Tag tag)
        {
            _dbContext.Tags.Add(tag);
            if(_dbContext.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }

        public bool AddRating(Rating rating)
        {
            _dbContext.Ratings.Add(rating);
            if(_dbContext.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }

        public bool AddActor(Actor actor)
        {
            _dbContext.Actors.Add(actor);
            if(_dbContext.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }

        public bool AddDirector(Director director)
        {
            _dbContext.Directors.Add(director);
            if(_dbContext.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }

        public bool AddGenre(Genre genre)
        {
            _dbContext.Genres.Add(genre);
            if(_dbContext.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }

        public bool AddLanguage(Language language)
        {
            _dbContext.Languages.Add(language);
            if(_dbContext.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }

        public bool AddMovieTagUser(MovieTagUser movieTagUser)
        {
            var movieTag = GetMovieTag(movieTagUser.ImdbId, movieTagUser.TagName);
            if(movieTag == null)
            {
                movieTag = new MovieTag();
                movieTag.ImdbId = movieTagUser.ImdbId;
                movieTag.TagName = movieTagUser.TagName;
                movieTag.VoteSum = 0;
                _dbContext.MovieTags.Add(movieTag);
            }
            bool isUpvote = movieTagUser.IsUpvote ?? true;
            if(isUpvote)
            {
                if(movieTag.VoteSum < int.MaxValue)
                    movieTag.VoteSum += 1;
            }
            else
            {
                if(movieTag.VoteSum > int.MinValue)
                    movieTag.VoteSum -= 1;
            }
            _dbContext.MovieTagUsers.Add(movieTagUser);
            if(_dbContext.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }

        public bool UpdateTag(Tag tag)
        {
            _dbContext.Tags.Update(tag);
            if(_dbContext.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }

        public bool UpdateMovieTagUser(MovieTagUser movieTagUser)
        {
            var movieTag = GetMovieTag(movieTagUser.ImdbId, movieTagUser.TagName);
            if(movieTag == null)
            {
                movieTag = new MovieTag();
                movieTag.ImdbId = movieTagUser.ImdbId;
                movieTag.TagName = movieTagUser.TagName;
                movieTag.VoteSum = 0;
                _dbContext.MovieTags.Add(movieTag);
            }
            bool isUpvote = movieTagUser.IsUpvote ?? true;
            if(isUpvote)
            {
                if(movieTag.VoteSum < int.MaxValue)
                    movieTag.VoteSum += 1;
            }
            else
            {
                if(movieTag.VoteSum > int.MinValue)
                    movieTag.VoteSum -= 1;
            }
            _dbContext.MovieTagUsers.Update(movieTagUser);
            if(_dbContext.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }

        public List<Movie> GetAllMovies()
        {
            return _dbContext.Movies.ToList<Movie>();
        }

        public MovieTag GetMovieTag(string movieId, string tagName)
        {
            return _dbContext.MovieTags.FirstOrDefault(mt => mt.ImdbId == movieId && mt.TagName == tagName);
        }

        public Movie GetMovie(string movieId)
        {
            return _dbContext.Movies.FirstOrDefault(m => m.ImdbId == movieId);
        }

        public bool MovieExists(string movieId)
        {
            return (_dbContext.Movies.FirstOrDefault(m => m.ImdbId == movieId) != null);
        }
        public bool TagExists(string tagName)
        {
            return (_dbContext.Tags.FirstOrDefault(t => t.TagName == tagName) != null);
        }
        public bool RatingExists(string ratingName)
        {
            return (_dbContext.Ratings.FirstOrDefault(r => r.RatingName == ratingName) != null);
        }
        public bool ActorExists(string actor)
        {
            return (_dbContext.Actors.FirstOrDefault(a => a.ActorName == actor) != null);
        }
        public bool LanguageExists(string language)
        {
            return (_dbContext.Languages.FirstOrDefault(l => l.LanguageName == language) != null);
        }
        public bool DirectorExists(string director)
        {
            return (_dbContext.Directors.FirstOrDefault(d => d.DirectorName == director) != null);
        }
        public bool GenreExists(string genre)
        {
            return (_dbContext.Genres.FirstOrDefault(g => g.GenreName == genre) != null);
        }
        public bool MovieActorExists(MovieActor movieActor)
        {
            return (_dbContext.MovieActors.FirstOrDefault(ma => ma.ImdbId == movieActor.ImdbId
                && ma.ActorId == movieActor.ActorId) != null);
        }
        public bool MovieDirectorExists(MovieDirector movieDirector)
        {
            return (_dbContext.MovieDirectors.FirstOrDefault(md => md.ImdbId == movieDirector.ImdbId
                && md.DirectorId == movieDirector.DirectorId) != null);
        }
        public bool MovieGenreExists(MovieGenre movieGenre)
        {
            return (_dbContext.MovieGenres.FirstOrDefault(mg => mg.ImdbId == movieGenre.ImdbId
                && mg.GenreId == movieGenre.GenreId) != null);
        }
        public bool MovieLanguageExists(MovieLanguage movieLanguage)
        {
            return (_dbContext.MovieLanguages.FirstOrDefault(ml => ml.ImdbId == movieLanguage.ImdbId
                && ml.LanguageId == movieLanguage.LanguageId) != null);
        }
        public bool MovieTagUserExists(MovieTagUser movieTagUser)
        {
            return (_dbContext.MovieTagUsers.FirstOrDefault(mtu => mtu.ImdbId == movieTagUser.ImdbId
                && mtu.TagName == movieTagUser.TagName && mtu.UserId == movieTagUser.UserId) != null);
        }
    }
}
