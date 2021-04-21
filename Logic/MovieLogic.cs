using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
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
        public async Task<bool> CreateMovie(Guid movieId)
        {
            return await _repo.AddMovie(movieId);
        }

        public async Task<List<Movie>> GetAllMovies()
        {
            return await _repo.getAllThemoves();
        }
    }
}
