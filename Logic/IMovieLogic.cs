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
        Task<MovieDTO> GetMovie(string movieid);

        /// <summary>
        /// Returns a movieId for each movie that matches all of the filters
        /// in the argument. Filters are passed as key:value pairs, where
        /// the key is the category (Tag, Actor, Director, etc.).
        /// </summary>
        /// <param name="filters"></param>
        /// <returns></returns>
        List<string> SearchMovies(Dictionary<string, string> filters);

        /// <summary>
        /// Updates the fields of the Movie with a matching movieId to the
        /// values provided in the MovieDTO object. Creates the movie if it
        /// does not exist. Missing properties are set to null. The array
        /// properties in the MovieDTO will replace the existing lists. If
        /// the movie does not yet exist, the movie is first added via the
        /// public movie API.
        /// </summary>
        /// <param name="movieDTO"></param>
        /// <returns></returns>
        bool CreateOrUpdateMovie(MovieDTO movieDTO);

        /// <summary>
        /// Updates the fields of the Movie with a matching movieId to the
        /// values provided in the MovieDTO object. If any of the passed-in values
        /// are null/empty, they will remain unchanged. The passed-in array
        /// fields will be appened to the existing lists. Pre-existing entries
        /// in the lists will remain. If the movie does not yet exist, the
        /// movie is first added via the public movie API.
        /// </summary>
        /// <param name="movieDTO"></param>
        /// <returns></returns>
        Task<bool> AppendMovie(MovieDTO movieDTO);

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
        bool SetTagBanStatus(string tagname, bool isBan);

        /// <summary>
        /// Deletes the Movie from the database. Also deletes all associated
        /// entries in: Movie_Actor, Movie_Director, Movie_Genre, Movie_Language,
        /// Movie_Tag, Movie_Tag_User.
        /// Returns true if successful.
        /// </summary>
        /// <param name="movieId"></param>
        /// <returns></returns>
        bool DeleteMovie(string movieId);
    }
}
