using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Logic;
using Microsoft.AspNetCore.Mvc;
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

        [HttpPost("{movieid}")]
        public async Task<ActionResult> CreateMovie(Guid movieid)
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

    }
}
