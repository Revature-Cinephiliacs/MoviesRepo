using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Logic;
using Logic.ApiHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using RestSharp;
using Newtonsoft.Json;
using System.Net.Http.Json;

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
        [HttpGet("{movieId}")]
        public async Task<ActionResult<MovieDTO>> GetMovie(string movieId)
        {
            MovieDTO movieDTO = await _movieLogic.GetMovie(movieId);
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
        public ActionResult<List<string>> SearchMovies([FromBody] Dictionary<string, string[]> filters)
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
        /// Creates a movie based on the information in the argument.
        /// </summary>
        /// <param name="movieDTO"></param>
        /// <returns></returns>
        [HttpPost]
        [Authorize]
        public async Task<ActionResult> CreateMovie([FromBody] MovieDTO movieDTO)
        {
            if(!ModelState.IsValid)
            {
                return StatusCode(400);
            }
            if(await _movieLogic.CreateMovie(movieDTO))
            {
                return StatusCode(200);
            }
            else
            {
                return StatusCode(400);
            }
        }

        /// <summary>
        /// Updates every field of the movie with a matching movieId to
        /// the provided values. Sets missing values to null. Adds the
        /// movie if it does not exist.
        /// </summary>
        /// <param name="movieDTO"></param>
        /// <returns></returns>
        [HttpPut("{movieId}")]
        [Authorize]
        public async Task<ActionResult> UpdateMovie(string movieId, [FromBody] MovieDTO movieDTO)
        {
            if(!ModelState.IsValid)
            {
                return StatusCode(400);
            }
            if(await _movieLogic.UpdateMovie(movieId, movieDTO))
            {
                return StatusCode(200);
            }
            else
            {
                return StatusCode(400);
            }
        }

        /// <summary>
        /// Updates the fields of the movie with a matching movieId to the
        /// provided non-null/empty values. If any of the passed-in values
        /// are null/empty, they will remain unchanged. The passed-in array
        /// fields will be appened to the existing lists. Pre-existing entries
        /// in the lists will remain. If the movie does not yet exist, the
        /// movie is first added via the public movie API.
        /// </summary>
        /// <param name="movieDTO"></param>
        /// <returns></returns>
        [HttpPatch("{movieId}")]
        [Authorize]
        public async Task<ActionResult> AppendMovie(string movieId, [FromBody] MovieDTO movieDTO)
        {
            if(!ModelState.IsValid)
            {
                return StatusCode(400);
            }
            if(await _movieLogic.AppendMovie(movieId, movieDTO))
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
        [Authorize("manage:awebsite")]
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
        [HttpPost("tags")]
        [Authorize]
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
        /// Returns all currently existing tag names, excluding banned
        /// tags.
        /// </summary>
        /// <returns></returns>
        [HttpGet("tags")]
        public ActionResult<List<string>> GetAllTags()
        {
            return _movieLogic.GetAllTags();
        }

        /// <summary>
        /// Bans the specified tag. This is only available to Moderators
        /// and Administrators.
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        [HttpPut("tag/ban/{tagName}")]
        [Authorize("manage:awebsite")]
        public ActionResult BanTag(string tagName)
        {
            if(_movieLogic.SetTagBanStatus(tagName, true))
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
        /// <param name="tagName"></param>
        /// <returns></returns>
        [HttpDelete("tag/ban/{tagName}")]
        [Authorize("manage:awebsite")]
        public ActionResult UnbanTag(string tagName)
        {
            if(_movieLogic.SetTagBanStatus(tagName, false))
            {
                return StatusCode(200);
            }
            else
            {
                return StatusCode(404);
            }
        }

        /// <summary>
        /// Adds the movie to the user's following-movies list.
        /// </summary>
        /// <param name="movieId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpPut("follow/{movieId}")]
        [Authorize]
        public async Task<ActionResult> FollowMovie(string movieId)
        {
            var response = await Helpers.Helper.Sendrequest("/userdata", Method.GET, Helpers.Helper.GetTokenFromRequest(this.Request));
            var userId = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content)["sub"];
            if(_movieLogic.FollowMovie(movieId, userId))
            {
                return StatusCode(200);
            }
            else
            {
                return StatusCode(404);
            }
        }

        /// <summary>
        /// Removes the movie from the user's following-movies list.
        /// </summary>
        /// <param name="movieId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpDelete("follow/{movieId}")]
        [Authorize]
        public async Task<ActionResult> UnfollowMovie(string movieId)
        {
            var response = await Helpers.Helper.Sendrequest("/userdata", Method.GET, Helpers.Helper.GetTokenFromRequest(this.Request));
            var userId = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content)["sub"];
            if(_movieLogic.UnfollowMovie(movieId, userId))
            {
                return StatusCode(200);
            }
            else
            {
                return StatusCode(404);
            }
        }

        /// <summary>
        /// Returns all movies that the user is following.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("follow")]
        public async Task<ActionResult<List<string>>> GetFollowingMovies()
        {
            var response = await Helpers.Helper.Sendrequest("/userdata", Method.GET, Helpers.Helper.GetTokenFromRequest(this.Request));
            var userId = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content)["sub"];
            return _movieLogic.GetFollowingMovies(userId);
        }

        /// <summary>
        /// Takes a review packet from reviews api,
        /// and gets a list of users following the movie.
        /// Returns response code.
        /// </summary>
        /// <param name="review"></param>
        /// <returns></returns>
        [HttpPost("review/notification")] //Needs endpoint -Larson
        [Authorize]
        public async Task<ActionResult<bool>> RetrieveNewReview([FromBody] ReviewNotification reviewNotification)
        {
            var review = _movieLogic.GetFollowersForReviewNotification(reviewNotification);
            if(review.Followers != null){
                await Logic.ApiHelper.ApiProcessor.SendReviewNotification(review);
                return StatusCode(200);
            }else{
                return StatusCode(404);
            }
            
        }

        /// <summary>
        /// Takes a forum packet from forum api,
        /// and gets a list of users following the movie.
        /// Returns response code.
        /// </summary>
        /// <param name="forumNotification"></param>
        /// <returns></returns>
        [HttpPost("discussion/notification")] //Needs endpoint -Larson
        [Authorize]
        public async Task<ActionResult<bool>> RetrieveNewDiscussion([FromBody] ForumNotification forumNotification)
        {
            var forumNote = _movieLogic.GetFollowersForForumNotification(forumNotification);
            if(forumNote.Followers != null){
                await Logic.ApiHelper.ApiProcessor.SendForumNotification(forumNote);
                return StatusCode(200);
            }else{
                return StatusCode(404);
            }
        }

        /// <summary>
        /// retuns recommended movies basewd
        /// </summary>
        /// <param name="imdbId"></param>
        /// <returns></returns>

        [HttpGet("recommended/{imdbId}")]
        public async Task<ActionResult<List<MovieDTO>>> getRecommended(string imdbId)
        {
            List<MovieDTO> movieDto = await _movieLogic.recommendedMovies(imdbId);
            if (movieDto == null)
            {
                return StatusCode(404);
            }

            StatusCode(200);
            return movieDto;
        }

        /// <summary>
        /// return a list of movies recommended by the userId
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        [HttpGet("recommendedByUserId")]
        [Authorize]
        public async Task<ActionResult<List<MovieDTO>>> getRecommendedById()
        {
            var response = await Helpers.Helper.Sendrequest("/userdata", Method.GET, Helpers.Helper.GetTokenFromRequest(this.Request));
            var userId = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content)["sub"];
            List<MovieDTO> movieDto = await _movieLogic.recommendedMoviesByUserId(userId);
            if (movieDto == null)
            {
                return StatusCode(404);
            }

            StatusCode(200);
            return movieDto;
        }
    }
}