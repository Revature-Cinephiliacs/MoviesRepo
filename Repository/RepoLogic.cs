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

        /// <summary>
        /// Adds a Movie to the database. Returns true if
        /// successful; false otherwise.
        /// </summary>
        /// <param name="movie"></param>
        /// <returns></returns>
        public bool AddMovie(Movie movie)
        {
            _dbContext.Movies.Add(movie);
            if(_dbContext.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds a Tag to the database. Returns true if
        /// successful; false otherwise.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public bool AddTag(Tag tag)
        {
            _dbContext.Tags.Add(tag);
            if(_dbContext.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds a Rating to the database. Returns true if
        /// successful; false otherwise.
        /// </summary>
        /// <param name="rating"></param>
        /// <returns></returns>
        public bool AddRating(Rating rating)
        {
            _dbContext.Ratings.Add(rating);
            if(_dbContext.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds an Actor to the database. Returns true if
        /// successful; false otherwise.
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        public bool AddActor(Actor actor)
        {
            _dbContext.Actors.Add(actor);
            if(_dbContext.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds a Director to the database. Returns true if
        /// successful; false otherwise.
        /// </summary>
        /// <param name="director"></param>
        /// <returns></returns>
        public bool AddDirector(Director director)
        {
            _dbContext.Directors.Add(director);
            if(_dbContext.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds a Genre to the database. Returns true if
        /// successful; false otherwise.
        /// </summary>
        /// <param name="genre"></param>
        /// <returns></returns>
        public bool AddGenre(Genre genre)
        {
            _dbContext.Genres.Add(genre);
            if(_dbContext.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds a Language to the database. Returns true if
        /// successful; false otherwise.
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        public bool AddLanguage(Language language)
        {
            _dbContext.Languages.Add(language);
            if(_dbContext.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Adds a User's Tag Vote for a Movie to the database
        /// and increments the MovieTag's VoteSum.
        /// Adds the Tag to the database if it does not exist.
        /// Adds the MovieTag to the database if it does not
        /// exist.
        /// Returns true if successful; false otherwise.
        /// </summary>
        /// <param name="movieTagUser"></param>
        /// <returns></returns>
        public bool AddMovieTagUser(MovieTagUser movieTagUser)
        {
            if(!TagExists(movieTagUser.TagName))
            {
                Tag tag = new Tag();
                tag.TagName = movieTagUser.TagName;
                tag.IsBanned = false;
                _dbContext.Tags.Add(tag);
            }
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

        /// <summary>
        /// Assigns an Actor to a Movie in the database.
        /// Adds the Actor to the database if it does not exist.
        /// Returns true if successful; false otherwise.
        /// </summary>
        /// <param name="movieActorName"></param>
        /// <returns></returns>
        public bool AddMovieActor(string movieId, string movieActorName)
        {
            if(!MovieExists(movieId))
            {
                return false;
            }
            if(!ActorExists(movieActorName))
            {
                Actor newActor = new Actor();
                newActor.ActorName = movieActorName;
                if(!AddActor(newActor))
                {
                    return false;
                }
            }

            Actor actor = GetActor(movieActorName);
            if(actor == null)
            {
                return false;
            }

            MovieActor movieActor = new MovieActor();
            movieActor.ImdbId = movieId;
            movieActor.ActorId = actor.ActorId;
            _dbContext.MovieActors.Add(movieActor);
            if(_dbContext.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Assigns an Director to a Movie in the database.
        /// Adds the Director to the database if it does not exist.
        /// Returns true if successful.
        /// </summary>
        /// <param name="movieDirectorName"></param>
        /// <returns></returns>
        public bool AddMovieDirector(string movieId, string movieDirectorName)
        {
            if(!MovieExists(movieId))
            {
                return false;
            }
            if(!DirectorExists(movieDirectorName))
            {
                Director newDirector = new Director();
                newDirector.DirectorName = movieDirectorName;
                if(!AddDirector(newDirector))
                {
                    return false;
                }
            }

            Director director = GetDirector(movieDirectorName);
            if(director == null)
            {
                return false;
            }

            MovieDirector movieDirector = new MovieDirector();
            movieDirector.ImdbId = movieId;
            movieDirector.DirectorId = director.DirectorId;
            _dbContext.MovieDirectors.Add(movieDirector);
            if(_dbContext.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Assigns an Genre to a Movie in the database.
        /// Adds the Genre to the database if it does not exist.
        /// Returns true if successful; false otherwise.
        /// </summary>
        /// <param name="movieGenreName"></param>
        /// <returns></returns>
        public bool AddMovieGenre(string movieId, string movieGenreName)
        {
            if(!MovieExists(movieId))
            {
                return false;
            }
            if(!GenreExists(movieGenreName))
            {
                Genre newGenre = new Genre();
                newGenre.GenreName = movieGenreName;
                if(!AddGenre(newGenre))
                {
                    return false;
                }
            }

            Genre genre = GetGenre(movieGenreName);
            if(genre == null)
            {
                return false;
            }
            
            MovieGenre movieGenre = new MovieGenre();
            movieGenre.ImdbId = movieId;
            movieGenre.GenreId = genre.GenreId;
            _dbContext.MovieGenres.Add(movieGenre);
            if(_dbContext.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// Assigns an Language to a Movie in the database.
        /// Adds the Language to the database if it does not exist.
        /// Returns true if successful; false otherwise.
        /// </summary>
        /// <param name="movieLanguageName"></param>
        /// <returns></returns>
        public bool AddMovieLanguage(string movieId, string movieLanguageName)
        {
            if(!MovieExists(movieId))
            {
                return false;
            }
            if(!LanguageExists(movieLanguageName))
            {
                Language newLanguage = new Language();
                newLanguage.LanguageName = movieLanguageName;
                if(!AddLanguage(newLanguage))
                {
                    return false;
                }
            }

            Language language = GetLanguage(movieLanguageName);
            if(language == null)
            {
                return false;
            }
            
            MovieLanguage movieLanguage = new MovieLanguage();
            movieLanguage.ImdbId = movieId;
            movieLanguage.LanguageId = language.LanguageId;
            _dbContext.MovieLanguages.Add(movieLanguage);
            if(_dbContext.SaveChanges() > 0)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="movie"></param>
        /// <returns></returns>
        public bool UpdateMovie(Movie movie)
        {
            if(!MovieExists(movie.ImdbId))
            {
                return false;
            }
            _dbContext.Movies.Update(movie);
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

        public Rating GetRating(string ratingName)
        {
            return _dbContext.Ratings.FirstOrDefault(r => r.RatingName == ratingName);
        }

        public Actor GetActor(string actorName)
        {
            return _dbContext.Actors.FirstOrDefault(a => a.ActorName == actorName);
        }

        public Director GetDirector(string directorName)
        {
            return _dbContext.Directors.FirstOrDefault(d => d.DirectorName == directorName);
        }

        public Genre GetGenre(string genreName)
        {
            return _dbContext.Genres.FirstOrDefault(g => g.GenreName == genreName);
        }

        public Language GetLanguage(string languageName)
        {
            return _dbContext.Languages.FirstOrDefault(l => l.LanguageName == languageName);
        }

        public void ClearMovieActors(string imdbId)
        {
            var thisMoviesActors = _dbContext.MovieActors.Where(ma => ma.ImdbId == imdbId).ToList();
            _dbContext.MovieActors.RemoveRange(thisMoviesActors);
        }

        public void ClearMovieDirectors(string imdbId)
        {
            var thisMoviesDirectors = _dbContext.MovieDirectors.Where(md => md.ImdbId == imdbId).ToList();
            _dbContext.MovieDirectors.RemoveRange(thisMoviesDirectors);
        }

        public void ClearMovieGenres(string imdbId)
        {
            var thisMoviesGenres = _dbContext.MovieGenres.Where(mg => mg.ImdbId == imdbId).ToList();
            _dbContext.MovieGenres.RemoveRange(thisMoviesGenres);
        }

        public void ClearMovieLanguages(string imdbId)
        {
            var thisMoviesLanguages = _dbContext.MovieLanguages.Where(ml => ml.ImdbId == imdbId).ToList();
            _dbContext.MovieLanguages.RemoveRange(thisMoviesLanguages);
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
