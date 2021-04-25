using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Logic;
using Logic.ApiHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Model;
using Repository.Models;

namespace CinemaAPI.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class MovieController : ControllerBase
    {
        private readonly IMovieLogic _movieLogic;

        public MovieController(IMovieLogic movieLogic)
        {
            _movieLogic = movieLogic;
        }

        /// <summary>
        /// Returns detailed information for the specified movieid
        /// </summary>
        /// <param name="movieid"></param>
        /// <returns></returns>
        [HttpGet("{movieid}")]
        public async Task<MovieDTO> GetMovie(string movieid)
        {
            await _movieLogic.GetMovie(movieid);
        }

        /// <summary>
        /// Adds a new Movie based on the information provided.
        /// Returns a 400 status code if creation fails.
        /// </summary>
        /// <param name="movieid"></param>
        /// <returns></returns>
        [HttpPost("{movieid}")]
        public async Task<IActionResult> AddMovie(string movieid)
        {
            if (await _movieLogic.AddMovie(movieid))
            {
                return new StatusCodeResult(201);
            }
            return new StatusCodeResult(400);
        }

        /// <summary>
        /// Returns the movieId for each movie that matches all of the tags
        /// passed in as tag:[value] pairs. Returns a 404 response if any of
        /// the tags are invalid.
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        [HttpGet("filtered")]
        public ActionResult<List<string>> GetMoviesFiltered([FromBody] Dictionary<string, string> filters)
        {
            var movies = _movieLogic.GetMoviesFiltered(filters);
            if (movies == null)
            {
                return StatusCode(404);
            }
            StatusCode(200);
            return movies;
        }

        [HttpPatch("update/{imdb}")]
        public async Task<ActionResult> UpdateMovie(string imdb,Movie movie)
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
        /// Submits a vote as to whether the specified tag is associated
        /// with the specified movie. Each user may have only one vote
        /// per movie/tag combination.
        /// </summary>
        /// <param name="taggingDTO"></param>
        /// <returns></returns>
        [HttpPost("tag/movie")]
        public async Task<ActionResult> TagMovie([FromBody] TaggingDTO taggingDTO)
        {
            await _movieLogic.TagMovie(taggingDTO);
        }

        /// <summary>
        /// Bans the specified tag. This is only available to Moderators
        /// and Administrators.
        /// </summary>
        /// <param name="tagname"></param>
        /// <returns></returns>
        [HttpPost("tag/ban/{tagname}")]
        public async Task<ActionResult> BanTag(string tagname)
        {
            await _movieLogic.BanTag(tagname);
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

        /// <summary>
        /// Only for testing the Kubernetes deployment.
        /// </summary>
        /// <returns></returns>
        [HttpGet("test")]
        public MovieObject GetTestObject()
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
    }
}
