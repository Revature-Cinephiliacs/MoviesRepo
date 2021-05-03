
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using Logic.ApiHelper;
using Model;
using Repository;
using Repository.Models;

namespace Logic
{
    // Comments for methods implemented from IMovieLogic reside in the interface.
    public class MovieLogic : IMovieLogic
    {
        private readonly RepoLogic _repo;

        public MovieLogic(RepoLogic repo)
        {
            _repo = repo;
        }

        public async Task<MovieDTO> GetMovie(string movieId)
        {
            if(_repo.MovieExists(movieId))
            {
                Movie movie = _repo.GetMovieFullInfo(movieId);

                var tagNames = new List<string>();
                foreach (var movieTag in movie.MovieTags)
                {
                    var tag = _repo.GetTag(movieTag.TagName);
                    if(tag != null && tag.IsBanned == false)
                    {
                        tagNames.Add(tag.TagName);
                    }
                }

                return Mapper.MovieToMovieDTO(movie);
            }

            ApiHelper.MovieObject movieObject = await ApiHelper.ApiProcessor.LoadMovieAsync(movieId);
            if(movieObject == null || movieObject.imdbID != movieId)
            {
                return null;
            }
            return Mapper.MovieObjectToMovieDTO(movieObject);
        }

        public List<string> SearchMovies(Dictionary<string, string[]> filters)
        {
            string ratingName = null;
            var filterResults = new List<List<string>>();
            foreach (var filter in filters)
            {
                switch (filter.Key.ToLower())
                {
                    case "any":
                        filterResults.Add(FilterMoviesByAny(filter.Value));
                    break;
                    case "tags":
                    case "tag":
                        filterResults.Add(FilterMoviesByTags(filter.Value));
                    break;
                    case "actors":
                    case "actor":
                        filterResults.Add(FilterMoviesByActors(filter.Value));
                    break;
                    case "directors":
                    case "director":
                        filterResults.Add(FilterMoviesByDirectors(filter.Value));
                    break;
                    case "languages":
                    case "language":
                        filterResults.Add(FilterMoviesByLanguages(filter.Value));
                    break;
                    case "genres":
                    case "genre":
                        filterResults.Add(FilterMoviesByGenres(filter.Value));
                    break;
                    case "rating":
                        if(String.IsNullOrWhiteSpace(filter.Value[0]))
                        {
                            return new List<string>();
                        }
                        ratingName = filter.Value[0];
                    break;
                }
                if(filterResults.Count > 1)
                {
                    if(filterResults[0].Count == 0)
                    {
                        return new List<string>();
                    }
                    for (int outer = filterResults.Count - 1; outer > 0; outer--)
                    {
                        for (int inner = filterResults[0].Count - 1; inner >= 0; inner--)
                        {
                            if(!filterResults[outer].Contains(filterResults[0][inner]))
                            {
                                filterResults[0].RemoveAt(inner);
                            }
                        }
                    }
                    if(filterResults[0].Count == 0)
                    {
                        return new List<string>();
                    }
                    filterResults.RemoveAt(1);
                }
            }

            // Filter by rating
            if(ratingName != null)
            {
                if(filterResults.Count == 0)
                {
                    var movies = _repo.GetAllMovies();
                    if(movies == null)
                    {
                        return new List<string>();
                    }
                    var movieIds = new List<string>();
                    foreach (var movie in movies)
                    {
                        movieIds.Add(movie.ImdbId);
                    }
                    filterResults.Add(movieIds);
                }
                FilterMoviesByRating(filterResults[0], ratingName);
                if(filterResults[0].Count == 0)
                {
                    return new List<string>();
                }
            }

            return filterResults[0];
        }

