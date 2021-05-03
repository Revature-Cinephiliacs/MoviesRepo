using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic;
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
    }
}
