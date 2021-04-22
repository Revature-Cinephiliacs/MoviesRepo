using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Logic;
using Logic.ApiHelper;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Repository.Models;


namespace CinemaAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MovieController
    {
        private readonly IMovieLogic _movieLogic;

        public MovieController(IMovieLogic movieLogic)
        {
            _movieLogic = movieLogic;
        }

        [HttpGet("api/{search}")]
        public async Task<MovieObject> getMovieObject(string search)
        {
            return await MovieProcessor.LoadMovie(search);
        }

        /// <summary>
        /// Adds a new Movie based on the information provided.
        /// Returns a 400 status code if creation fails.
        /// </summary>
        /// <param name="movieid"></param>
        /// <returns></returns>
        [HttpPost("{movieid}")]
        public async Task<IActionResult> CreateMovie(string movieid)
        {
            if (await _movieLogic.CreateMovie(movieid))
            {
                return new StatusCodeResult(201);
            }
            return new StatusCodeResult(400);
        }

        [HttpGet]
        public async Task<List<Movie>> GetThem()
        {
            return await _movieLogic.GetAllMovies();
        }

        [HttpGet("byActor/{actor}")]
        public ActionResult<List<Movie>> GetAllMoviesByActor(string actor)
        {
            List<Movie> movies =  _movieLogic.getAllMoviesByActor(actor);
            if (movies == null)
            {
                return new StatusCodeResult(404);
            }
            new StatusCodeResult(200);
            return movies;
        }

        [HttpGet("byGenre/{genre}")]
        public  ActionResult<List<Movie>> GetAllMoviesByGenre(string genre)
        {
            List<Movie> movies =  _movieLogic.getAllMoviesByGenre(genre);
            if (movies == null)
            {
                return new StatusCodeResult(404);
            }
            new StatusCodeResult(200);
            return movies;
        }
        [HttpGet("byDirector/{director}")]
        public  ActionResult<List<Movie>> GetAllMoviesByDir(string director)
        { 
            List<Movie> movies =  _movieLogic.getAllMoviesByDirector(director);
            if (movies == null)
            {
                return new StatusCodeResult(404);
            }
            new StatusCodeResult(200);
            return movies;

        }

        [HttpGet("byLanguage/{language}")]
        public  ActionResult<List<Movie>> GetAllMoviesByLanguage(string language)
        {
            List<Movie> movies =  _movieLogic.getAllMoviesByLanguage(language);
            if (movies == null)
            {
                return new StatusCodeResult(404);
            }
            new StatusCodeResult(200);
            return movies;
        }

        [HttpGet("byIMDB/{imdb}")]
        public async Task<ActionResult<Movie>> getOneM(string imdb)
        {
            var movie = await _movieLogic.getOneMovie(imdb);
            if (movie == null)
            {
                return  new StatusCodeResult(404);
            }
            new StatusCodeResult(200);
            return movie;
        }
        [HttpPatch("update/{imdb}")]
        public async Task<ActionResult> updateMovie(string imdb,Movie movie)
        {
            var movieExist = await _movieLogic.getOneMovie(imdb);

            if (movieExist != null)
            {
                movie.ImdbId = movieExist.ImdbId;
                _movieLogic.UpdatedPlotMovie(movie);
                return new StatusCodeResult(200);
            }

            return new StatusCodeResult(404);

        }
    }
}
