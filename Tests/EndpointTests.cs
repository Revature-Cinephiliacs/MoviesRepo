using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repository;
using Xunit;
using Repository.Models;
using Model;
using Logic;
using CinemaAPI.Controllers;

namespace Tests
{
    public class EndpointTests
    {
        readonly DbContextOptions<Cinephiliacs_MovieContext> dbOptions =
            new DbContextOptionsBuilder<Cinephiliacs_MovieContext>()
            .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

        [Fact]
        public async Task GetMovieTest()
        {
            MovieDTO inputMovie = TestingHelper.GetRandomMovie();
            MovieDTO outputMovie;

            // Seed the test database
            using(var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                TestingHelper.AddMovieDTOToDatabase(context, inputMovie);
            }

            using(var context = new Cinephiliacs_MovieContext(dbOptions))
            {                
                RepoLogic repoLogic = new RepoLogic(context);

                // Test GetMovie()
                IMovieLogic movieLogic = new MovieLogic(repoLogic);
                MovieController movieController = new MovieController(movieLogic);
                outputMovie = (await movieController.GetMovie(inputMovie.ImdbId)).Value;
            }

            Assert.Equal(inputMovie, outputMovie);
        }
    }
}