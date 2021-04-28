using System;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repository;
using Xunit;
using Repository.Models;
using Model;
using Logic;
using CinemaAPI.Controllers;
using System.Linq;
using System.Collections.Generic;

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

                IMovieLogic movieLogic = new MovieLogic(repoLogic);
                MovieController movieController = new MovieController(movieLogic);
                // Test GetMovie()
                outputMovie = (await movieController.GetMovie(inputMovie.ImdbId)).Value;
            }

            Assert.Equal(inputMovie, outputMovie);
        }
        
        [Fact]
        public async Task PatchMovieTest()
        {
            MovieDTO inputMovie = TestingHelper.GetRandomMovie();
            Movie outputMovie;

            // Seed the test database
            using(var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                TestingHelper.AddMovieDTOToDatabase(context, inputMovie);
                
                RepoLogic repoLogic = new RepoLogic(context);
                IMovieLogic movieLogic = new MovieLogic(repoLogic);
                MovieController movieController = new MovieController(movieLogic);
                // Test AppendMovie()
                await movieController.AppendMovie(inputMovie.ImdbId, TestingHelper.GetRandomMovie());
            }

            using(var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                outputMovie = context.Movies.FirstOrDefault(m => m.ImdbId == inputMovie.ImdbId);
            }

            Assert.NotEqual(inputMovie.Title, outputMovie.Title);
        }
        
        [Fact]
        public void DeleteMovieTest()
        {
            MovieDTO inputMovie = TestingHelper.GetRandomMovie();
            Movie outputMovie;

            using(var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                TestingHelper.AddMovieDTOToDatabase(context, inputMovie);

                RepoLogic repoLogic = new RepoLogic(context);
                IMovieLogic movieLogic = new MovieLogic(repoLogic);
                MovieController movieController = new MovieController(movieLogic);
                // Test DeleteMovie()
                movieController.DeleteMovie(inputMovie.ImdbId);
            }

            using(var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                outputMovie = context.Movies.FirstOrDefault(m => m.ImdbId == inputMovie.ImdbId);
            }

            Assert.Null(outputMovie);
        }
        
        [Fact]
        public void PostMovieTest()
        {
            MovieDTO inputMovie = TestingHelper.GetRandomMovie();
            Movie outputMovie;

            // Seed the test database
            using(var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                TestingHelper.AddMovieDTOToDatabase(context, inputMovie);
                
                RepoLogic repoLogic = new RepoLogic(context);
                IMovieLogic movieLogic = new MovieLogic(repoLogic);
                MovieController movieController = new MovieController(movieLogic);
                // Test CreateMovie()
                movieController.CreateMovie(inputMovie);
            }

            using(var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                outputMovie = context.Movies.FirstOrDefault(m => m.ImdbId == inputMovie.ImdbId);
            }

            Assert.Equal(inputMovie.Title, outputMovie.Title);
        }

        [Fact]
        public void PutMovieTest()
        {
            MovieDTO inputMovie = TestingHelper.GetRandomMovie();
            MovieDTO updatedMovie = TestingHelper.GetRandomMovie();
            updatedMovie.ImdbId = inputMovie.ImdbId;
            Movie outputMovie;

            // Seed the test database
            using(var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                TestingHelper.AddMovieDTOToDatabase(context, inputMovie);
                
                RepoLogic repoLogic = new RepoLogic(context);
                IMovieLogic movieLogic = new MovieLogic(repoLogic);
                MovieController movieController = new MovieController(movieLogic);
                // Test UpdateMovie()
                movieController.UpdateMovie(inputMovie.ImdbId, updatedMovie);
            }

            using(var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                outputMovie = context.Movies.FirstOrDefault(m => m.ImdbId == inputMovie.ImdbId);
            }

            Assert.NotEqual(inputMovie.Title, outputMovie.Title);
        }

        [Fact]
        public void SearchTest()
        {
            MovieDTO inputMovie = TestingHelper.GetRandomMovie(1, 1, 1, 1, 1);
            List<string> searchResults;

            Dictionary<string, string> filters = new Dictionary<string, string>();
            filters.Add("Actor", inputMovie.MovieActors[0]);

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
                IMovieLogic movieLogic = new MovieLogic(repoLogic);
                MovieController movieController = new MovieController(movieLogic);
                // Test SearchMovies()
                searchResults = movieController.SearchMovies(filters).Value;
            }

            Assert.Equal(inputMovie.ImdbId, searchResults[0]);
        }
    }
}