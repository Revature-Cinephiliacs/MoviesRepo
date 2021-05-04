using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logic;
using Microsoft.EntityFrameworkCore;
using Model;
using Repository;
using Repository.Models;
using Xunit;

namespace Tests
{
    public class RepoLogicRecommendationTest
    {
        
        [Fact]
        public void TestRecommendedMoviesReturnListMovie()
        {
            //Arrange
            string movieId = "tt0178207";

            //Act

            IMovieLogic ml = new MovieLogic(null);
            var res = ml.RecommendedMovies(movieId);
            string actual = res.Status.ToString();
            string expected = "WaitingForActivation";
            //Assert
            Assert.Equal(expected, actual);
           
           
        }

        
        [Fact]
        public void TestRecommendedMoviesUnknownUserReturnError()
        {
            //Arrange
            string userId = "random_user";
            

            //Act

            IMovieLogic ml = new MovieLogic(null);
            var res = ml.RecommendedMoviesByUserId(userId);
            string actual = res.Status.ToString();
            string expected = "Faulted";
            //Assert
            Assert.Equal(expected, actual);
        }
    }
}
