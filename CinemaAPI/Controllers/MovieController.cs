using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Logic;
using Logic.ApiHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Model;

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
        public async Task<ActionResult<MovieDTO>> GetMovie(string movieid)
        {
            MovieDTO movieDTO = await _movieLogic.GetMovie(movieid);
            if(movieDTO == null)
            {
                return StatusCode(404);
            }
            StatusCode(200);
            return movieDTO;
        }

        /// <summary>
        /// Returns the movieId for each movie that matches all of the tags
        /// passed in as tag:[value] pairs. This is a POST method because 
        /// GET does not allow body data.
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        [HttpPost("search")]
        public ActionResult<List<string>> SearchMovies([FromBody] Dictionary<string, string> filters)
        {
            var movies = _movieLogic.SearchMovies(filters);
            if (movies == null)
            {
                return StatusCode(404);
            }
            StatusCode(200);
            return movies;
        }

        /// <summary>
        /// Updates every field of the movie with a matching ImdbId to
        /// the provided values. Sets missing values to null. Adds the
        /// movie if it does not exist.
        /// </summary>
        /// <param name="movieDTO"></param>
        /// <returns></returns>
        [HttpPatch("update")]
        public ActionResult CreateOrUpdateMovie([FromBody] MovieDTO movieDTO)
        {
            if(!ModelState.IsValid)
            {
                return StatusCode(400);
            }
            if(_movieLogic.CreateOrUpdateMovie(movieDTO))
            {
                return StatusCode(200);
            }
            else
            {
                Console.WriteLine("false 400");
                return StatusCode(400);
            }
        }

        /// <summary>
        /// Updates the fields of the movie with a matching ImdbId to the
        /// provided non-null/empty values. If any of the passed-in values
        /// are null/empty, they will remain unchanged. The passed-in array
        /// fields will be appened to the existing lists. Pre-existing entries
        /// in the lists will remain. If the movie does not yet exist, the
        /// movie is first added via the public movie API.
        /// </summary>
        /// <param name="movieDTO"></param>
        /// <returns></returns>
        [HttpPatch("append")]
        public async Task<ActionResult> AppendMovie([FromBody] MovieDTO movieDTO)
        {
            if(!ModelState.IsValid)
            {
                return StatusCode(400);
            }
            if(await _movieLogic.AppendMovie(movieDTO))
            {
                return StatusCode(200);
            }
            else
            {
                return StatusCode(400);
            }
        }

        /// <summary>
        /// Removes the movie from the database. 
        /// </summary>
        /// <param name="movieId"></param>
        /// <returns></returns>
        [HttpDelete("{movieId}")]
        public ActionResult DeleteMovie(string movieId)
        {
            if(_movieLogic.DeleteMovie(movieId))
            {
                return StatusCode(200);
            }
            else
            {
                return StatusCode(400);
            }
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
            if(await _movieLogic.TagMovie(taggingDTO))
            {
                return StatusCode(200);
            }
            else
            {
                return StatusCode(400);
            }
        }

        /// <summary>
        /// Bans the specified tag. This is only available to Moderators
        /// and Administrators.
        /// </summary>
        /// <param name="tagname"></param>
        /// <returns></returns>
        [HttpPost("tag/ban/{tagname}")]
        public ActionResult BanTag(string tagname)
        {
            if(_movieLogic.SetTagBanStatus(tagname, true))
            {
                return StatusCode(200);
            }
            else
            {
                return StatusCode(404);
            }
        }

        /// <summary>
        /// Unbans the specified tag. This is only available to Moderators
        /// and Administrators.
        /// </summary>
        /// <param name="tagname"></param>
        /// <returns></returns>
        [HttpPost("tag/unban/{tagname}")]
        public ActionResult UnbanTag(string tagname)
        {
            if(_movieLogic.SetTagBanStatus(tagname, false))
            {
                return StatusCode(200);
            }
            else
            {
                return StatusCode(404);
            }
        }

        /// <summary>
        /// Example for using authentication
        /// </summary>
        /// <returns></returns>
        [HttpGet("authexample")]
        [Authorize]
        public ActionResult<string> GetExample()
        {
            return "Success";
        }

        /// <summary>
        /// Temporary endpoint for testing the Kubernetes deployment.
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
