using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Logic;
using Logic.ApiHelper;
using Microsoft.AspNetCore.Authorization;
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
        /// Only for testing the Kubernetes deployment.
        /// </summary>
        /// <returns></returns>
        [HttpGet("test")]
        public MovieObject getTestObject()
        {
            MovieObject mo = new MovieObject();
            mo.Title = "Test Str";
            mo.Year = "Test Str";
            mo.Rated = "Test Str";
            mo.Released = "Test Str";
            mo.RunTime = "Test Str";
            mo.Genre = "Test Str";
            mo.Director = "Test Str";
            mo.Writer = "Test Str";
            mo.Actors = "Test Str";
            mo.Plot = "Test Str";
            mo.Language = "Test Str";
            mo.Country = "Test Str";
            mo.Awards = "Test Str";
            mo.Poster = "Test Str";
            mo.Ratings = null;
            mo.Metascore = "Test Str";
            mo.imdbVotes = "Test Str";
            mo.imdbID = "Test Str";
            mo.Type = "Test Str";
            mo.DVD = "Test Str";
            mo.BoxOffice = "Test Str";
            mo.Production = "Test Str";
            mo.Website = "Test Str";
            mo.Response = "Test Str";
            return mo;
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

        /// <summary>
        /// Example for using authentication
        /// </summary>
        /// <returns></returns>
        [HttpGet("users")]
        [Authorize]
        public async Task<ActionResult<string>> GetExample()
        {
            return "Success";
        }
    }
}
