using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repository.Models;

namespace Repository
{
    public class RepoLogic
    {
        private readonly MovieDBContext _dbContext;

        public RepoLogic(MovieDBContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<bool> AddMovie(string movieid)
        {
            if(MovieExists(movieid))
            {
                Console.WriteLine("RepoLogic.AddMovie() was called for a movie that doesn't exist.");
                return false;
            }
            Movie movie = new Movie
            {
                ImdbId = movieid
            };
            await _dbContext.Movies.AddAsync(movie);

            await _dbContext.SaveChangesAsync();
            return true;
        }
        private bool MovieExists(string movieid)
        {
            return (_dbContext.Movies.FirstOrDefault(m => m.ImdbId == movieid) != null);
        }

        public async Task<List<Movie>> getAllThemoves()
        {
            return await _dbContext.Movies.ToListAsync();
        }
        // Not working properly 
        public  List<Movie> getAllByActor(string actor)
        {
            if (!ActorExist(actor))
            {
                return null;
            }

            return _dbContext.Movies.Include("MovieActors").Include("Actor").ToList();

            //return _dbContext.Movies
            //    .FromSqlRaw(
            //        $"SELECT * FROM Movie m Join Movie_Actor ma on m.imdbId = ma.imdbId Join Actor a on a.actorId = ma.actorId where a.actorName = {actor}")
            //    .ToList();
        }
        public async Task<Movie> getOneMovie(string imdb)
        {
            if (!MovieExist(imdb))
            {
                return null;
            }

            return await _dbContext.Movies.FirstOrDefaultAsync(a=>a.ImdbId == imdb);
        }

        public async Task<bool> updateMovie(string imdb, Movie updatedMovie)
        {
            var exisitingMovie = _dbContext.Movies.FirstOrDefault(a => a.ImdbId == imdb);
            if (exisitingMovie == null)
            {
                return false;
            }
            exisitingMovie.Plot = updatedMovie.Plot;
            await _dbContext.SaveChangesAsync();
            return true;
        }
        private bool ActorExist(string actor)
        {
            return (_dbContext.Actors.FirstOrDefault(a => a.ActorName == actor) != null);
        }
        private bool LanguageExist(string language)
        {
            return (_dbContext.Languages.FirstOrDefault(a => a.LanguageName ==language) != null);
        }
        private bool DirectorExist(string director)
        {
            return (_dbContext.Directors.FirstOrDefault(a => a.DirectorName == director) != null);
        }
        private bool GenreExist(string genre)
        {
            return (_dbContext.Genres.FirstOrDefault(a => a.GenreName == genre) != null);
        }
        private bool MovieExist(string movieIMDB)
        {
            return (_dbContext.Movies.FirstOrDefault(a => a.ImdbId == movieIMDB) != null);
        }
    }
}
