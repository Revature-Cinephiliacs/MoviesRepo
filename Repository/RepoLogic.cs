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

        public bool AddMovieTag(MovieTag movieTag)
        {
            _dbContext.MovieTags.Add(movieTag);
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

        public bool UpdateMovieTag(MovieTag movieTag)
        {
            _dbContext.MovieTags.Update(movieTag);
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
        public bool MovieTagExists(MovieTag movieTag)
        {
            return (_dbContext.MovieTags.FirstOrDefault(mt => mt.ImdbId == movieTag.ImdbId
                && mt.TagName == movieTag.TagName && mt.ImdbId == movieTag.ImdbId) != null);
        }
    }
}
