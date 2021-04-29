using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
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
        /// Adds a Movie to the database.
        /// </summary>
        /// <param name="movie"></param>
        /// <returns></returns>
        public void AddMovie(Movie movie)
        {
            _dbContext.Movies.Add(movie);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Adds a Rating to the database.
        /// </summary>
        /// <param name="rating"></param>
        /// <returns></returns>
        public void AddRating(Rating rating)
        {
            _dbContext.Ratings.Add(rating);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Adds an Actor to the database.
        /// </summary>
        /// <param name="actor"></param>
        /// <returns></returns>
        private void AddActor(Actor actor)
        {
            _dbContext.Actors.Add(actor);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Adds a Director to the database.
        /// </summary>
        /// <param name="director"></param>
        /// <returns></returns>
        private void AddDirector(Director director)
        {
            _dbContext.Directors.Add(director);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Adds a Genre to the database.
        /// </summary>
        /// <param name="genre"></param>
        /// <returns></returns>
        private void AddGenre(Genre genre)
        {
            _dbContext.Genres.Add(genre);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Adds a Language to the database.
        /// </summary>
        /// <param name="language"></param>
        /// <returns></returns>
        private void AddLanguage(Language language)
        {
            _dbContext.Languages.Add(language);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Adds a User's Tag Vote for a Movie to the database
        /// and increments the MovieTag's VoteSum.
        /// Adds the Tag to the database if it does not exist.
        /// Adds the MovieTag to the database if it does not
        /// exist.
        /// </summary>
        /// <param name="movieTagUser"></param>
        /// <returns></returns>
        public void AddMovieTagUser(MovieTagUser movieTagUser)
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
            _dbContext.SaveChanges();
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
                AddActor(newActor);
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
            _dbContext.SaveChanges();
            return true;
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
                AddDirector(newDirector);
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
            _dbContext.SaveChanges();
            return true;
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
                AddGenre(newGenre);
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
            _dbContext.SaveChanges();
            return true;
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
                AddLanguage(newLanguage);
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
            _dbContext.SaveChanges();
            return true;
        }

        /// <summary>
        /// Adds the movie specified in the argument to the user
        /// specified in the argument's following movie list.
        /// </summary>
        /// <param name="movieId"></param>
        /// <param name="userId"></param>
        public void AddFollowingMovie(string movieId, string userId)
        {
            if(FollowingMovieExists(movieId, userId))
            {
                return;
            }
            var followingMovie = new FollowingMovie();
            followingMovie.ImdbId = movieId;
            followingMovie.UserId = userId;
            _dbContext.FollowingMovies.Add(followingMovie);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Updates an existing Movie in the database.
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
            _dbContext.SaveChanges();
            return true;
        }

        /// <summary>
        /// Updates an existing Tag in the database.
        /// </summary>
        /// <param name="tag"></param>
        /// <returns></returns>
        public void UpdateTag(Tag tag)
        {
            _dbContext.Tags.Update(tag);
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Updates a User's existing Movie Tag in the database.
        /// </summary>
        /// <param name="movieTagUser"></param>
        /// <returns></returns>
        public void UpdateMovieTagUser(MovieTagUser movieTagUser)
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
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Returns a list of all Movies in the database.
        /// </summary>
        /// <returns></returns>
        public List<Movie> GetAllMovies()
        {
            return _dbContext.Movies.ToList<Movie>();
        }
        
        /// <summary>
        /// Returns a list of all Tags in the database.
        /// </summary>
        /// <returns></returns>
        public List<Tag> GetAllTags()
        {
            return _dbContext.Tags.ToList<Tag>();
        }

        /// <summary>
        /// Gets a MovieTag whose movieId and Tag name match the provided
        /// arguments. Returns null if no match is found.
        /// </summary>
        /// <param name="movieId"></param>
        /// <param name="tagName"></param>
        /// <returns></returns>
        private MovieTag GetMovieTag(string movieId, string tagName)
        {
            return _dbContext.MovieTags.FirstOrDefault(mt => mt.ImdbId == movieId && mt.TagName == tagName);
        }

        /// <summary>
        /// Gets the Movie whose movieId matches the provided argument.
        /// Returns null if no match is found.
        /// </summary>
        /// <param name="movieId"></param>
        /// <returns></returns>
        public Movie GetMovie(string movieId)
        {
            return _dbContext.Movies.FirstOrDefault(m => m.ImdbId == movieId);
        }

        /// <summary>
        /// Gets the Rating whose name matches the provided argument.
        /// Returns null if no match is found.
        /// </summary>
        /// <param name="ratingName"></param>
        /// <returns></returns>
        public Rating GetRating(string ratingName)
        {
            return _dbContext.Ratings.FirstOrDefault(r => r.RatingName == ratingName);
        }

        /// <summary>
        /// Gets the Rating whose ratingId matches the provided argument.
        /// Returns null if no match is found.
        /// </summary>
        /// <param name="ratingId"></param>
        /// <returns></returns>
        public Rating GetRating(int ratingId)
        {
            return _dbContext.Ratings.FirstOrDefault(r => r.RatingId == ratingId);
        }

        /// <summary>
        /// Gets the Actor whose name matches the provided argument.
        /// Returns null if no match is found.
        /// </summary>
        /// <param name="actorName"></param>
        /// <returns></returns>
        private Actor GetActor(string actorName)
        {
            return _dbContext.Actors.FirstOrDefault(a => a.ActorName == actorName);
        }

        /// <summary>
        /// Gets the Director whose name matches the provided argument.
        /// Returns null if no match is found.
        /// </summary>
        /// <param name="directorName"></param>
        /// <returns></returns>
        private Director GetDirector(string directorName)
        {
            return _dbContext.Directors.FirstOrDefault(d => d.DirectorName == directorName);
        }

        /// <summary>
        /// Gets the Genre whose name matches the provided argument.
        /// Returns null if no match is found.
        /// </summary>
        /// <param name="genreName"></param>
        /// <returns></returns>
        private Genre GetGenre(string genreName)
        {
            return _dbContext.Genres.FirstOrDefault(g => g.GenreName == genreName);
        }

        /// <summary>
        /// Gets the Language whose name matches the provided argument.
        /// Returns null if no match is found.
        /// </summary>
        /// <param name="languageName"></param>
        /// <returns></returns>
        private Language GetLanguage(string languageName)
        {
            return _dbContext.Languages.FirstOrDefault(l => l.LanguageName == languageName);
        }

        /// <summary>
        /// Gets the Tag whose name matches the provided argument.
        /// Returns null if no match is found.
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public Tag GetTag(string tagName)
        {
            return _dbContext.Tags.FirstOrDefault(t => t.TagName == tagName);
        }

        /// <summary>
        /// Returns a list containing the name of each Actor associated with
        /// the movieId provided in the argument.
        /// </summary>
        /// <param name="movieId"></param>
        /// <returns></returns>
        public List<string> GetMovieActorNames(string movieId)
        {
            var movieActors = _dbContext.MovieActors.Where(ma => ma.ImdbId == movieId);
            var movieActorIds = new List<Guid>();
            foreach (var movieActor in movieActors)
            {
                movieActorIds.Add(movieActor.ActorId);
            }
            var movieActorNames = new List<string>();
            foreach (var movieActorId in movieActorIds)
            {
                var actor = _dbContext.Actors.FirstOrDefault(a => a.ActorId == movieActorId);
                movieActorNames.Add(actor.ActorName);
            }
            return movieActorNames;
        }

        /// <summary>
        /// Returns a list containing the name of each Director associated with
        /// the movieId provided in the argument.
        /// </summary>
        /// <param name="movieId"></param>
        /// <returns></returns>
        public List<string> GetMovieDirectorNames(string movieId)
        {
            var movieDirectors = _dbContext.MovieDirectors.Where(ma => ma.ImdbId == movieId);
            var movieDirectorIds = new List<Guid>();
            foreach (var movieDirector in movieDirectors)
            {
                movieDirectorIds.Add(movieDirector.DirectorId);
            }
            var movieDirectorNames = new List<string>();
            foreach (var movieDirectorId in movieDirectorIds)
            {
                var actor = _dbContext.Directors.FirstOrDefault(a => a.DirectorId == movieDirectorId);
                movieDirectorNames.Add(actor.DirectorName);
            }
            return movieDirectorNames;
        }

        /// <summary>
        /// Returns a list containing the name of each Genre associated with
        /// the movieId provided in the argument.
        /// </summary>
        /// <param name="movieId"></param>
        /// <returns></returns>
        public List<string> GetMovieGenreNames(string movieId)
        {
            var movieGenres = _dbContext.MovieGenres.Where(ma => ma.ImdbId == movieId);
            var movieGenreIds = new List<Guid>();
            foreach (var movieGenre in movieGenres)
            {
                movieGenreIds.Add(movieGenre.GenreId);
            }
            var movieGenreNames = new List<string>();
            foreach (var movieGenreId in movieGenreIds)
            {
                var actor = _dbContext.Genres.FirstOrDefault(a => a.GenreId == movieGenreId);
                movieGenreNames.Add(actor.GenreName);
            }
            return movieGenreNames;
        }

        /// <summary>
        /// Returns a list containing the name of each Language associated with
        /// the movieId provided in the argument.
        /// </summary>
        /// <param name="movieId"></param>
        /// <returns></returns>
        public List<string> GetMovieLanguageNames(string movieId)
        {
            var movieLanguages = _dbContext.MovieLanguages.Where(ma => ma.ImdbId == movieId);
            var movieLanguageIds = new List<Guid>();
            foreach (var movieLanguage in movieLanguages)
            {
                movieLanguageIds.Add(movieLanguage.LanguageId);
            }
            var movieLanguageNames = new List<string>();
            foreach (var movieLanguageId in movieLanguageIds)
            {
                var actor = _dbContext.Languages.FirstOrDefault(a => a.LanguageId == movieLanguageId);
                movieLanguageNames.Add(actor.LanguageName);
            }
            return movieLanguageNames;
        }

        /// <summary>
        /// Returns a list containing all MovieTag objects associated with
        /// the movieId provided in the argument.
        /// </summary>
        /// <param name="movieId"></param>
        /// <returns></returns>
        public List<MovieTag> GetMovieTags(string movieId)
        {
            return _dbContext.MovieTags.Where(mt => mt.ImdbId == movieId).ToList();
        }

        /// <summary>
        /// Returns a list containing the name of every Movie that the user
        /// specified in the argument is following.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        public List<string> GetFollowingMovies(string userId)
        {
            var followingMovies = _dbContext.FollowingMovies.Where(fm => fm.UserId == userId);
            var movieNames = new List<string>();
            foreach (var followingMovie in followingMovies)
            {
                movieNames.Add(followingMovie.ImdbId);
            }
            return movieNames;
        }

        /// <summary>
        /// Removes all actors from the Movie assocaited with the
        /// provided movie Id.
        /// </summary>
        /// <param name="imdbId"></param>
        public void ClearMovieActors(string imdbId)
        {
            _dbContext.MovieActors.RemoveRange(_dbContext.MovieActors.Where(ma => ma.ImdbId == imdbId));
        }

        /// <summary>
        /// Removes all directors from the Movie assocaited with the
        /// provided movie Id.
        /// </summary>
        /// <param name="imdbId"></param>
        public void ClearMovieDirectors(string imdbId)
        {
            _dbContext.MovieDirectors.RemoveRange(_dbContext.MovieDirectors.Where(md => md.ImdbId == imdbId));
        }

        /// <summary>
        /// Removes all genres from the Movie assocaited with the
        /// provided movie Id.
        /// </summary>
        /// <param name="imdbId"></param>
        public void ClearMovieGenres(string imdbId)
        {
            _dbContext.MovieGenres.RemoveRange(_dbContext.MovieGenres.Where(mg => mg.ImdbId == imdbId));
        }

        /// <summary>
        /// Removes all languages from the Movie assocaited with the
        /// provided movie Id.
        /// </summary>
        /// <param name="imdbId"></param>
        public void ClearMovieLanguages(string imdbId)
        {
            _dbContext.MovieLanguages.RemoveRange(_dbContext.MovieLanguages.Where(ml => ml.ImdbId == imdbId));
        }

        /// <summary>
        /// Deletes the movie associated with the provided movie Id.
        /// Removes the movie's references to all Tags, Actors,
        /// Directors, Genres, and Languages as well; but does not
        /// delete the Tags, Actors, etc. themselves, only the references
        /// between these entities and the specified movie.
        /// </summary>
        /// <param name="movieId"></param>
        /// <returns></returns>
        public void DeleteMovie(string movieId)
        {
            _dbContext.MovieTags.RemoveRange(_dbContext.MovieTags.Where(mt => mt.ImdbId == movieId));
            _dbContext.MovieTagUsers.RemoveRange(_dbContext.MovieTagUsers.Where(mtu => mtu.ImdbId == movieId));
            _dbContext.MovieActors.RemoveRange(_dbContext.MovieActors.Where(ma => ma.ImdbId == movieId));
            _dbContext.MovieDirectors.RemoveRange(_dbContext.MovieDirectors.Where(md => md.ImdbId == movieId));
            _dbContext.MovieGenres.RemoveRange(_dbContext.MovieGenres.Where(mg => mg.ImdbId == movieId));
            _dbContext.MovieLanguages.RemoveRange(_dbContext.MovieLanguages.Where(ml => ml.ImdbId == movieId));
            _dbContext.Movies.RemoveRange(_dbContext.Movies.Where(m => m.ImdbId == movieId));
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Removes the movie specified in the argument from the user
        /// specified in the argument's following movie list.
        /// </summary>
        /// <param name="movieId"></param>
        /// <param name="userId"></param>
        public void DeleteFollowingMovie(string movieId, string userId)
        {
            if(!FollowingMovieExists(movieId, userId))
            {
                return;
            }
            _dbContext.FollowingMovies.Remove(_dbContext.FollowingMovies.FirstOrDefault(fm =>
                fm.ImdbId == movieId && fm.UserId == userId));
            _dbContext.SaveChanges();
        }

        /// <summary>
        /// Returns true iff the Movie specified by the argument
        /// exists.
        /// </summary>
        /// <param name="movieId"></param>
        /// <returns></returns>
        public bool MovieExists(string movieId)
        {
            return (_dbContext.Movies.FirstOrDefault(m => m.ImdbId == movieId) != null);
        }

        /// <summary>
        /// Returns true iff the Tag specified by the argument
        /// exists.
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        public bool TagExists(string tagName)
        {
            return (_dbContext.Tags.FirstOrDefault(t => t.TagName == tagName) != null);
        }

        /// <summary>
        /// Returns true iff the Rating specified by the argument
        /// exists.
        /// </summary>
        /// <param name="ratingName"></param>
        /// <returns></returns>
        public bool RatingExists(string ratingName)
        {
            return (_dbContext.Ratings.FirstOrDefault(r => r.RatingName == ratingName) != null);
        }

        /// <summary>
        /// Returns true iff the Actor specified by the argument
        /// exists.
        /// </summary>
        /// <param name="actorName"></param>
        /// <returns></returns>
        public bool ActorExists(string actorName)
        {
            return (_dbContext.Actors.FirstOrDefault(a => a.ActorName == actorName) != null);
        }

        /// <summary>
        /// Returns true iff the Language specified by the argument
        /// exists.
        /// </summary>
        /// <param name="languageName"></param>
        /// <returns></returns>
        public bool LanguageExists(string languageName)
        {
            return (_dbContext.Languages.FirstOrDefault(l => l.LanguageName == languageName) != null);
        }

        /// <summary>
        /// Returns true iff the Director specified by the argument
        /// exists.
        /// </summary>
        /// <param name="directorName"></param>
        /// <returns></returns>
        public bool DirectorExists(string directorName)
        {
            return (_dbContext.Directors.FirstOrDefault(d => d.DirectorName == directorName) != null);
        }

        /// <summary>
        /// Returns true iff the Genre specified by the argument
        /// exists.
        /// </summary>
        /// <param name="genreName"></param>
        /// <returns></returns>
        public bool GenreExists(string genreName)
        {
            return (_dbContext.Genres.FirstOrDefault(g => g.GenreName == genreName) != null);
        }

        /// <summary>
        /// Returns true iff the MovieActor specified by the
        /// argument exists.
        /// </summary>
        /// <param name="movieId"></param>
        /// <param name="actorName"></param>
        /// <returns></returns>
        public bool MovieActorExists(string movieId, string actorName)
        {
            Actor actor = _dbContext.Actors.FirstOrDefault(a => a.ActorName == actorName);
            if(actor == null)
            {
                return false;
            }
            
            return (_dbContext.MovieActors.FirstOrDefault(ma => ma.ImdbId == movieId
                && ma.ActorId == actor.ActorId) != null);
        }

        /// <summary>
        /// Returns true iff the MovieDirector specified by the
        /// argument exists.
        /// </summary>
        /// <param name="movieId"></param>
        /// <param name="directorName"></param>
        /// <returns></returns>
        public bool MovieDirectorExists(string movieId, string directorName)
        {
            Director director = _dbContext.Directors.FirstOrDefault(d => d.DirectorName == directorName);
            if(director == null)
            {
                return false;
            }

            return (_dbContext.MovieDirectors.FirstOrDefault(md => md.ImdbId == movieId
                && md.DirectorId == director.DirectorId) != null);
        }

        /// <summary>
        /// Returns true iff the MovieGenre specified by the
        /// argument exists.
        /// </summary>
        /// <param name="movieId"></param>
        /// <param name="genreName"></param>
        /// <returns></returns>
        public bool MovieGenreExists(string movieId, string genreName)
        {
            Genre genre = _dbContext.Genres.FirstOrDefault(g => g.GenreName == genreName);
            if(genre == null)
            {
                return false;
            }

            return (_dbContext.MovieGenres.FirstOrDefault(mg => mg.ImdbId == movieId
                && mg.GenreId == genre.GenreId) != null);
        }

        /// <summary>
        /// Returns true iff the MovieLanguage specified by the
        /// argument exists.
        /// </summary>
        /// <param name="movieId"></param>
        /// <param name="languageName"></param>
        /// <returns></returns>
        public bool MovieLanguageExists(string movieId, string languageName)
        {
            Language language = _dbContext.Languages.FirstOrDefault(l => l.LanguageName == languageName);
            if(language == null)
            {
                return false;
            }

            return (_dbContext.MovieLanguages.FirstOrDefault(ml => ml.ImdbId == movieId
                && ml.LanguageId == language.LanguageId) != null);
        }

        /// <summary>
        /// Returns true iff the MovieTagUser specified by the
        /// argument exists.
        /// </summary>
        /// <param name="movieTagUser"></param>
        /// <returns></returns>
        public bool MovieTagUserExists(MovieTagUser movieTagUser)
        {
            return (_dbContext.MovieTagUsers.FirstOrDefault(mtu => mtu.ImdbId == movieTagUser.ImdbId
                && mtu.TagName == movieTagUser.TagName && mtu.UserId == movieTagUser.UserId) != null);
        }

        public bool FollowingMovieExists(string movieId, string userId)
        {
            return (_dbContext.FollowingMovies.FirstOrDefault(fm => fm.ImdbId == movieId 
                && fm.UserId == userId) != null);
        }
    }
}
