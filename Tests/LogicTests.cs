using System;
using System.Threading.Tasks;
using Repository;
using Xunit;
using Repository.Models;
using Model;
using Logic;
using System.Collections.Generic;

namespace Tests
{
    public class LogicTests
    {

        [Fact]
        public void DeleteFollowedMovieTest()
        {
            var dbOptions = TestingHelper.GetUniqueContextOptions<Cinephiliacs_MovieContext>();
            MovieDTO inputMovie = TestingHelper.GetRandomMovie();
            string userId = Guid.NewGuid().ToString();
            List<string> followedMovies;

            // Seed the test database
            using(var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                TestingHelper.AddMovieDTOToDatabase(context, inputMovie);

                RepoLogic repoLogic = new RepoLogic(context);
                IMovieLogic movieLogic = new MovieLogic(repoLogic);
                // Test FollowMovie()
                movieLogic.FollowMovie(inputMovie.ImdbId, userId);
                movieLogic.UnfollowMovie(inputMovie.ImdbId, userId);
            }

            using(var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                RepoLogic repoLogic = new RepoLogic(context);
                IMovieLogic movieLogic = new MovieLogic(repoLogic);
                // Test FollowMovie()
                followedMovies = movieLogic.GetFollowingMovies(userId);
            }

            Assert.DoesNotContain(inputMovie.ImdbId, followedMovies);
        }
    }
}