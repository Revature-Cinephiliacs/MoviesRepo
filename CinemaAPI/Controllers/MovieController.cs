using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Logic;
using Logic.ApiHelper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using System.Net.Http;
using Newtonsoft.Json.Linq;
using System.Net.Http.Headers;
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
        public ActionResult CreateMovie([FromBody] MovieDTO movieDTO)
        {
            if(!ModelState.IsValid)
            {
                return StatusCode(400);
            }
            if(_movieLogic.CreateMovie(movieDTO))
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
        public ActionResult UpdateMovie(string movieId, [FromBody] MovieDTO movieDTO)
        {
            if(!ModelState.IsValid)
            {
                return StatusCode(400);
            }
            if(_movieLogic.UpdateMovie(movieId, movieDTO))
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
            List<string> tagNames = _movieLogic.GetAllTags();
            if(tagNames == null)
            {
                return StatusCode(400);
            }
            StatusCode(200);
            return tagNames;
        }

        /// <summary>
        /// Bans the specified tag. This is only available to Moderators
        /// and Administrators.
        /// </summary>
        /// <param name="tagName"></param>
        /// <returns></returns>
        [HttpPut("tag/ban/{tagName}")]
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
        [HttpPut("follow/{movieId}/{userId}")]
        public ActionResult FollowMovie(string movieId, string userId)
        {
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
        [HttpDelete("follow/{movieId}/{userId}")]
        public ActionResult UnfollowMovie(string movieId, string userId)
        {
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
        [HttpGet("follow/{userId}")]
        public ActionResult<List<string>> GetFollowingMovies(string userId)
        {
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
            ReviewNotification review = new ReviewNotification();
            review = reviewNotification;
            review = _movieLogic.GetFollowersForReviewNotification(review);
            if(review.Followers != null){
                SendReviewNotification(review);
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
            ForumNotification forumNote = new ForumNotification();
            forumNote = forumNotification;
            forumNote = _movieLogic.GetFollowersForForumNotification(forumNote);
            if(forumNote.Followers != null){
                SendForumNotification(forumNote);
                return StatusCode(200);
            }else{
                return StatusCode(404);
            }
        }

        /// <summary>
        /// Sends the review notification on to Users, with the list of users who follow the movie the new movie review belongs to.
        /// </summary>
        /// <param name="reviewNotification"></param
        /// <returns></returns>
        public async Task<bool> SendReviewNotification(ReviewNotification reviewNotification)
        {
            HttpClient client = new HttpClient();
            string path = "http://20.45.2.119/user/notification/review";
            HttpResponseMessage response = await client.PostAsJsonAsync(path, reviewNotification);
            if(response.IsSuccessStatusCode)
            {   
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Sends the forum notification on to Users with the list of users who follow the movie the new forum topic belongs to.
        /// </summary>
        /// <param name="forumNotification"></param>
        /// <returns></returns>
        public async Task<bool> SendForumNotification(ForumNotification forumNotification)
        {
            HttpClient client = new HttpClient();
            string path = "http://20.45.2.119/user/notification/discussion";
            HttpResponseMessage response = await client.PostAsJsonAsync(path, forumNotification);
            if(response.IsSuccessStatusCode)
            {   
                return true;
            }
            else
            {
                return false;
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
        [HttpGet("recommendedByUserId/{userId}")]
        public async Task<ActionResult<List<MovieDTO>>> getRecommendedById(string userId)
        {
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