using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Model;
using Repository.Models;

namespace Logic
{
    public interface IMovieLogic
    {
        /// <summary>
        /// Returns detailed information for the specified movieid. Returns
        /// null if the specified movieId does not exist.
        /// </summary>
        /// <param name="movieid"></param>
        /// <returns></returns>
        Task<MovieDTO> GetMovie(string movieId);

        /// <summary>
        /// Returns a movieId for each movie that matches all of the filters
        /// in the argument. Filters are passed as keys with arrays of values
        /// where the key is the category (Tag, Actor, Director, etc.).
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        List<string> SearchMovies(Dictionary<string, string[]> filters);

        /// <summary>
        /// Updates the fields of the Movie with a matching movieId to the
        /// values provided in the MovieDTO object. Creates the movie if it
        /// does not exist. Missing properties are set to null. The array
        /// properties in the MovieDTO will replace the existing lists. If
        /// the movie does not yet exist, the movie is first added via the
        /// public movie API. Returns true if successful.
        /// </summary>
        /// <param name="movieDTO"></param>
        /// <returns></returns>
        Task<bool> UpdateMovie(string movieId, MovieDTO movieDTO);

        /// <summary>
        /// Updates the fields of the Movie with a matching movieId to the
        /// values provided in the MovieDTO object. If any of the passed-in values
        /// are null/empty, they will remain unchanged. The passed-in array
        /// fields will be appened to the existing lists. Pre-existing entries
        /// in the lists will remain. If the movie does not yet exist, the
        /// movie is first added via the public movie API. Returns true
        /// if successful.
        /// </summary>
        /// <param name="movieDTO"></param>
        /// <returns></returns>
        Task<bool> AppendMovie(string movieId, MovieDTO movieDTO);

        /// <summary>
        /// Creates a new Movie entry from the information within the MovieDTO
        /// argument. Returns true if successful.
        /// </summary>
        /// <param name="movieDTO"></param>
        /// <returns></returns>
        Task<bool> CreateMovie(MovieDTO movieDTO);

        /// <summary>
        /// Adds a User's Tag Vote for a Movie to the database.
        /// Creates the Movie if it does not exist, based on the
        /// information from the public movie API.
        /// Adds the Tag to the database if it does not exist.
        /// Adds the MovieTag to the database if it does not
        /// exist.
        /// Returns true if successful; false otherwise.
        /// </summary>
        /// <param name="taggingDTO"></param>
        /// <returns></returns>
        Task<bool> TagMovie(TaggingDTO taggingDTO);

        /// <summary>
        /// Marks a Tag's banned state to true or false, as determined by the
        /// isBan argument. Returns true if successful.
        /// </summary>
        /// <param name="tagname"></param>
        /// <param name="IsBan"></param>
        /// <returns></returns>
        bool SetTagBanStatus(string tagName, bool isBan);

        /// <summary>
        /// Deletes the Movie from the database. Also deletes all associated
        /// entries in: Movie_Actor, Movie_Director, Movie_Genre, Movie_Language,
        /// Movie_Tag, Movie_Tag_User.
        /// Returns true if successful.
        /// </summary>
        /// <param name="movieId"></param>
        /// <returns></returns>
        bool DeleteMovie(string movieId);

        /// <summary>
        /// Adds the Movie to the User's following list.
        /// </summary>
        /// <param name="movieId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool FollowMovie(string movieId, string userId);

        /// <summary>
        /// Removes the Movie to the User's following list.
        /// </summary>
        /// <param name="movieId"></param>
        /// <param name="userId"></param>
        /// <returns></returns>
        bool UnfollowMovie(string movieId, string userId);

        /// <summary>
        /// Returns all movies that the user is following.
        /// </summary>
        /// <param name="userId"></param>
        /// <returns></returns>
        List<string> GetFollowingMovies(string userId);

        /// <summary>
        /// Returns a list containing the names of all tags that
        /// are not banned.
        /// </summary>
        /// <returns></returns>
        List<string> GetAllTags();
        /// <summary>
        /// return a list a movies depending on particular Movie Id 
        /// </summary>
        /// <param name="imdbId"></param>
        /// <returns></returns>
        Task<List<MovieDTO>> recommendedMovies(string imdbId);

        /// <summary>
        /// return a list a movies depending on particular User Id
        /// </summary>
        /// <param name="imdbId"></param>
        /// <returns></returns>
        Task<List<MovieDTO>> recommendedMoviesByUserId(string userId);

        /// <summary>
        /// Takes in a review with an empty follower list.
        /// Gets the follower list from the repo.
        /// Adds follower list to review.
        /// </summary>
        /// <param name="review"></param>
        /// <returns>ReviewNotification</returns>
        ReviewNotification GetFollowersForReviewNotification(ReviewNotification review);

        /// <summary>
        /// Takes in a discussion notification with it's existing follower list.
        /// Gets the follower list from the repo for the movie noted in the discussion.
        /// Adds movie follower list to existing list.
        /// </summary>
        /// <param name="forumNote"></param>
        /// <returns>ForumNotification</returns>
        ForumNotification GetFollowersForForumNotification(ForumNotification forumNote);
    }
}