        public async Task<bool> TagMovie(TaggingDTO taggingDTO)
        {
            if(!_repo.MovieExists(taggingDTO.MovieId))
            {
                ApiHelper.MovieObject movieObject = await ApiHelper.ApiProcessor
                    .LoadMovieAsync(taggingDTO.MovieId);
                if(movieObject == null)
                {
                    return false;
                }
                MovieDTO movieDTO = Mapper.MovieObjectToMovieDTO(movieObject);
                if(!(await CreateMovie(movieDTO)))
                {
                    return false;
                }
            }

// Call the User microservice to make sure the user exists

            MovieTagUser movieTagUser = new MovieTagUser();
            movieTagUser.ImdbId = taggingDTO.MovieId;
            movieTagUser.TagName = taggingDTO.TagName;
            movieTagUser.UserId = taggingDTO.UserId;
            movieTagUser.IsUpvote = taggingDTO.IsUpvote;
            if(_repo.MovieTagUserExists(movieTagUser))
            {
                _repo.UpdateMovieTagUser(movieTagUser);
            }
            else
            {
                _repo.AddMovieTagUser(movieTagUser);
            }
            return true;
        }

        public bool SetTagBanStatus(string tagName, bool isBan)
        {
            if(!_repo.TagExists(tagName))
            {
                return false;
            }
            Tag tag = _repo.GetTag(tagName);
            tag.IsBanned = isBan;
            _repo.UpdateTag(tag);
            return true;
        }

