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
        public async Task<bool> CreateMovie(string movieId)
        {
            return await _repo.AddMovie(movieId);
        }

        public async Task<List<Movie>> GetAllMovies()
        {
            return await _repo.getAllThemoves();
        }

        public  List<Movie> getAllMoviesByActor(string actor)
        {
            return  _repo.getAllByActor(actor);
        }

        public List<Movie> getAllMoviesByDirector(string director)
        {
            return _repo.getAllByDirector(director);
        }

        public List<Movie> getAllMoviesByGenre(string genre)
        {
            return _repo.getAllByGenre(genre);
        }

        public List<Movie> getAllMoviesByLanguage(string language)
        {
            return _repo.getAllByLanguage(language);
        }

        public async Task<Movie> getOneMovie(string imdb)
        {
            return await _repo.getOneMovie(imdb);
        }

        public  Movie UpdatedPlotMovie( Movie movie)
        {
            return  _repo.updateMovie(movie);
        }
    }
}
