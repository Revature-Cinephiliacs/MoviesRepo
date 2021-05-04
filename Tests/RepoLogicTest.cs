using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic;
using Model;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.Models;
using Xunit;

namespace Tests
{
    public class RepoLogicTest
    {
        readonly DbContextOptions<Cinephiliacs_MovieContext> dbOptions =
            new DbContextOptionsBuilder<Cinephiliacs_MovieContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;

       

        [Fact]
        public void TestAddMovie()
        {
            var sut = new Movie() { ImdbId = "Something",Title = "Anis",};
            Movie result2;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                msr.AddMovie(sut);
                context2.SaveChanges();
                result2 = context2.Movies.FirstOrDefault(r => r.ImdbId == sut.ImdbId);
            }
            Assert.Equal(result2.Title, sut.Title);
        }
        
        [Fact]
        public void TestAddRating()
        {
            var sut = new Rating() { RatingId = 43,RatingName = "Anis",};

            Rating result;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                msr.AddRating(sut);
                result = context2.Ratings.FirstOrDefault(r => r.RatingId == sut.RatingId);


            }
            Assert.Equal(result.RatingName,sut.RatingName);
        }

        [Fact]
        public void TestupdateMovie()
        {
            var movie = new Movie() { ImdbId = "Anis", Title = "Something" };
            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Movies.Add(movie);
                context1.SaveChanges();
            }

            Movie result;
            bool result2;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                
                var msr = new RepoLogic(context2);
                result =  msr.GetMovie(movie.ImdbId);
                result.Title = "Brad";
                result2 =  msr.UpdateMovie(result);
            }
            Assert.True(result2);
        }
        [Fact]
        public void TestupdateMovieBadPath()
        {
            
            Movie result = new Movie();
            bool result2;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                
                var msr = new RepoLogic(context2);
                result2 =  msr.UpdateMovie(result);
            }
            Assert.False(result2);
        }
        [Fact]
        public void TestupdateTag()
        {
            var tag = new Tag() { TagName = "Anis", IsBanned = false };
            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Tags.Add(tag);
                context1.SaveChanges();

            }

            Tag result;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                
                var msr = new RepoLogic(context2);
                msr.UpdateTag(tag);
                result = context2.Tags.FirstOrDefault(t => t.TagName == tag.TagName);
            }
            Assert.Equal(tag,result);
        }
        [Fact]
        public void TestGetAllMovies()
        {
            var sut1 = new Movie() { ImdbId = "Anis", Title = "Something" };
            var sut2 = new Movie() { ImdbId = "Anis3", Title = "Something" };
            var sut3 = new Movie() { ImdbId = "Anis4", Title = "Something" };
            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Movies.Add(sut1);
                context1.Movies.Add(sut2);
                context1.Movies.Add(sut3);
                context1.SaveChanges();
            }

            List<Movie> result;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                
                var msr = new RepoLogic(context2);
                result = msr.GetAllMovies();
                
            }
            Assert.Equal(3,result.Count);
        }

        [Fact]
        public void TestMovie()
        {
            var sut1 = new Movie() {ImdbId = "Anis", Title = "Cool"};

            Movie result1;
            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Movies.Add(sut1);
                context1.SaveChanges();
                result1 = context1.Movies.Find(sut1.ImdbId);

            }

            Movie result2;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                
                var msr = new RepoLogic(context2);
                result2 = msr.GetMovie(sut1.ImdbId);

            }
            Assert.Equal(result1.Title,result2.Title);
        }
        [Fact]
        public void TestRating()
        {
            var sut1 = new Rating() {RatingId = 45, RatingName = "Cool"};

            Rating result1;
            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Ratings.Add(sut1);
                context1.SaveChanges();
                result1 = context1.Ratings.FirstOrDefault(r => r.RatingName == "Cool");

            }

            Rating result2;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                
                var msr = new RepoLogic(context2);
                result2 = msr.GetRating("Cool");

            }
            Assert.Equal(result1.RatingName,result2.RatingName);
        }
        [Fact]
        public void TestTag()
        {
            var sut1 = new Tag() {TagName = "Anis", IsBanned = false};

            Tag result1;
            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Tags.Add(sut1);
                context1.SaveChanges();
                result1 = context1.Tags.FirstOrDefault(r => r.TagName == "Anis");

            }

            Tag result2;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                
                var msr = new RepoLogic(context2);
                result2 = msr.GetTag("Anis");

            }
            Assert.Equal(result1.TagName,result2.TagName);
        }
        [Fact]
        public void  TestClearMovieActor()
        {
            var sut1 = new MovieActor() {ImdbId = "Anis", ActorId = Guid.NewGuid()};
            var sut2 = new MovieActor() {ImdbId = "Anis", ActorId = Guid.NewGuid()};
            var sut3 = new MovieActor() {ImdbId = "Anis", ActorId = Guid.NewGuid()};

            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.MovieActors.Add(sut1);
                context1.MovieActors.Add(sut2);
                context1.MovieActors.Add(sut3);
                context1.SaveChanges();

            }
            List<MovieActor> result2;   
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                context2.Database.EnsureDeleted();
                var msr = new RepoLogic(context2);
                msr.ClearMovieActors("Anis"); 
                result2 = context2.MovieActors.Where(r => r.ImdbId == "Anis").ToList();
            }
            Assert.Empty(result2);
        }
        [Fact]
        public void TestClearMovieDirector()
        {
            var sut1 = new MovieDirector() {ImdbId = "Anis", DirectorId = Guid.NewGuid()};
            var sut2 = new MovieDirector() {ImdbId = "Anis", DirectorId = Guid.NewGuid()};
            var sut3 = new MovieDirector() {ImdbId = "Anis", DirectorId = Guid.NewGuid()};

            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.MovieDirectors.Add(sut1);
                context1.MovieDirectors.Add(sut2);
                context1.MovieDirectors.Add(sut3);
                context1.SaveChanges();

            }
            List<MovieDirector> result2;   
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                context2.Database.EnsureDeleted();
                var msr = new RepoLogic(context2);
                msr.ClearMovieDirectors(sut1.ImdbId);
                result2 = context2.MovieDirectors.Where(r => r.ImdbId == "Anis").ToList();
                
            }
            Assert.Empty(result2);
        }
        [Fact]
        public void  TestClearMovieGenre()
        {
            var sut1 = new MovieGenre() {ImdbId = "Anis", GenreId = Guid.NewGuid()};
            var sut2 = new MovieGenre() {ImdbId = "Anis", GenreId = Guid.NewGuid()};
            var sut3 = new MovieGenre() {ImdbId = "Anis3", GenreId = Guid.NewGuid()};

            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.MovieGenres.Add(sut1);
                context1.MovieGenres.Add(sut2);
                context1.MovieGenres.Add(sut3);
                context1.SaveChanges();

            }
            List<MovieDirector> result2;   
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                msr.ClearMovieGenres("Anis");
                result2 = context2.MovieDirectors.Where(r => r.ImdbId == "Anis").ToList();
                
            }
            Assert.Empty(result2);
        }
        [Fact]
        public void TestClearMovieLanguage()
        {
            var sut1 = new MovieLanguage() {ImdbId = "Anis", LanguageId = Guid.NewGuid()};
            var sut2 = new MovieLanguage() {ImdbId = "Anis", LanguageId = Guid.NewGuid()};
            var sut3 = new MovieLanguage() {ImdbId = "Anis", LanguageId = Guid.NewGuid()};

            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.MovieLanguages.Add(sut1);
                context1.MovieLanguages.Add(sut2);
                context1.MovieLanguages.Add(sut3);
                context1.SaveChanges();

            }
            List<MovieDirector> result2;   
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
               
                var msr = new RepoLogic(context2);
                msr.ClearMovieLanguages("Anis");
                result2 = context2.MovieDirectors.Where(r => r.ImdbId == "Anis").ToList();
                
            }
            Assert.Empty(result2);
        }
        [Fact]
        public void TestDeleteMovie()
        {
            var sut1 = new Movie() {ImdbId = "Anis", Title = "Rombo"};
          

            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Movies.Add(sut1);
                context1.SaveChanges();

            }

            List<Movie> movies;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                 msr.DeleteMovie(sut1.ImdbId);
                 movies = context2.Movies.ToList();


            }
            Assert.Empty(movies);
        }
        [Fact]
        public void  TestMovieExist()
        {
            var sut1 = new Movie() {ImdbId = "Anis", Title = "Rombo"};
          

            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Movies.Add(sut1);
                context1.SaveChanges();

            }

            bool result1; 
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result1 = msr.MovieExists(sut1.ImdbId);
                

            }
            Assert.True(result1);
        }
        [Fact]
        public void TestMovieExistBadPath()
        {
            
            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.SaveChanges();

            }

            bool result1; 
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result1 = msr.MovieExists("Anis");
                

            }
            Assert.False(result1);
        }
        [Fact]
        public void TestTagExist()
        {
            var sut1 = new Tag() {TagName = "Anis", IsBanned = true};
          

            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Tags.Add(sut1);
                context1.SaveChanges();

            }

            bool result1; 
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result1 = msr.TagExists(sut1.TagName);
            }
            Assert.True(result1);
        }
        [Fact]
        public void TestTagExistBadPath()
        {
            bool result1; 
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result1 = msr.TagExists("Anis");
                

            }
            Assert.False(result1);
        }
        [Fact]
        public void TestRatingExist()
        {
            var sut1 = new Rating() {RatingName = "Anis", RatingId = 45};
          

            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Ratings.Add(sut1);
                context1.SaveChanges();

            }

            bool result1; 
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result1 = msr.RatingExists(sut1.RatingName);
            }
            Assert.True(result1);
        }
        [Fact]
        public void TestRatingExistBadPath()
        {
            bool result1; 
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result1 = msr.RatingExists("Anis");
                

            }
            Assert.False(result1);
        }
        [Fact]
        public void TestActorExist()
        {
            var sut1 = new Actor() {ActorId = Guid.NewGuid(), ActorName = "Anis"};
          

            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Actors.Add(sut1);
                context1.SaveChanges();

            }

            bool result1; 
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result1 = msr.ActorExists(sut1.ActorName);
            }
            Assert.True(result1);
        }
        [Fact]
        public void TestActorExistBadPath()
        {
            bool result1; 
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result1 = msr.ActorExists("Anis");
                

            }
            Assert.False(result1);
        }
        [Fact]
        public void TestLanguageExist()
        {
            var sut1 = new Language() {LanguageId = Guid.NewGuid(), LanguageName = "French"};
          

            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Languages.Add(sut1);
                context1.SaveChanges();

            }

            bool result1; 
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result1 = msr.LanguageExists(sut1.LanguageName);
            }
            Assert.True(result1);
        }
        [Fact]
        public void TestLanguageExistBadPath()
        {
            bool result1; 
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result1 = msr.LanguageExists("Anis");
                

            }
            Assert.False(result1);
        }
        [Fact]
        public void TestDirectorExist()
        {
            var sut1 = new Director() {DirectorName = "Anis", DirectorId = Guid.NewGuid()};
          

            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Directors.Add(sut1);
                context1.SaveChanges();

            }

            bool result1; 
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result1 = msr.DirectorExists(sut1.DirectorName);
            }
            Assert.True(result1);
        }
        [Fact]
        public void TestDirectorExistBadPath()
        {
            bool result1; 
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result1 = msr.DirectorExists("Anis");
                

            }
            Assert.False(result1);
        }
        [Fact]
        public void TestGenreExist()
        {
            var sut1 = new Genre() {GenreName = "Anis", GenreId = Guid.NewGuid()};
          

            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Genres.Add(sut1);
                context1.SaveChanges();

            }

            bool result1; 
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result1 = msr.GenreExists(sut1.GenreName);
            }
            Assert.True(result1);
        }
        [Fact]
        public void TestGenreExistBadPath()
        {
            bool result1; 
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result1 = msr.GenreExists("Anis");
                

            }
            Assert.False(result1);
        }
        [Fact]
        public void TestMovieGenreExist()
        {
            var sut1 = new MovieGenre() {ImdbId = "Anis", GenreId = Guid.NewGuid(),Genre = new Genre(){GenreId = Guid.NewGuid(),GenreName = "Romance"}};
                

            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.MovieGenres.Add(sut1);
                context1.SaveChanges();

            }

            bool result1; 
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result1 = msr.MovieGenreExists(sut1.ImdbId,sut1.Genre.GenreName);
            }
            Assert.True(result1);
        }
        [Fact]
        public void TestMovieGenreExistBadPath()
        {
            var sut1 = new MovieGenre() {ImdbId = "Anis", GenreId = Guid.NewGuid()};
                

            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.MovieGenres.Add(sut1);
                context1.SaveChanges();

            }
            bool result1; 
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result1 = msr.MovieGenreExists("Anis","Romance");
                
            }
            Assert.False(result1);
        }
        [Fact]
        public void TestMovieLanguageExist()
        {
            var sut1 = new MovieLanguage() {ImdbId = "Anis", LanguageId = Guid.NewGuid(),Language = new Language(){LanguageId = Guid.NewGuid(),LanguageName = "french"}};
                

            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.MovieLanguages.Add(sut1);
                context1.SaveChanges();

            }

            bool result1; 
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result1 = msr.MovieLanguageExists(sut1.ImdbId,sut1.Language.LanguageName);
            }
            Assert.True(result1);
        }
        [Fact]
        public void TestMovieLanguageExistBadPath()
        {
            var sut1 = new MovieLanguage() {ImdbId = "Anis", LanguageId = Guid.NewGuid()};
                

            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.MovieLanguages.Add(sut1);
                context1.SaveChanges();

            }
            bool result1; 
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result1 = msr.MovieLanguageExists(sut1.ImdbId,"Romance");
                
            }
            Assert.False(result1);
        }
        [Fact]
        public void TestMovieTagUserExist()
        {
            var sut1 = new MovieTagUser() {ImdbId = "Anis", TagName = "Bad",UserId = "12345"};
                

            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.MovieTagUsers.Add(sut1);
                context1.SaveChanges();

            }

            bool result1; 
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result1 = msr.MovieTagUserExists(sut1);
            }
            Assert.True(result1);
        }
        [Fact]
        public void TestMovieActorExist()
        {
            var sut1 = new MovieActor() {ImdbId = "Anis",ActorId = Guid.NewGuid(),Actor = new Actor(){ActorId = Guid.NewGuid(),ActorName = "Anis2"}};
                

            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.MovieActors.Add(sut1);
                context1.SaveChanges();

            }

            bool result1; 
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result1 = msr.MovieActorExists(sut1.ImdbId,sut1.Actor.ActorName);
            }
            Assert.True(result1);
        }
        [Fact]
        public void TestMovieActorExistBadPath()
        {
            var sut1 = new MovieActor() {ImdbId = "Anis", ActorId = Guid.NewGuid(),Actor = new Actor(){ActorId = Guid.NewGuid()}};
                

            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.MovieActors.Add(sut1);
                context1.SaveChanges();

            }
            bool result1; 
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result1 = msr.MovieActorExists("Anis","Medini");
                
            }
            Assert.False(result1);
        }
        [Fact]
        public void TestMovieDirectorExist()
        {
            var sut1 = new MovieDirector() {ImdbId = "Anis",DirectorId = Guid.NewGuid(),Director = new Director(){DirectorId = Guid.NewGuid(),DirectorName = "Anis2"}};
                

            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.MovieDirectors.Add(sut1);
                context1.SaveChanges();

            }

            bool result1; 
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result1 = msr.MovieDirectorExists(sut1.ImdbId,sut1.Director.DirectorName);
            }
            Assert.True(result1);
        }
        [Fact]
        public void TestMovieDirectorExistBadPath()
        {
            var sut1 = new MovieDirector() {ImdbId = "Anis", DirectorId = Guid.NewGuid(),Director = new Director(){DirectorId = Guid.NewGuid()}};
                

            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.MovieDirectors.Add(sut1);
                context1.SaveChanges();

            }
            bool result1; 
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result1 = msr.MovieDirectorExists("Anis","Medini");
                
            }
            Assert.False(result1);
        }

        [Fact]
        public void TestGetFollowersForReviewNotification()
        {
            Cinephiliacs_MovieContext movieContext= new Cinephiliacs_MovieContext(dbOptions);
            RepoLogic repoLogic = new RepoLogic(movieContext);
            MovieLogic movieLogic = new MovieLogic(repoLogic);
            repoLogic.AddFollowingMovie("Avengers", "Avengee");
            ReviewNotification reviewNotification = new ReviewNotification();
            reviewNotification.Imdbid = "Avengers";
            ReviewNotification testNote = movieLogic.GetFollowersForReviewNotification(reviewNotification);
            var expected = 1;
            var actual = testNote.Followers.Count;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestGetFollowersForForumNotification()
        {
            Cinephiliacs_MovieContext movieContext= new Cinephiliacs_MovieContext(dbOptions);
            RepoLogic repoLogic = new RepoLogic(movieContext);
            MovieLogic movieLogic = new MovieLogic(repoLogic);
            repoLogic.AddFollowingMovie("Avengers", "Avengee");
            ForumNotification forumNotification = new ForumNotification();
            forumNotification.Imdbid = "Avengers";
            ForumNotification testNote = movieLogic.GetFollowersForForumNotification(forumNotification);
            var expected = 1;
            var actual = testNote.Followers.Count;
            Assert.Equal(expected, actual);
        }

        [Fact]
        public void TestAddMovieDirector()
        {
            var dir = new Director() {DirectorId = Guid.NewGuid(), DirectorName = "Anis"};
            var movie = new Movie() {ImdbId = "12345", Title = "Avenger"};
            var movieDir = new MovieDirector()
            {
                Director = dir,
                DirectorId = Guid.NewGuid(), ImdbId = "12345",
                Imdb = movie
            };
           
           
            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Movies.Add(movie);
                context1.Directors.Add(dir);
                context1.SaveChanges();
              
            }
            bool result;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result =  msr.AddMovieDirector(movieDir.ImdbId,movieDir.Director.DirectorName);
            }
            Assert.True(result);
        }
        [Fact]
        public void TestAddMovieDirectorNoMovie()
        {
            var dir = new Director() {DirectorId = Guid.NewGuid(), DirectorName = "Anis"};
            var movieDir = new MovieDirector()
            {
                Director = dir,
                DirectorId = Guid.NewGuid(), ImdbId = "12345",
                
            };
           
           
            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Directors.Add(dir);
                context1.SaveChanges();
              
            }
            bool result;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result =  msr.AddMovieDirector(movieDir.ImdbId,movieDir.Director.DirectorName);
            }
            Assert.False(result);
        }
        [Fact]
        public void TestAddMovieDirectorNoDirector()
        {
            var movie = new Movie() {ImdbId = "12345",Title = "Avenger"};
            var movieDir = new MovieDirector()
            {
               Imdb = movie,
               Director = new Director(){DirectorId = Guid.NewGuid(),DirectorName = "Anis"}
            };
           
           
            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Movies.Add(movie);
                context1.SaveChanges();
              
            }
            bool result;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result =  msr.AddMovieDirector(movieDir.ImdbId,movieDir.Director.DirectorName);
            }
            Assert.False(result);
        }
        [Fact]
        public void TestAddMovieLanguage()
        {
            var lang = new Language() {LanguageId = Guid.NewGuid(), LanguageName = "French"};
            var movie = new Movie() {ImdbId = "12345", Title = "Avenger"};
           
            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Movies.Add(movie);
                context1.Languages.Add(lang);
                context1.SaveChanges();
              
            }
            bool result;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result =  msr.AddMovieLanguage("12345","French");
            }
            Assert.True(result);
        }
        [Fact]
        public void TestAddMovieLanguageNoMovie()
        {
            var lang = new Language() {LanguageId = Guid.NewGuid(), LanguageName = "French"};
            
            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Languages.Add(lang);
                context1.SaveChanges();
              
            }
            bool result;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result =  msr.AddMovieLanguage("12345","French");
            }
            Assert.False(result);
        }
        [Fact]
        public void TestAddMovieLanguageNoLanguage()
        {
            var movie = new Movie() {ImdbId = "12345", Title = "Avenger"};
            
            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Movies.Add(movie);
                context1.SaveChanges();
              
            }
            bool result;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result =  msr.AddMovieLanguage("12345","French");
            }
            Assert.True(result);
        }
        [Fact]
        public void TestAddWord()
        {
            var sut = new Word() {BaseWord = "Friend", IsTag = true, Word1 = "Friend2"};
            
            bool result;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result =  msr.AddWord(sut);
            }
            Assert.True(result);
        }

        [Fact]
        public void TestListOfTag()
        {
            var tags = new List<Tag>();
            var tag1 = new Tag() {IsBanned = true, TagName = "Action"};
            var tag2 = new Tag() {IsBanned = true, TagName = "Sc-Fi"};
            var tag3 = new Tag() {IsBanned = true, TagName = "Drama"};

            tags.Add(tag1);
            tags.Add(tag2);
            tags.Add(tag3);
            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Tags.Add(tag1);
                context1.Tags.Add(tag2);
                context1.Tags.Add(tag3);
                context1.SaveChanges();
            }

            List<Tag> result2;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result2 = msr.GetAllTags();
            }
            Assert.Equal(result2.Count,tags.Count);
        }
        [Fact]
        public void TestGetMovieFullInfo()
        {
            var sut = new Movie() {ImdbId = "12345", Title = "Titanic"};
            
            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Movies.Add(sut);
                context1.SaveChanges();
            }

            Movie result2;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result2 = msr.GetMovieFullInfo("12345");
            }
            Assert.Equal(result2.Title,sut.Title);
        }
        [Fact]
        public void TestRatingById()
        {
            var sut1 = new Rating() {RatingId = 45, RatingName = "Cool"};

            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Ratings.Add(sut1);
                context1.SaveChanges();
            }
            Rating result2;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                
                var msr = new RepoLogic(context2);
                result2 = msr.GetRating(sut1.RatingId);

            }
            Assert.Equal(sut1.RatingName,result2.RatingName);
        }
        [Fact]
        public void TestActor()
        {
            var sut1 = new Actor() {ActorId = Guid.NewGuid(), ActorName = "Cool"};

           
            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Actors.Add(sut1);
                context1.SaveChanges();
            }
            Actor result2;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                
                var msr = new RepoLogic(context2);
                result2 = msr.GetActor(sut1.ActorName);

            }
            Assert.Equal(sut1.ActorName,result2.ActorName);
        }
        [Fact]
        public void TestDirector()
        {
            var sut1 = new Director() {DirectorId = Guid.NewGuid(), DirectorName = "Cool"};

           
            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Directors.Add(sut1);
                context1.SaveChanges();
            }
            Director result2;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                
                var msr = new RepoLogic(context2);
                result2 = msr.GetDirector(sut1.DirectorName);

            }
            Assert.Equal(sut1.DirectorName,result2.DirectorName);
        }
        [Fact]
        public void TestGenre()
        {
            var sut1 = new Genre() {GenreId = Guid.NewGuid(), GenreName = "Cool"};

           
            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Genres.Add(sut1);
                context1.SaveChanges();
            }
            Genre result2;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                
                var msr = new RepoLogic(context2);
                result2 = msr.GetGenre(sut1.GenreName);

            }
            Assert.Equal(sut1.GenreName,result2.GenreName);
        }
        [Fact]
        public void TestLanguage()
        {
            var sut1 = new Language() {LanguageId = Guid.NewGuid(), LanguageName = "Cool"};

           
            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Languages.Add(sut1);
                context1.SaveChanges();
            }
            Language result2;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                
                var msr = new RepoLogic(context2);
                result2 = msr.GetLanguage(sut1.LanguageName);

            }
            Assert.Equal(sut1.LanguageName,result2.LanguageName);
        }
        [Fact]
        public void TestWord()
        {
            var sut1 = new Word() {BaseWord = "Anis", Word1 = "Cool",IsTag = true};

           
            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Words.Add(sut1);
                context1.SaveChanges();
            }
            Word result2;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                
                var msr = new RepoLogic(context2);
                result2 = msr.GetWord(sut1.Word1);

            }
            Assert.Equal(sut1.Word1,result2.Word1);
        }
        [Fact]
        public void TestGetMovieActorNames()
        {
            var actor1 = new Actor() {ActorId = Guid.NewGuid(), ActorName = "Anis"};
            var actor2 = new Actor() {ActorId = Guid.NewGuid(), ActorName = "Aniss"};
            var movie = new Movie() {ImdbId = "12345", Title = "Avenger"};
            var movie2 = new Movie() {ImdbId = "123454", Title = "Titanic"};

            var sut1 = new MovieActor() {Actor = actor1, Imdb = movie};
            var sut2 = new MovieActor() {Actor = actor2, Imdb = movie2};

           
            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Actors.Add(actor1);
                context1.Actors.Add(actor2);
                context1.Movies.Add(movie);
                context1.Movies.Add(movie2);
                context1.MovieActors.Add(sut1);
                context1.MovieActors.Add(sut2);

                context1.SaveChanges();
            }
            List<string> result2;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                
                var msr = new RepoLogic(context2);
                result2 = msr.GetMovieActorNames("12345");

            }
            Assert.Single(result2);
        }
        [Fact]
        public void TestGetMovieDirectorNames()
        {
            var dir1 = new Director() {DirectorId = Guid.NewGuid(), DirectorName = "Anis"};
            var dir2 = new Director() {DirectorId = Guid.NewGuid(), DirectorName = "Aniss"};
            var movie = new Movie() {ImdbId = "12345", Title = "Avenger"};
            var movie2 = new Movie() {ImdbId = "123454", Title = "Titanic"};

            var sut1 = new MovieDirector() {Director = dir1, Imdb = movie};
            var sut2 = new MovieDirector() {Director = dir2, Imdb = movie2};

           
            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Directors.Add(dir1);
                context1.Directors.Add(dir2);
                context1.Movies.Add(movie);
                context1.Movies.Add(movie2);
                context1.MovieDirectors.Add(sut1);
                context1.MovieDirectors.Add(sut2);

                context1.SaveChanges();
            }
            List<string> result2;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                
                var msr = new RepoLogic(context2);
                result2 = msr.GetMovieDirectorNames("12345");

            }
            Assert.Single(result2);
        }
        [Fact]
        public void TestGetMovieGenreNames()
        {
            var genre1 = new Genre() {GenreId = Guid.NewGuid(), GenreName = "Anis"};
            var genre2 = new Genre() {GenreId = Guid.NewGuid(), GenreName = "Aniss"};
            var movie = new Movie() {ImdbId = "12345", Title = "Avenger"};
            var movie2 = new Movie() {ImdbId = "123454", Title = "Titanic"};

            var sut1 = new MovieGenre() {Genre = genre1, Imdb = movie};
            var sut2 = new MovieGenre() {Genre = genre2, Imdb = movie2};

           
            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Genres.Add(genre1);
                context1.Genres.Add(genre2);
                context1.Movies.Add(movie);
                context1.Movies.Add(movie2);
                context1.MovieGenres.Add(sut1);
                context1.MovieGenres.Add(sut2);

                context1.SaveChanges();
            }
            List<string> result2;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                
                var msr = new RepoLogic(context2);
                result2 = msr.GetMovieGenreNames("12345");

            }
            Assert.Single(result2);
        }
        [Fact]
        public void TestGetMovieLanguageNames()
        {
            var language1 = new Language() {LanguageId = Guid.NewGuid(), LanguageName = "Anis"};
            var language2 = new Language() {LanguageId = Guid.NewGuid(), LanguageName = "Aniss"};
            var movie = new Movie() {ImdbId = "12345", Title = "Avenger"};
            var movie2 = new Movie() {ImdbId = "123454", Title = "Titanic"};

            var sut1 = new MovieLanguage() {Language = language1, Imdb = movie};
            var sut2 = new MovieLanguage() {Language = language2, Imdb = movie2};

           
            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.Languages.Add(language1);
                context1.Languages.Add(language2);
                context1.Movies.Add(movie);
                context1.Movies.Add(movie2);
                context1.MovieLanguages.Add(sut1);
                context1.MovieLanguages.Add(sut2);

                context1.SaveChanges();
            }
            List<string> result2;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                
                var msr = new RepoLogic(context2);
                result2 = msr.GetMovieLanguageNames("12345");

            }
            Assert.Single(result2);
        }
        [Fact]
        public void TestGetMovieTags()
        {
            var sut1 = new MovieTag() {
                Imdb = new Movie(){ImdbId = "12345",Title = "Avenger"},
                TagNameNavigation = new Tag(){TagName = "Action",IsBanned = true}

            };

            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.MovieTags.Add(sut1);
                context1.SaveChanges();
            }
            List<MovieTag> result2;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                
                var msr = new RepoLogic(context2);
                result2 = msr.GetMovieTags("12345");

            }
            Assert.Single(result2);
        }
        [Fact]
        public void TestGetFollowingMovie()
        {
            
            var followed = new FollowingMovie() {Imdb = new Movie(){ImdbId = "12345",Title = "Avenger"}, UserId = "Anis"};
            var followed2 = new FollowingMovie() {Imdb = new Movie(){ImdbId = "123456",Title = "Titanic"}, UserId = "Anis"};

            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.FollowingMovies.Add(followed);
                context1.FollowingMovies.Add(followed2);
                context1.SaveChanges();
            }
            List<string> result2;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                
                var msr = new RepoLogic(context2);
                result2 = msr.GetFollowingMovies("Anis");

            }
            Assert.Equal(2,result2.Count);
        }
        [Fact]
        public void TestGetFollowingMovieByMovieId()
        {
            
            var followed = new FollowingMovie() {Imdb = new Movie(){ImdbId = "12345",Title = "Avenger"}, UserId = "Anis"};
            var followed2 = new FollowingMovie() {Imdb = new Movie(){ImdbId = "123457",Title = "Avenger"}, UserId = "Anis"};
            
            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.FollowingMovies.Add(followed);
                context1.FollowingMovies.Add(followed2);
                context1.SaveChanges();
            }
            List<string> result2;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                
                var msr = new RepoLogic(context2);
                result2 = msr.GetFollowingMoviesByMovieID("12345");

            }
            Assert.Single(result2);
        }
        [Fact]
        public void TestGetMovieTagsByname()
        {
            List<MovieTag> mvtags = new List<MovieTag>();
            var followed = new MovieTag() {Imdb = new Movie(){ImdbId = "12345",Title = "Avenger"}, TagName = "Anis"};
            var followed2 = new MovieTag() { Imdb = new Movie() { ImdbId = "123456", Title = "Titanic" }, TagName = "Anis" };
            mvtags.Add(followed);
            mvtags.Add(followed2);

            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.MovieTags.Add(followed);
                context1.MovieTags.Add(followed2);
                context1.SaveChanges();
            }
            List<MovieTag> result2;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                
                var msr = new RepoLogic(context2);
                result2 = msr.GetMovieTagsByName("Anis");
            }
            Assert.Equal(2,result2.Count);
        }
        [Fact]
        public void TestGetMovieDirectorById()
        {
            var id  = Guid.NewGuid();
            
            var followed = new MovieDirector() {Imdb = new Movie(){ImdbId = "12345",Title = "Avenger"}, DirectorId = id};
            var followed2 = new MovieDirector() {Imdb = new Movie(){ImdbId = "123456",Title = "Titanic"}, DirectorId = id};

            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.MovieDirectors.Add(followed);
                context1.MovieDirectors.Add(followed2);
                context1.SaveChanges();
            }
            List<MovieDirector> result2;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                
                var msr = new RepoLogic(context2);
                result2 = msr.GetMovieDirectorsById(id);

            }
            Assert.Equal(2,result2.Count);
        }
        [Fact]
        public void TestGetMovieGenreById()
        {
            var id  = Guid.NewGuid();
            
            var followed = new MovieGenre() {Imdb = new Movie(){ImdbId = "12345",Title = "Avenger"}, GenreId = id};
            var followed2 = new MovieGenre() {Imdb = new Movie(){ImdbId = "123456",Title = "Titanic"}, GenreId = id};

            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.MovieGenres.Add(followed);
                context1.MovieGenres.Add(followed2);
                context1.SaveChanges();
            }
            List<MovieGenre> result2;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                
                var msr = new RepoLogic(context2);
                result2 = msr.GetMovieGenresById(id);

            }
            Assert.Equal(2,result2.Count);
        }
        [Fact]
        public void TestGetMovieLanguageById()
        {
            var id  = Guid.NewGuid();
            var followed = new MovieLanguage() {Imdb = new Movie(){ImdbId = "12345",Title = "Avenger"}, LanguageId = id};
            var followed2 = new MovieLanguage() {Imdb = new Movie(){ImdbId = "123456",Title = "Titanic"}, LanguageId = id};

            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.MovieLanguages.Add(followed);
                context1.MovieLanguages.Add(followed2);
                context1.SaveChanges();
            }
            List<MovieLanguage> result2;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                
                var msr = new RepoLogic(context2);
                result2 = msr.GetMovieLanguagesById(id);

            }
            Assert.Equal(2,result2.Count);
        }
        [Fact]
        public void TestDeleteFollowingMovie()
        {
            var id  = Guid.NewGuid().ToString();
            
            var followed = new FollowingMovie() {Imdb = new Movie(){ImdbId = "12345",Title = "Avenger"}, UserId = id};
            

            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.FollowingMovies.Add(followed);
                context1.SaveChanges();
            }
            List<FollowingMovie> result2;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                
                var msr = new RepoLogic(context2);
                msr.DeleteFollowingMovie("12345",id);
                result2 = context2.FollowingMovies.Where(fm => fm.UserId == id).ToList();

            }
            Assert.Empty(result2);
        }
        [Fact]
        public void TestFollowingMovieExist()
        {
            var followed = new FollowingMovie() {Imdb = new Movie(){ImdbId = "12345",Title = "Avenger"},UserId = "Anis"};
           

            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.FollowingMovies.Add(followed);
                context1.SaveChanges();
            }
            bool result2;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                
                var msr = new RepoLogic(context2);
                result2 = msr.FollowingMovieExists("12345", "Anis");

            }
            Assert.True(result2);
        }
        [Fact]
        public void TestFollowingMovieExistBadPath()
        {
            

            using (var context1 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context1.Database.EnsureDeleted();
                context1.Database.EnsureCreated();
                context1.SaveChanges();
            }
            bool result2;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                
                var msr = new RepoLogic(context2);
                result2 = msr.FollowingMovieExists("12345", "Anis");

            }
            Assert.False(result2);
        }
        [Fact]
        public void TestUpdateMovieTagUser()
        {
            var movieTagUser = new MovieTagUser();
            movieTagUser.ImdbId = Guid.NewGuid().ToString();
            movieTagUser.TagName = Guid.NewGuid().ToString();
            movieTagUser.IsUpvote = true;
            movieTagUser.UserId = Guid.NewGuid().ToString();

            using (var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                context.Database.EnsureDeleted();
                context.Database.EnsureCreated();

                Tag tag = new Tag();
                tag.TagName = movieTagUser.TagName;
                tag.IsBanned = false;

                context.Tags.Add(tag);
                context.MovieTagUsers.Add(movieTagUser);

                context.SaveChanges();
            }

            movieTagUser.IsUpvote = false;
            MovieTagUser result;
            
            using (var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                context.Database.EnsureCreated();
                
                var repo = new RepoLogic(context);
                repo.UpdateMovieTagUser(movieTagUser);

                result = context.MovieTagUsers.First(mtu => mtu.ImdbId == movieTagUser.ImdbId
                    && mtu.UserId == movieTagUser.UserId && mtu.TagName == movieTagUser.TagName);
            }

            Assert.False(result.IsUpvote);
        }
    }
}
