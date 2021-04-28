using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using Model;
using Repository.Models;

namespace Tests
{
    internal static class TestingHelper
    {
        /// <summary>6</summary>
        const int _maxListItemCount = 6;
        static int nextMovieIdNumber = 0;
        static Random randomGen = new Random();

        /// <summary>
        /// Returns a MovieDTO object with all properties populated with
        /// random values. Values are bounded to a range where appropriate.
        /// The number of entries in each list is a random number between
        /// 0 and <inheritdoc cref="_maxListItemCount"/>
        /// </summary>
        /// <returns></returns>
        internal static MovieDTO GetRandomMovie()
        {
            return GetRandomMovie(randomGen.Next(_maxListItemCount), randomGen.Next(_maxListItemCount), 
                randomGen.Next(_maxListItemCount), randomGen.Next(_maxListItemCount), 
                randomGen.Next(_maxListItemCount));
        }

        /// <summary>
        /// Returns a MovieDTO object with all properties populated with
        /// random values. Values are bounded to a range where appropriate.
        /// The number of entries in each list is set by the arguments.
        /// </summary>
        /// <returns></returns>
        internal static MovieDTO GetRandomMovie(int numberOfActors, int numberOfDirectors,
            int numberofGenres, int numberOfLanguages, int numberOfTags)
        {
            MovieDTO movieDTO = new MovieDTO();

            movieDTO.ImdbId = Guid.NewGuid().ToString().Substring(0, 10)
                + (nextMovieIdNumber).ToString();
            nextMovieIdNumber++;

            movieDTO.Title = Guid.NewGuid().ToString();
            movieDTO.RatingName = Guid.NewGuid().ToString();
            movieDTO.ReleaseCountry = Guid.NewGuid().ToString();
            movieDTO.Plot = Guid.NewGuid().ToString();
            movieDTO.PosterURL = Guid.NewGuid().ToString();

            movieDTO.ReleaseDate = GetRandomDateTime().ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
            movieDTO.RuntimeMinutes = (short)randomGen.Next(200);
            movieDTO.IsReleased = randomGen.Next(1) == 1;
            
            movieDTO.MovieActors = new List<string>();
            for (int i = 0; i < numberOfActors; i++)
            {
                movieDTO.MovieActors.Add(Guid.NewGuid().ToString());
            }
            
            movieDTO.MovieDirectors = new List<string>();
            for (int i = 0; i < numberOfDirectors; i++)
            {
                movieDTO.MovieDirectors.Add(Guid.NewGuid().ToString());
            }
            
            movieDTO.MovieGenres = new List<string>();
            for (int i = 0; i < numberofGenres; i++)
            {
                movieDTO.MovieGenres.Add(Guid.NewGuid().ToString());
            }
            
            movieDTO.MovieLanguages = new List<string>();
            for (int i = 0; i < numberOfLanguages; i++)
            {
                movieDTO.MovieLanguages.Add(Guid.NewGuid().ToString());
            }
            
            movieDTO.MovieTags = new List<string>();
            for (int i = 0; i < numberOfTags; i++)
            {
                movieDTO.MovieTags.Add(Guid.NewGuid().ToString());
            }

            return movieDTO;
        }

        private static DateTime GetRandomDateTime()
        {
            DateTime dateTime = DateTime.Now;
            
            return dateTime.AddDays(-1*randomGen.Next(10000));
        }

        /// <summary>
        /// Adds the provided Movie and all associated information (Actors, Directors, Genres,
        /// Languages, Tags) to the provided context. Assumes the all Movie information is
        /// unique, GetRandomMovie() provides a Movie with information that is astronomically
        /// unlikely to repeat.
        /// </summary>
        /// <param name="context"></param>
        /// <param name="movieDTO"></param>
        public static void AddMovieDTOToDatabase(Cinephiliacs_MovieContext context, MovieDTO movieDTO)
        {
            Movie movie = new Movie();
            movie.ImdbId = movieDTO.ImdbId;
            movie.Title = movieDTO.Title;

            Rating newRating = new Rating();
            newRating.RatingName = movieDTO.RatingName;
            context.Ratings.Add(newRating);
            context.SaveChanges();
            movie.RatingId = context.Ratings.FirstOrDefault(r => r.RatingName == movieDTO.RatingName).RatingId;

            movie.ReleaseDate = DateTime.ParseExact(movieDTO.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            movie.IsReleased = true;
            movie.ReleaseCountry = movieDTO.ReleaseCountry;
            movie.RuntimeMinutes = movieDTO.RuntimeMinutes;
            movie.Plot = movieDTO.Plot;
            movie.PosterUrl = movieDTO.PosterURL;

            context.Movies.Add(movie);
            context.SaveChanges();
            
            foreach (var movieActorName in movieDTO.MovieActors)
            {
                Actor newActor = new Actor();
                newActor.ActorName = movieActorName;
                context.Actors.Add(newActor);
                context.SaveChanges();

                Actor actor = context.Actors.FirstOrDefault(a => a.ActorName == movieActorName);

                MovieActor movieActor = new MovieActor();
                movieActor.ImdbId = movieDTO.ImdbId;
                movieActor.ActorId = actor.ActorId;
                context.MovieActors.Add(movieActor);
                context.SaveChanges();
            }
            
            foreach (var movieDirectorName in movieDTO.MovieDirectors)
            {
                Director newDirector = new Director();
                newDirector.DirectorName = movieDirectorName;
                context.Directors.Add(newDirector);
                context.SaveChanges();

                Director director = context.Directors.FirstOrDefault(d => d.DirectorName == movieDirectorName);

                MovieDirector movieDirector = new MovieDirector();
                movieDirector.ImdbId = movieDTO.ImdbId;
                movieDirector.DirectorId = director.DirectorId;
                context.MovieDirectors.Add(movieDirector);
                context.SaveChanges();
            }
            
            foreach (var movieGenreName in movieDTO.MovieGenres)
            {
                Genre newGenre = new Genre();
                newGenre.GenreName = movieGenreName;
                context.Genres.Add(newGenre);
                context.SaveChanges();

                Genre genre = context.Genres.FirstOrDefault(a => a.GenreName == movieGenreName);

                MovieGenre movieGenre = new MovieGenre();
                movieGenre.ImdbId = movieDTO.ImdbId;
                movieGenre.GenreId = genre.GenreId;
                context.MovieGenres.Add(movieGenre);
                context.SaveChanges();
            }
            
            foreach (var movieLanguageName in movieDTO.MovieLanguages)
            {
                Language newLanguage = new Language();
                newLanguage.LanguageName = movieLanguageName;
                context.Languages.Add(newLanguage);
                context.SaveChanges();

                Language language = context.Languages.FirstOrDefault(a => a.LanguageName == movieLanguageName);

                MovieLanguage movieLanguage = new MovieLanguage();
                movieLanguage.ImdbId = movieDTO.ImdbId;
                movieLanguage.LanguageId = language.LanguageId;
                context.MovieLanguages.Add(movieLanguage);
                context.SaveChanges();
            }
            
            foreach (var movieTagName in movieDTO.MovieTags)
            {
                Tag newTag = new Tag();
                newTag.TagName = movieTagName;
                newTag.IsBanned = false;
                context.Tags.Add(newTag);
                context.SaveChanges();

                MovieTag movieTag = new MovieTag();
                movieTag.ImdbId = movieDTO.ImdbId;
                movieTag.TagName = movieTagName;
                movieTag.VoteSum = 1;
                context.MovieTags.Add(movieTag);
                context.SaveChanges();
            }
        }
    }
}