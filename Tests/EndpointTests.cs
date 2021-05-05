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
        readonly DbContextOptions<Cinephiliacs_MovieContext> dbOptions
            = TestingHelper.GetUniqueContextOptions<Cinephiliacs_MovieContext>();

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

            Assert.Equal(inputMovie.ImdbId, outputMovie.ImdbId);
        }

        [Fact]
        public async Task PatchNewMovieTest()
        {
            MovieDTO inputMovie = TestingHelper.GetRandomMovie();
            inputMovie.ImdbId = "tt4154796";
            Movie outputMovie;

            // Seed the test database
            using(var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                
                RepoLogic repoLogic = new RepoLogic(context);
                IMovieLogic movieLogic = new MovieLogic(repoLogic);
                MovieController movieController = new MovieController(movieLogic);
                // Test AppendMovie()
                await movieController.AppendMovie(inputMovie.ImdbId, inputMovie);
            }

            using(var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                outputMovie = context.Movies.FirstOrDefault(m => m.ImdbId == inputMovie.ImdbId);
            }

            Assert.Equal(inputMovie.Title, outputMovie.Title);
        }
        
        [Fact]
        public async Task PatchExistingMovieTest()
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
        public async Task PostMovieTest()
        {
            MovieDTO inputMovie = TestingHelper.GetRandomMovie();
            Movie outputMovie;

            // Seed the test database
            using(var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();
                
                RepoLogic repoLogic = new RepoLogic(context);
                IMovieLogic movieLogic = new MovieLogic(repoLogic);
                MovieController movieController = new MovieController(movieLogic);
                // Test CreateMovie()
                await movieController.CreateMovie(inputMovie);
            }

            using(var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                outputMovie = context.Movies.FirstOrDefault(m => m.ImdbId == inputMovie.ImdbId);
            }

            Assert.Equal(inputMovie.Title, outputMovie.Title);
        }
        
        [Fact]
        public async Task PostExistingMovieTest()
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
                await movieController.CreateMovie(inputMovie);
            }

            using(var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                outputMovie = context.Movies.FirstOrDefault(m => m.ImdbId == inputMovie.ImdbId);
            }

            Assert.Equal(inputMovie.Title, outputMovie.Title);
        }

        [Fact]
        public async Task PutMovieTest()
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
                await movieController.UpdateMovie(inputMovie.ImdbId, updatedMovie);
            }

            using(var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                outputMovie = context.Movies.FirstOrDefault(m => m.ImdbId == inputMovie.ImdbId);
            }

            Assert.NotEqual(inputMovie.Title, outputMovie.Title);
        }

        [Fact]
        public void SearchActorTest()
        {
            MovieDTO inputMovie = TestingHelper.GetRandomMovie(1, 1, 1, 1, 1);
            List<string> searchResults;

            var filters = new Dictionary<string, string[]>();
            filters.Add("Actor", new string[] {inputMovie.MovieActors[0]});
            Console.WriteLine(inputMovie.MovieActors[0]);

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

        [Fact]
        public void SearchDirectorTest()
        {
            MovieDTO inputMovie = TestingHelper.GetRandomMovie(1, 1, 1, 1, 1);
            List<string> searchResults;

            var filters = new Dictionary<string, string[]>();
            filters.Add("Director", new string[] {inputMovie.MovieDirectors[0]});

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

        [Fact]
        public void SearchGenreTest()
        {
            MovieDTO inputMovie = TestingHelper.GetRandomMovie(1, 1, 1, 1, 1);
            List<string> searchResults;

            var filters = new Dictionary<string, string[]>();
            filters.Add("Genre", new string[] {inputMovie.MovieGenres[0]});

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

        [Fact]
        public void SearchLanguageTest()
        {
            MovieDTO inputMovie = TestingHelper.GetRandomMovie(1, 1, 1, 1, 1);
            List<string> searchResults;

            var filters = new Dictionary<string, string[]>();
            filters.Add("Language", new string[] {inputMovie.MovieLanguages[0]});

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

        [Fact]
        public void SearchTagTest()
        {
            MovieDTO inputMovie = TestingHelper.GetRandomMovie(1, 1, 1, 1, 1);
            List<string> searchResults;

            var filters = new Dictionary<string, string[]>();
            filters.Add("Tag", new string[] {inputMovie.MovieTags[0]});

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

        [Fact]
        public void SearchAnyTest()
        {
            MovieDTO inputMovie = TestingHelper.GetRandomMovie(1, 1, 1, 1, 1);
            List<string> searchResults;

            var filters = new Dictionary<string, string[]>();
            filters.Add("Any", new string[] {inputMovie.MovieDirectors[0]});

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

        [Fact]
        public void SearchRatingTest()
        {
            MovieDTO inputMovie = TestingHelper.GetRandomMovie(0, 0, 0, 0, 0);
            List<string> searchResults;

            var filters = new Dictionary<string, string[]>();
            Console.WriteLine(inputMovie);
            filters.Add("Rating", new string[] {inputMovie.RatingName});

            // Seed the test database
            using(var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                TestingHelper.AddMovieDTOToDatabase(context, inputMovie);
                TestingHelper.AddMovieDTOToDatabase(context, TestingHelper.GetRandomMovie(0, 0, 0, 0, 0));
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

        [Fact]
        public void SearchMultiTagTest1()
        {
            MovieDTO inputMovie = TestingHelper.GetRandomMovie(0, 0, 0, 0, 2);
            List<string> searchResults;

            var filters = new Dictionary<string, string[]>();
            var tagArray = new string[] {inputMovie.MovieTags[0], inputMovie.MovieTags[1]};
            filters.Add("Tag", tagArray);

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

        [Fact]
        public void SearchActorTagTest()
        {
            MovieDTO inputMovie = TestingHelper.GetRandomMovie(1, 1, 1, 1, 1);
            MovieDTO unmatchedMovie = TestingHelper.GetRandomMovie(1, 1, 1, 1, 1);
            unmatchedMovie.MovieActors[0] = inputMovie.MovieActors[0];
            List<string> searchResults;

            var filters = new Dictionary<string, string[]>();
            filters.Add("Actor", new string[] {inputMovie.MovieActors[0]});
            filters.Add("Tag", new string[] {inputMovie.MovieTags[0]});

            // Seed the test database
            using(var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                TestingHelper.AddMovieDTOToDatabase(context, inputMovie);
                TestingHelper.AddMovieDTOToDatabase(context, unmatchedMovie);
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
        
        [Fact]
        public async Task PostTagTest()
        {
            var inputMovie = TestingHelper.GetRandomMovie();
            var inputTag = TestingHelper.GetRandomTaggingDTO(inputMovie.ImdbId);
            MovieTagUser movieTagUser;

            // Seed the test database
            using(var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                TestingHelper.AddMovieDTOToDatabase(context, inputMovie);
                
                RepoLogic repoLogic = new RepoLogic(context);
                IMovieLogic movieLogic = new MovieLogic(repoLogic);
                MovieController movieController = new MovieController(movieLogic);
                // Test TagMovie()
                await movieController.TagMovie(inputTag);
            }

            using(var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                movieTagUser = context.MovieTagUsers.FirstOrDefault(m => m.ImdbId == inputTag.MovieId);
            }

            Assert.Equal(inputTag.MovieId, movieTagUser.ImdbId);
            Assert.Equal(inputTag.TagName, movieTagUser.TagName);
            Assert.Equal(inputTag.UserId, movieTagUser.UserId);
        }
        
        [Fact]
        public void BanTagTest()
        {
            var inputMovie = TestingHelper.GetRandomMovie();
            var inputTag = TestingHelper.GetRandomTaggingDTO(inputMovie.ImdbId);
            bool inputBanState = false;
            Tag tag;

            // Seed the test database
            using(var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                TestingHelper.AddMovieDTOToDatabase(context, inputMovie);
                TestingHelper.AddTagToDatabase(context, inputTag, inputBanState);
                
                RepoLogic repoLogic = new RepoLogic(context);
                IMovieLogic movieLogic = new MovieLogic(repoLogic);
                MovieController movieController = new MovieController(movieLogic);
                // Test BanTag()
                movieController.BanTag(inputTag.TagName);
            }

            using(var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                tag = context.Tags.FirstOrDefault(t => t.TagName == inputTag.TagName);
            }

            Assert.True(tag.IsBanned);
        }
        
        [Fact]
        public void UnbanTagTest()
        {
            var inputMovie = TestingHelper.GetRandomMovie();
            var inputTag = TestingHelper.GetRandomTaggingDTO(inputMovie.ImdbId);
            bool inputBanState = true;
            Tag tag;

            // Seed the test database
            using(var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                TestingHelper.AddMovieDTOToDatabase(context, inputMovie);
                TestingHelper.AddTagToDatabase(context, inputTag, inputBanState);
                
                RepoLogic repoLogic = new RepoLogic(context);
                IMovieLogic movieLogic = new MovieLogic(repoLogic);
                MovieController movieController = new MovieController(movieLogic);
                // Test UnbanTag()
                movieController.UnbanTag(inputTag.TagName);
            }

            using(var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                tag = context.Tags.FirstOrDefault(t => t.TagName == inputTag.TagName);
            }

            Assert.False(tag.IsBanned);
        }

        [Fact]
        public void GetTagsTest()
        {
            MovieDTO inputMovie = TestingHelper.GetRandomMovie(0, 0, 0, 0, 3);
            List<string> results;

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

                // Test GetAllTags()
                results = movieController.GetAllTags().Value;
            }

            Assert.Contains(inputMovie.MovieTags[0], results);
            Assert.Contains(inputMovie.MovieTags[1], results);
            Assert.Contains(inputMovie.MovieTags[2], results);
        }

        [Fact]
        public void FollowMovieTest()
        {
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
            }

            using(var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                RepoLogic repoLogic = new RepoLogic(context);
                IMovieLogic movieLogic = new MovieLogic(repoLogic);
                MovieController movieController = new MovieController(movieLogic);
                // Test FollowMovie()
                followedMovies = movieController.GetFollowingMovies(userId).Value;
            }

            Assert.Contains(inputMovie.ImdbId, followedMovies);
        }

        [Fact]
        public async Task RecommendedMoviesByUserIdTest()
        {
            MovieDTO inputMovie = TestingHelper.GetRandomMovie();
            inputMovie.ImdbId = "tt4154796";
            string userId = Guid.NewGuid().ToString();
            List<MovieDTO> results;

            // Seed the test database
            using(var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                TestingHelper.AddMovieDTOToDatabase(context, inputMovie);

                var followingMovie = new FollowingMovie();
                followingMovie.ImdbId = inputMovie.ImdbId;
                followingMovie.UserId = userId;
                context.FollowingMovies.Add(followingMovie);
                context.SaveChanges();
            }

            using(var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                RepoLogic repoLogic = new RepoLogic(context);
                MovieLogic movieLogic = new MovieLogic(repoLogic);
                // Test RecommendedMoviesByUserId()
                results = await movieLogic.RecommendedMoviesByUserId(userId);
            }

            Assert.True(results.Count > 0);
        }
    }
}