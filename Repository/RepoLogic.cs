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

        public async Task<bool> AddMovie(Guid movieid)
        {
            if(MovieExists(movieid))
            {
                Console.WriteLine("RepoLogic.AddMovie() was called for a movie that doesn't exist.");
                return false;
            }
            Movie movie = new Movie();
            movie.MovieId = movieid;
            await _dbContext.Movies.AddAsync(movie);

            await _dbContext.SaveChangesAsync();
            return true;
        }
        private bool MovieExists(Guid movieid)
        {
            return (_dbContext.Movies.FirstOrDefault(m => m.MovieId == movieid) != null);
        }


        public async Task<List<Movie>> getAllThemoves()
        {
            return await _dbContext.Movies.ToListAsync();
        }
    }
}
