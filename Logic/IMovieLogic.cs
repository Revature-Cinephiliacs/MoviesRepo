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
        Task<MovieDTO> GetMovie(string movieid);
        List<string> SearchMovies(Dictionary<string, string> filters);
        bool UpdateMovie(MovieDTO movieDTO);
        Task<bool> AppendMovie(MovieDTO movieDTO);
        Task<bool> TagMovie(TaggingDTO taggingDTO);
        bool BanTag(string tagname);
        bool DeleteMovie(string movieId);
    }
}