        public async Task<bool> UpdateMovie(string movieId, MovieDTO movieDTO)
        {
            if(movieDTO.ImdbId != null && movieDTO.ImdbId != movieId)
            {
                return false;
            }

            if(!_repo.MovieExists(movieId))
            {
                return await CreateMovie(movieDTO);
            }
            
            Movie movie = _repo.GetMovie(movieId);

            movie.Title = movieDTO.Title;
            movie.ReleaseCountry = movieDTO.ReleaseCountry;
            movie.RuntimeMinutes = movieDTO.RuntimeMinutes;
            movie.Plot = movieDTO.Plot;
            movie.PosterUrl = movieDTO.PosterURL;

            if(String.IsNullOrEmpty(movieDTO.RatingName))
            {
                movie.RatingId = null;
            }
            else
            {
                if(!_repo.RatingExists(movieDTO.RatingName))
                {
                    Rating newRating = new Rating();
                    newRating.RatingName = movieDTO.RatingName;
                    _repo.AddRating(newRating);
                }
                movie.RatingId = _repo.GetRating(movieDTO.RatingName).RatingId;
            }

            if(String.IsNullOrEmpty(movieDTO.ReleaseDate))
            {
                movie.ReleaseDate = null;
                movie.IsReleased = null;
            }
            else
            {
                movie.ReleaseDate = DateTime.ParseExact(movieDTO.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                movie.IsReleased = true;
            }

            _repo.UpdateMovie(movie);

            _repo.ClearMovieActors(movieId);
            if(movieDTO.MovieActors != null)
            {
                foreach (var movieActorName in movieDTO.MovieActors)
                {
                    if(!_repo.AddMovieActor(movieId, movieActorName))
                    {
                        return false;
                    }
                }
            }

            _repo.ClearMovieDirectors(movieId);
            if(movieDTO.MovieDirectors != null)
            {
                foreach (var movieDirectorName in movieDTO.MovieDirectors)
                {
                    if(!_repo.AddMovieDirector(movieId, movieDirectorName))
                    {
                        return false;
                    }
                }
            }

            _repo.ClearMovieGenres(movieId);
            if(movieDTO.MovieGenres != null)
            {
                foreach (var movieGenreName in movieDTO.MovieGenres)
                {
                    if(!_repo.AddMovieGenre(movieId, movieGenreName))
                    {
                        return false;
                    }
                }
            }

            _repo.ClearMovieLanguages(movieId);
            if(movieDTO.MovieLanguages != null)
            {
                foreach (var movieLanguageName in movieDTO.MovieLanguages)
                {
                    if(!_repo.AddMovieLanguage(movieId, movieLanguageName))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        public async Task<List<MovieDTO>> recommendedMovies(string imdbId)
        {
            List<string> recommendedURLs = await ApiProcessor.LoadRecommendedMovies(imdbId);
            var getMovieTasks = new List<Task<MovieDTO>>();
            foreach (var recommendedURL in recommendedURLs)
            {
                var movieId = ParseMovieIdFromURL(recommendedURL);

                getMovieTasks.Add(GetMovie(movieId));
            }

            var recommendedDTOs = new List<MovieDTO>();
            while(getMovieTasks.Count > 0)
            {
                var completedTask = await Task.WhenAny(getMovieTasks);
                recommendedDTOs.Add(completedTask.Result);
                getMovieTasks.Remove(completedTask);
            }

            return recommendedDTOs;
        }

        public async Task<List<MovieDTO>> recommendedMoviesByUserId(string userId)
        {
            var loadRecommendedTask = new List<Task<List<string>>>();
            List<string> followedMovieIds = _repo.GetFollowingMovies(userId);
            if (followedMovieIds.Count > 5 )
            {
                for (int i = 0; i < 5; i++)
                {
                    loadRecommendedTask.Add(ApiProcessor.LoadRecommendedMovies(followedMovieIds[i]));
                }
            }

            if (followedMovieIds.Count < 5)
            {
                foreach (var followedMovieId in followedMovieIds)
                {
                    loadRecommendedTask.Add(ApiProcessor.LoadRecommendedMovies(followedMovieId));
                } 
            }
            var movieIds = new List<string>();
            while(loadRecommendedTask.Count > 0)
            {
                var completedTask = await Task.WhenAny(loadRecommendedTask);
                foreach (var recommendedURL in completedTask.Result)
                {
                    var movieId = ParseMovieIdFromURL(recommendedURL);
                    movieIds.Add(movieId);
                }
                loadRecommendedTask.Remove(completedTask);
            }

            var getMovieTasks = new List<Task<MovieDTO>>();
            foreach (var movieId in movieIds)
            {
                getMovieTasks.Add(GetMovie(movieId));
            }

            var recommendedDTOs = new List<MovieDTO>();
            while(getMovieTasks.Count > 0)
            {
                var completedTask = await Task.WhenAny(getMovieTasks);
                recommendedDTOs.Add(completedTask.Result);
                getMovieTasks.Remove(completedTask);
            }

            RemoveFollowedMovies(recommendedDTOs, followedMovieIds);

            return recommendedDTOs;
        }

        /// <summary>
        /// Splits a single string into a list of individual words.
        /// Removes all characters that are not letters, including
        /// whitespace. Returns an empty list if the input string
        /// is shorter than 11 characters.
        /// </summary>
        /// <param name="plot"></param>
        /// <returns></returns>
        private List<string> SplitPlotIntoWords(string plot)
        {
            var plotWords = new List<string>();
            if(plot != null && plot.Length > 10)
            {
                var lettersOnlyPlot = new string((from c in plot
                  where char.IsWhiteSpace(c) || char.IsLetter(c)
                  select c ).ToArray());
                lettersOnlyPlot = lettersOnlyPlot.ToLowerInvariant();
                plotWords = lettersOnlyPlot.Split(' ').ToList<string>();
            }
            return plotWords;
        }

        public async Task<bool> CreateMovie(MovieDTO movieDTO)
        {
            if(movieDTO.ImdbId == null || _repo.MovieExists(movieDTO.ImdbId))
            {
                return false;
            }

            Movie movie = new Movie();
            movie.ImdbId = movieDTO.ImdbId;
            
            AppendMoviesOptionalProperties(movie, movieDTO);

            _repo.AddMovie(movie);

            var plotWords = SplitPlotIntoWords(movieDTO.Plot);
            foreach (var plotWord in plotWords)
            {
                Word dbWord = _repo.GetWord(plotWord);
                if(dbWord == null)
                {
                    var wordObject = await ApiHelper.ApiProcessor.LoadDefinitionAsync(plotWord);

                    bool wordIsTag = ProcessWordObject(plotWord, wordObject);

                    if(wordIsTag)
                    {
                        var movieTagUser = new MovieTagUser();
                        movieTagUser.ImdbId = movieDTO.ImdbId;
                        movieTagUser.UserId = "~AutoGenerated";
                        movieTagUser.TagName = wordObject.Word;
                        movieTagUser.IsUpvote = true;
                        if(!_repo.MovieTagUserExists(movieTagUser))
                        {
                            _repo.AddMovieTagUser(movieTagUser);
                        }
                    }
                }
                else
                {
                    if(dbWord.IsTag)
                    {
                        var movieTagUser = new MovieTagUser();
                        movieTagUser.ImdbId = movieDTO.ImdbId;
                        movieTagUser.UserId = "~AutoGenerated";
                        movieTagUser.TagName = dbWord.BaseWord;
                        movieTagUser.IsUpvote = true;
                        if(!_repo.MovieTagUserExists(movieTagUser))
                        {
                            _repo.AddMovieTagUser(movieTagUser);
                        }
                    }
                }
            }

            if(movieDTO.MovieActors != null)
            {
                foreach (var movieActorName in movieDTO.MovieActors)
                {
                    if(!_repo.AddMovieActor(movieDTO.ImdbId, movieActorName))
                    {
                        return false;
                    }
                }
            }
            if(movieDTO.MovieDirectors != null)
            {
                foreach (var movieDirectorName in movieDTO.MovieDirectors)
                {
                    if(!_repo.AddMovieDirector(movieDTO.ImdbId, movieDirectorName))
                    {
                        return false;
                    }
                }
            }
            if(movieDTO.MovieGenres != null)
            {
                foreach (var movieGenreName in movieDTO.MovieGenres)
                {
                    if(!_repo.AddMovieGenre(movieDTO.ImdbId, movieGenreName))
                    {
                        return false;
                    }
                }
            }
            if(movieDTO.MovieLanguages != null)
            {
                foreach (var movieLanguageName in movieDTO.MovieLanguages)
                {
                    if(!_repo.AddMovieLanguage(movieDTO.ImdbId, movieLanguageName))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        /// <summary>
        /// Returns true if the wordObject meets the criteria to be a tag.
        /// </summary>
        /// <param name="wordObject"></param>
        /// <returns></returns>
        private bool WordQualifiesAsTag(WordObject wordObject)
        {
            const double NOUN_RATIO_THRESHOLD = 0.45;
            const int MINIMUM_LENGTH = 3;

            if(wordObject.Word.Length < MINIMUM_LENGTH)
            {
                return false;
            }

            int nounCount = 0;
            foreach (var definition in wordObject.Definitions)
            {
                if(definition.PartOfSpeech == "noun")
                {
                    nounCount++;
                }
            }
            return ((double)nounCount / wordObject.Definitions.Count) > NOUN_RATIO_THRESHOLD;
        }

        /// <summary>
        /// Processes the word object, adding new words to the database
        /// with the appropriate properties.
        /// Returns true if the wordObject meets the criteria to be a tag.
        /// </summary>
        /// <param name="wordObject"></param>
        /// <returns></returns>
        private bool ProcessWordObject(string originalWord, WordObject wordObject)
        {
            if(wordObject == null)
            {
                return false;
            }

            bool wordIsTag = WordQualifiesAsTag(wordObject);

            var newWord = new Word();
            newWord.IsTag = wordIsTag;
            newWord.Word1 = originalWord;
            newWord.BaseWord = wordObject.Word;
            _repo.AddWord(newWord);

            if(wordObject.Word != originalWord)
            {
                var baseWord = new Word();
                baseWord.IsTag = wordIsTag;
                baseWord.Word1 = wordObject.Word;
                baseWord.BaseWord = wordObject.Word;
                _repo.AddWord(baseWord);
            }
            return wordIsTag;
        }

        public async Task<bool> AppendMovie(string movieId, MovieDTO movieDTO)
        {
            if(!_repo.MovieExists(movieId))
            {
                ApiHelper.MovieObject movieObject = await ApiHelper.ApiProcessor
                    .LoadMovieAsync(movieId);
                if(movieObject == null)
                {
                    return false;
                }
                MovieDTO newMovieDTO = Mapper.MovieObjectToMovieDTO(movieObject);
                if(!(await CreateMovie(newMovieDTO)))
                {
                    return false;
                }
            }

            Movie movie = _repo.GetMovie(movieId);

            if(movie == null)
            {
                return false;
            }

            AppendMoviesOptionalProperties(movie, movieDTO);

            _repo.UpdateMovie(movie);
            
            if(movieDTO.MovieActors != null)
            {
                foreach (var movieActorName in movieDTO.MovieActors)
                {
                    if(!_repo.MovieActorExists(movieId, movieActorName))
                    {
                        if(!_repo.AddMovieActor(movieId, movieActorName))
                        {
                            return false;
                        }
                    }
                }
            }
            if(movieDTO.MovieDirectors != null)
            {
                foreach (var movieDirectorName in movieDTO.MovieDirectors)
                {
                    if(!_repo.MovieDirectorExists(movieId, movieDirectorName))
                    {
                        if(!_repo.AddMovieDirector(movieId, movieDirectorName))
                        {
                            return false;
                        }
                    }
                }
            }
            if(movieDTO.MovieGenres != null)
            {
                foreach (var movieGenreName in movieDTO.MovieGenres)
                {
                    if(!_repo.MovieGenreExists(movieId, movieGenreName))
                    {
                        if(!_repo.AddMovieGenre(movieId, movieGenreName))
                        {
                            return false;
                        }
                    }
                }
            }
            if(movieDTO.MovieLanguages != null)
            {
                foreach (var movieLanguageName in movieDTO.MovieLanguages)
                {
                    if(!_repo.MovieLanguageExists(movieId, movieLanguageName))
                    {
                        if(!_repo.AddMovieLanguage(movieId, movieLanguageName))
                        {
                            return false;
                        }
                    }
                }
            }
            return true;
        }

        public bool DeleteMovie(string movieId)
        {
            if(!_repo.MovieExists(movieId))
            {
                return false;
            }
            _repo.DeleteMovie(movieId);
            return true;
        }

        public bool FollowMovie(string movieId, string userId)
        {
            if(!_repo.MovieExists(movieId))
            {
                return false;
            }

// Call the User microservice to make sure the user exists

            _repo.AddFollowingMovie(movieId, userId);
            return true;
        }

        public bool UnfollowMovie(string movieId, string userId)
        {
            if(!_repo.MovieExists(movieId))
            {
                return false;
            }

// Call the User microservice to make sure the user exists

            _repo.DeleteFollowingMovie(movieId, userId);
            return true;
        }

        public List<string> GetFollowingMovies(string userId)
        {
            return _repo.GetFollowingMovies(userId);
        }

        public List<string> GetAllTags()
        {
            var tags = _repo.GetAllTags();
            if(tags == null)
            {
                return null;
            }

            var tagNames = new List<string>();
            foreach (var tag in tags)
            {
                if(!tag.IsBanned)
                {
                    tagNames.Add(tag.TagName);
                }
            }
            return tagNames;
        }

        /// <summary>
        /// Takes in a review with an empty follower list.
        /// Gets the follower list from the repo.
        /// Adds follower list to review.
        /// </summary>
        /// <param name="review"></param>
        /// <returns>ReviewNotification</returns>
        public ReviewNotification GetFollowersForReviewNotification(ReviewNotification review)
        {
            
            review.Followers = _repo.GetFollowingMoviesByMovieID(review.Imdbid);
            return review;
        }

        /// <summary>
        /// Takes in a discussion notification with it's existing follower list.
        /// Gets the follower list from the repo for the movie noted in the discussion.
        /// Adds movie follower list to existing list.
        /// </summary>
        /// <param name="forumNote"></param>
        /// <returns>ForumNotification</returns>
        public ForumNotification GetFollowersForForumNotification(ForumNotification forumNote)
        {
            if(forumNote.Imdbid != null){
                forumNote.Followers = _repo.GetFollowingMoviesByMovieID(forumNote.Imdbid);
            }
            return forumNote;
        }

        /// <summary>
        /// Removes any movies from the list argument that do not have a property
        /// (Actor, Director, Genre, Language, Tag) that matches each of the
        /// provided names.
        /// </summary>
        /// <param name="anyNames"></param>
        /// <returns></returns>
        private List<string> FilterMoviesByAny(string[] anyNames)
        {
            var filterResults = new List<List<string>>();
            foreach (var name in anyNames)
            {
                var resultsCollection = new List<string>();

                var tagResults = FilterMoviesByTags(new string[] {name});
                resultsCollection.AddRange(tagResults);

                var actorResults = FilterMoviesByActors(new string[] {name});
                resultsCollection.AddRange(actorResults);

                var directorResults = FilterMoviesByDirectors(new string[] {name});
                resultsCollection.AddRange(directorResults);

                var genreResults = FilterMoviesByGenres(new string[] {name});
                resultsCollection.AddRange(genreResults);

                var languageResults = FilterMoviesByLanguages(new string[] {name});
                resultsCollection.AddRange(languageResults);

                filterResults.Add(resultsCollection);
            }

            var movieIds = new List<string>();
            if(filterResults.Count == 0)
            {
                return new List<string>();
            }
            
            movieIds = filterResults[0];
            for (int outer = filterResults.Count - 1; outer > 0; outer--)
            {
                for (int inner = movieIds.Count - 1; inner >= 0; inner--)
                {
                    if(!filterResults[outer].Contains(movieIds[inner]))
                    {
                        movieIds.RemoveAt(inner);
                    }
                }
            }

            return movieIds;
        }

        /// <summary>
        /// Removes any movies from the list argument that are not tagged
        /// with all of the provided tag names.
        /// </summary>
        /// <param name="movies"></param>
        /// <param name="tagName"></param>
        private List<string> FilterMoviesByTags(string[] tagNames)
        {
            var filterResults = new List<List<string>>();
            foreach (var tagName in tagNames)
            {
                string baseTagName;
                var word = _repo.GetWord(tagName);
                if(word == null || String.IsNullOrEmpty(word.BaseWord))
                {
                    baseTagName = tagName;
                }
                else
                {
                    baseTagName = word.BaseWord;
                }
                var movieTags = _repo.GetMovieTagsByName(baseTagName);
                var filterResult = new List<string>();
                foreach (var movieTag in movieTags)
                {
                    filterResult.Add(movieTag.ImdbId);
                }
                filterResults.Add(filterResult);
            }

            var movieIds = new List<string>();
            if(filterResults.Count == 0)
            {
                return new List<string>();
            }

            movieIds = filterResults[0];
            for (int outer = filterResults.Count - 1; outer > 0; outer--)
            {
                for (int inner = movieIds.Count - 1; inner >= 0; inner--)
                {
                    if(!filterResults[outer].Contains(movieIds[inner]))
                    {
                        movieIds.RemoveAt(inner);
                    }
                }
            }

            return movieIds;
        }

        /// <summary>
        /// Removes all movies from the list argument that do not have the
        /// rating that is specified in the argument.
        /// </summary>
        /// <param name="movies"></param>
        /// <param name="ratingName"></param>
        private void FilterMoviesByRating(List<string> movieIds, string ratingName)
        {
            var rating = _repo.GetRating(ratingName);
            for (int i = movieIds.Count - 1; i >= 0; i--)
            {
                var movie = _repo.GetMovie(movieIds[i]);
                if(movie == null || movie.RatingId != rating.RatingId)
                {
                    movieIds.RemoveAt(i);
                }
            }
        }

        /// <summary>
        /// Removes all movies from the list argument that do not have all of the
        /// actors that are specified in the argument.
        /// </summary>
        /// <param name="movies"></param>
        /// <param name="actorName"></param>
        private List<string> FilterMoviesByActors(string[] actorNames)
        {
            var filterResults = new List<List<string>>();
            foreach (var actorName in actorNames)
            {
                var actor = _repo.GetActor(actorName);
                if(actor != null)
                {
                    var movieActors = _repo.GetMovieActorsById(actor.ActorId);
                    var filterResult = new List<string>();
                    foreach (var movieActor in movieActors)
                    {
                        filterResult.Add(movieActor.ImdbId);
                    }
                    filterResults.Add(filterResult);
                }
            }

            var movieIds = new List<string>();
            if(filterResults.Count == 0)
            {
                return new List<string>();
            }
            
            movieIds = filterResults[0];
            for (int outer = filterResults.Count - 1; outer > 0; outer--)
            {
                for (int inner = movieIds.Count - 1; inner >= 0; inner--)
                {
                    if(!filterResults[outer].Contains(movieIds[inner]))
                    {
                        movieIds.RemoveAt(inner);
                    }
                }
            }

            return movieIds;
        }

        /// <summary>
        /// Removes all movies from the list argument that do not have all of the
        /// directors that are specified in the argument.
        /// </summary>
        /// <param name="movies"></param>
        /// <param name="directorName"></param>
        private List<string> FilterMoviesByDirectors(string[] directorNames)
        {
            var filterResults = new List<List<string>>();
            foreach (var directorName in directorNames)
            {
                var director = _repo.GetDirector(directorName);
                if(director != null)
                {
                    var movieDirectors = _repo.GetMovieDirectorsById(director.DirectorId);
                    var filterResult = new List<string>();
                    foreach (var movieDirector in movieDirectors)
                    {
                        filterResult.Add(movieDirector.ImdbId);
                    }
                    filterResults.Add(filterResult);
                }
            }

            var movieIds = new List<string>();
            if(filterResults.Count == 0)
            {
                return new List<string>();
            }
            
            movieIds = filterResults[0];
            for (int outer = filterResults.Count - 1; outer > 0; outer--)
            {
                for (int inner = movieIds.Count - 1; inner >= 0; inner--)
                {
                    if(!filterResults[outer].Contains(movieIds[inner]))
                    {
                        movieIds.RemoveAt(inner);
                    }
                }
            }

            return movieIds;
        }

        /// <summary>
        /// Removes all movies from the list argument that do not have all of the
        /// genres that are specified in the argument.
        /// </summary>
        /// <param name="movies"></param>
        /// <param name="genreName"></param>
        private List<string> FilterMoviesByGenres(string[] genreNames)
        {
            var filterResults = new List<List<string>>();
            foreach (var genreName in genreNames)
            {
                var genre = _repo.GetGenre(genreName);
                if(genre != null)
                {
                    var movieGenres = _repo.GetMovieGenresById(genre.GenreId);
                    var filterResult = new List<string>();
                    foreach (var movieGenre in movieGenres)
                    {
                        filterResult.Add(movieGenre.ImdbId);
                    }
                    filterResults.Add(filterResult);
                }
            }

            var movieIds = new List<string>();
            if(filterResults.Count == 0)
            {
                return new List<string>();
            }
            
            movieIds = filterResults[0];
            for (int outer = filterResults.Count - 1; outer > 0; outer--)
            {
                for (int inner = movieIds.Count - 1; inner >= 0; inner--)
                {
                    if(!filterResults[outer].Contains(movieIds[inner]))
                    {
                        movieIds.RemoveAt(inner);
                    }
                }
            }

            return movieIds;
        }

        /// <summary>
        /// Removes all movies from the list argument that do not have all of the
        /// languages that are specified in the argument.
        /// </summary>
        /// <param name="movies"></param>
        /// <param name="languageName"></param>
        private List<string> FilterMoviesByLanguages(string[] languageNames)
        {
            var filterResults = new List<List<string>>();
            foreach (var languageName in languageNames)
            {
                var language = _repo.GetLanguage(languageName);
                if(language != null)
                {
                    var movieLanguages = _repo.GetMovieLanguagesById(language.LanguageId);
                    var filterResult = new List<string>();
                    foreach (var movieLanguage in movieLanguages)
                    {
                        filterResult.Add(movieLanguage.ImdbId);
                    }
                    filterResults.Add(filterResult);
                }
            }

            var movieIds = new List<string>();
            if(filterResults.Count == 0)
            {
                return new List<string>();
            }
            
            movieIds = filterResults[0];
            for (int outer = filterResults.Count - 1; outer > 0; outer--)
            {
                for (int inner = movieIds.Count - 1; inner >= 0; inner--)
                {
                    if(!filterResults[outer].Contains(movieIds[inner]))
                    {
                        movieIds.RemoveAt(inner);
                    }
                }
            }

            return movieIds;
        }

        /// <summary>
        /// Updates all of the optional, non-List properties of the Movie argument
        /// to the properties supplied in the MovieDTO argument. Any null property
        /// of the MovieDTO remains unchanged in the Movie object.
        /// </summary>
        /// <param name="movie"></param>
        /// <param name="movieDTO"></param>
        public void AppendMoviesOptionalProperties(Movie movie, MovieDTO movieDTO)
        {
            if(!String.IsNullOrEmpty(movieDTO.Title))
            {
                movie.Title = movieDTO.Title;
            }

            if(!String.IsNullOrEmpty(movieDTO.RatingName))
            {
                if(!_repo.RatingExists(movieDTO.RatingName))
                {
                    Rating newRating = new Rating();
                    newRating.RatingName = movieDTO.RatingName;
                    _repo.AddRating(newRating);
                }
                movie.RatingId = _repo.GetRating(movieDTO.RatingName).RatingId;
            }

            if(!String.IsNullOrEmpty(movieDTO.ReleaseDate))
            {
                movie.ReleaseDate = DateTime.ParseExact(movieDTO.ReleaseDate, "yyyy-MM-dd", CultureInfo.InvariantCulture);
                movie.IsReleased = true;
            }
            if(!String.IsNullOrEmpty(movieDTO.ReleaseCountry))
            {
                movie.ReleaseCountry = movieDTO.ReleaseCountry;
            }
            if(movieDTO.RuntimeMinutes != null)
            {
                movie.RuntimeMinutes = movieDTO.RuntimeMinutes;
            }
            if(!String.IsNullOrEmpty(movieDTO.Plot))
            {
                movie.Plot = movieDTO.Plot;
            }
            if(!String.IsNullOrEmpty(movieDTO.PosterURL))
            {
                movie.PosterUrl = movieDTO.PosterURL;
            }
        }

        /// <summary>
        /// Extracts the movie id string from a url in the format:
        /// "/title/{movieid}/"
        /// </summary>
        /// <param name="url"></param>
        /// <returns></returns>
        private string ParseMovieIdFromURL(string url)
        {
            string movieId = url;
            if(url.Length > 8)
            {
                movieId = url.Substring(7, url.Length - 8);
            }
            return movieId;
        }

        /// <summary>
        /// Removes any movie whose movie id exists in the followedMovieIds list
        /// from the recommendedDTOs list.
        /// </summary>
        /// <param name="recommendedDTOs"></param>
        /// <param name="followedMovieIds"></param>
        /// <returns></returns>
        private List<MovieDTO> RemoveFollowedMovies(List<MovieDTO> recommendedDTOs, List<string> followedMovieIds)
        {
            for (int i = recommendedDTOs.Count - 1; i >= 0; i--)
            {
                foreach (var movieId in followedMovieIds)
                {
                    if(recommendedDTOs[i].ImdbId == movieId)
                    {
                        recommendedDTOs.RemoveAt(i);
                    }
                }
            }
            return recommendedDTOs;
        }
    }
}
