using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Repository.Models;

namespace Logic
{
    public interface IMovieLogic
    {
        Task<bool> CreateMovie(string movieId);
        Task<List<Movie>> GetAllMovies();

        List<Movie> getAllMoviesByActor(string actor);
        Task<Movie> getOneMovie(string imdb);
        Task<bool> UpdatedPlotMovie(string imdb, Movie movie);
    }
}
