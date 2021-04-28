using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Repository;
using Repository.Models;
using Xunit;

namespace Tests
{
    public class RepoLogicTestLight
    {
        readonly DbContextOptions<Cinephiliacs_MovieContext> dbOptions =
            new DbContextOptionsBuilder<Cinephiliacs_MovieContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString()).Options;
        [Fact]
        public void TestAddMovieTagUser()
        {
            var sut1 = new MovieTagUser() {ImdbId = "Anis", TagName = "Bad",UserId = "12345",IsUpvote = true};
            
            MovieTagUser result1;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                msr.AddMovieTagUser(sut1);
                result1 = context2.MovieTagUsers.FirstOrDefault(ma => ma.ImdbId == "Anis");
            }
            Assert.Equal(result1.ImdbId,"Anis");
        }

        [Fact]
        public void TestAddMovieActor()
        {
            var sut = new MovieActor() 
                  { ImdbId = "12345",
                    ActorId = Guid.NewGuid(),
                    Actor = new Actor(){ActorId = Guid.NewGuid(), ActorName = "Anis"},
                    Imdb = new Movie(){ImdbId = "12345",Title = "Titanic"}

                   };
            
            bool result;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result = msr.AddMovieActor(sut.Imdb.ImdbId, sut.Actor.ActorName);
            }
            Assert.False(result);
        }
        [Fact]
        public void TestAddMovieDirector()
        {
            var sut = new MovieDirector() 
            { ImdbId = "12345",
                DirectorId = Guid.NewGuid(),
                Director = new Director(){DirectorId = Guid.NewGuid(), DirectorName = "Anis"},
                Imdb = new Movie(){ImdbId = "12345",Title = "Titanic"}

            };
            
            bool result;
            using (var context2 = new Cinephiliacs_MovieContext(dbOptions))
            {
                context2.Database.EnsureCreated();
                var msr = new RepoLogic(context2);
                result = msr.AddMovieActor(sut.Imdb.ImdbId, sut.Director.DirectorName);
            }
            Assert.False(result);
        }
    }
}
