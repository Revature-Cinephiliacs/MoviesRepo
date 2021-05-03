using System.IO;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;
using Repository.Models;
using Logic;
using System;
using Repository;
using Model;
using System.Linq;
using Logic.ApiHelper;
using System.Globalization;

namespace Seeding
{
    public class Seeding
    {
        const int MAX_MOVIE_API_TASKS = 5;
        const int MAX_WORD_API_TASKS = 5;
        const int MAX_DB_TASKS = 15;
        const int BATCH_SIZE = 50;

        // Path to the file with all the idbdIds
        const string FILE_PATH = "USAOnly.txt";

        // Determines which line of the file is the starting point
        const int STARTING_LINE = 1;

        // Sets the number of movies to add to the database. If this is greater than the
        // remaining number of lines in the file, all the remianing movies will be added.
        const int MAX_NUMBER_OF_MOVIES = 10000;
        readonly static DbContextOptions<Cinephiliacs_MovieContext> dbOptions = new DbContextOptionsBuilder<Cinephiliacs_MovieContext>()
            .UseSqlServer(@"Server=tcp:mark-moore-03012021batch-p3-sqlserver.database.windows.net,1433;Initial Catalog=Movie;Persist Security Info=False;User ID=markmoorerev;Password=03012021batch!;MultipleActiveResultSets=False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;").Options;
        private static List<string> ReadMovieIdsFromFile(string path)
        {
            var movieIdList = new List<string>();

            StreamReader reader = new StreamReader(path);
            try
            {
                do
                {
                    movieIdList.Add(reader.ReadLine().Trim());
                }
                while(reader.Peek() != -1);
            }
            finally
            {
                reader.Close();
            }
            return movieIdList;
        }

        public async static Task<bool> SeedDbFromCSV(string startingLine)
        {
            int starting_index = 0;
            try{
                starting_index = int.Parse(startingLine) - 1;
            }
            catch{
                starting_index = 0;
            }
            if(starting_index < 0 || starting_index > 100000)
            {
                starting_index = 0;
            }

            var movieIdList = ReadMovieIdsFromFile(FILE_PATH);

            int limitCounter = 0;
            var getMovieTasks = new List<Task<MovieDTO>>();
            var movieDTOs = new List<MovieDTO>();
            var addMovieBatchTasks = new List<Task<bool>>();
            
            for (int index = starting_index; index < movieIdList.Count
                && limitCounter < MAX_NUMBER_OF_MOVIES; index++)
            {
                getMovieTasks.Add(GetMovie(movieIdList[index]));
                limitCounter++;

                while(getMovieTasks.Count >= MAX_MOVIE_API_TASKS)
                {
                    var completedTask = await Task<MovieDTO>.WhenAny(getMovieTasks);
                    if(completedTask.Result != null)
                    {
                        movieDTOs.Add(completedTask.Result);
                    }
                    else
                    {
                        Console.WriteLine("Public Movie API Returned Null!");
                    }
                    getMovieTasks.Remove(completedTask);
                }
                if(movieDTOs.Count >= BATCH_SIZE)
                {
                    // Move a batch of MovieDTO object to a new list which will be
                    // passed to the AddMovies task.
                    var movieDTObatch = new List<MovieDTO>();
                    for (int i = BATCH_SIZE - 1; i >= 0; i--)
                    {
                        movieDTObatch.Add(movieDTOs[i]);
                        movieDTOs.RemoveAt(i);
                    }

                    // Wait for the previous AddMovies task to complete.
                    while (addMovieBatchTasks.Count > 0)
                    {
                        var completedTask = await Task<List<MovieDTO>>.WhenAny(addMovieBatchTasks);
                        Console.WriteLine(completedTask.Result.ToString());
                        addMovieBatchTasks.Remove(completedTask);
                    }
                    Console.WriteLine("Starting Batch of " + movieDTObatch.Count.ToString() + " Movies, from Lines "
                        + (index - BATCH_SIZE - 2).ToString() + " - " + (index - 3).ToString() + ".");
                    addMovieBatchTasks.Add(AddMovies(movieDTObatch));
                }
            }
            // Await remaining GetMovie tasks
            Console.WriteLine("Awaiting Final GetMovie Tasks!");
            while(getMovieTasks.Count > 0)
            {
                var completedTask = await Task<MovieDTO>.WhenAny(getMovieTasks);
                if(completedTask.Result != null)
                {
                    movieDTOs.Add(completedTask.Result);
                }
                else
                {
                    Console.WriteLine("Public Movie API Returned Null!");
                }
                getMovieTasks.Remove(completedTask);
            }
            while(movieDTOs.Count > 0)
            {
                // Move a batch of MovieDTO object to a new list which will be
                // passed to the AddMovies task.
                var movieDTObatch = new List<MovieDTO>();
                var thisBatchSize = BATCH_SIZE;
                if(movieDTOs.Count < BATCH_SIZE)
                {
                    thisBatchSize = movieDTOs.Count;
                }
                for (int i = thisBatchSize - 1; i >= 0; i--)
                {
                    movieDTObatch.Add(movieDTOs[i]);
                    movieDTOs.RemoveAt(i);
                }

                // Wait for the previous AddMovies task to complete.
                while (addMovieBatchTasks.Count > 0)
                {
                    var completedTask = await Task<List<MovieDTO>>.WhenAny(addMovieBatchTasks);
                    Console.WriteLine(completedTask.Result.ToString());
                    addMovieBatchTasks.Remove(completedTask);
                }
                Console.WriteLine("Starting Batch with " + movieDTObatch.Count.ToString() + " Movies to Add!");
                addMovieBatchTasks.Add(AddMovies(movieDTObatch));
            }
            
            while (addMovieBatchTasks.Count > 0)
            {
                var completedTask = await Task<List<MovieDTO>>.WhenAny(addMovieBatchTasks);
                Console.WriteLine(completedTask.Result.ToString());
                addMovieBatchTasks.Remove(completedTask);
            }
  
            return true;
        }
        
        /// <summary>
        /// Gets a Movie from the public API. Splits the plot into individual words,
        /// removes duplicate words (case-insensitive), save them in the MovieTags list.
        /// </summary>
        /// <param name="movieId"></param>
        /// <returns></returns>
        public static async Task<MovieDTO> GetMovie(string movieId)
        {
            Logic.ApiHelper.MovieObject movieObject = await Logic.ApiHelper.ApiProcessor
                .LoadMovieAsync(movieId);
            if(movieObject == null)
            {
                return null;
            }
            MovieDTO newMovieDTO = Mapper.MovieObjectToMovieDTO(movieObject);
            newMovieDTO.MovieTags = SplitPlotIntoWords(newMovieDTO.Plot);
            for (int i = newMovieDTO.MovieTags.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < i; j++)
                {
                    if(newMovieDTO.MovieTags[j].ToLower() == newMovieDTO.MovieTags[i].ToLower())
                    {
                        newMovieDTO.MovieTags.RemoveAt(i);
                        break;
                    }
                }
            }

            return newMovieDTO;
        }

        /// <summary>
        /// Splits a single string into a list of individual words.
        /// Removes all characters that are not letters, including
        /// whitespace. Makes all characters lowercase. Returns an
        /// empty list if the input string is shorter than 11 
        /// characters.
        /// </summary>
        /// <param name="plot"></param>
        /// <returns></returns>
        private static List<string> SplitPlotIntoWords(string plot)
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

        /// <summary>
        /// This Task processes a batch of movieDTO objects.
        /// Only one of these will run at a time; however, GetMovie tasks
        /// may run concurrently.
        /// </summary>
        /// <param name="movieDTOs"></param>
        /// <returns></returns>
        public static async Task<bool> AddMovies(List<MovieDTO> movieDTOs)
        {
            Console.WriteLine("Accumulating Tags");
            // Use temporary object lists to accumulate all of the
            // Actors, Directors, Genres, Languages and Ratings that exist
            // in the movies to be added. These need to be added ahead of
            // time; so that the remainder of the process can be parallelized
            var actors = new List<Actor>();
            var directors = new List<Director>();
            var genres = new List<Genre>();
            var languages = new List<Language>();
            var ratings = new List<Rating>();
            var plotWords = new List<string>();

            for (int i = 0; i < movieDTOs.Count; i++)
            {
                foreach (var movieActor in movieDTOs[i].MovieActors)
                {
                    actors.Add(new Actor() {ActorName = movieActor});
                }
                foreach (var movieDirector in movieDTOs[i].MovieDirectors)
                {
                    directors.Add(new Director() {DirectorName = movieDirector});
                }
                foreach (var movieGenres in movieDTOs[i].MovieGenres)
                {
                    genres.Add(new Genre() {GenreName = movieGenres});
                }
                foreach (var movieLanguages in movieDTOs[i].MovieLanguages)
                {
                    languages.Add(new Language() {LanguageName = movieLanguages});
                }
                ratings.Add(new Rating() {RatingName = movieDTOs[i].RatingName});
                foreach (var word in movieDTOs[i].MovieTags)
                {
                    plotWords.Add(word);
                }
            }

            Console.WriteLine("Removing Duplicates");
            // Remove duplicate entries, case-insensitive
            for (int i = actors.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < i; j++)
                {
                    if(actors[j].ActorName.ToLower() == actors[i].ActorName.ToLower())
                    {
                        actors.RemoveAt(i);
                        break;
                    }
                }
            }
            for (int i = directors.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < i; j++)
                {
                    if(directors[j].DirectorName.ToLower() == directors[i].DirectorName.ToLower())
                    {
                        directors.RemoveAt(i);
                        break;
                    }
                }
            }
            for (int i = genres.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < i; j++)
                {
                    if(genres[j].GenreName.ToLower() == genres[i].GenreName.ToLower())
                    {
                        genres.RemoveAt(i);
                        break;
                    }
                }
            }
            for (int i = languages.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < i; j++)
                {
                    if(languages[j].LanguageName.ToLower() == languages[i].LanguageName.ToLower())
                    {
                        languages.RemoveAt(i);
                        break;
                    }
                }
            }
            for (int i = ratings.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < i; j++)
                {
                    if(ratings[j].RatingName.ToLower() == ratings[i].RatingName.ToLower())
                    {
                        ratings.RemoveAt(i);
                        break;
                    }
                }
            }
            for (int i = plotWords.Count - 1; i >= 0; i--)
            {
                for (int j = 0; j < i; j++)
                {
                    if(plotWords[j] == plotWords[i])
                    {
                        plotWords.RemoveAt(i);
                        break;
                    }
                }
            }

            Console.WriteLine("Processing Words");
            // Process each word, Adding any to the database that don't already exist.
            // Also add any new Tags.
            var addWordTasks = new List<Task>();
            foreach (var word in plotWords)
            {
                addWordTasks.Add(Task.Run(() => AddWord(word)));

                while(addWordTasks.Count >= MAX_WORD_API_TASKS)
                {
                    var completedTask = await Task.WhenAny(addWordTasks);
                    addWordTasks.Remove(completedTask);
                }
            }
            // Await final AddWord tasks
            while(addWordTasks.Count > 0)
            {
                var completedTask = await Task.WhenAny(addWordTasks);
                addWordTasks.Remove(completedTask);
            }


            Console.WriteLine("Add Prereqs. to Db");
            // Add new Actor, Director, Genre, Language, Rating Objects to the database
            using (var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                RepoLogic _repo = new RepoLogic(context);

                foreach (var actor in actors)
                {
                    if(!_repo.ActorExists(actor.ActorName))
                    {
                        context.Actors.Add(actor);
                    }
                }
                foreach (var director in directors)
                {
                    if(!_repo.DirectorExists(director.DirectorName))
                    {
                        context.Directors.Add(director);
                    }
                }
                foreach (var genre in genres)
                {
                    if(!_repo.GenreExists(genre.GenreName))
                    {
                        context.Genres.Add(genre);
                    }
                }
                foreach (var language in languages)
                {
                    if(!_repo.LanguageExists(language.LanguageName))
                    {
                        context.Languages.Add(language);
                    }
                }
                foreach (var rating in ratings)
                {
                    if(!_repo.RatingExists(rating.RatingName))
                    {
                        var newRating = new Rating() {
                            RatingName = rating.RatingName
                        };
                        context.Ratings.Add(newRating);
                    }
                }
                context.SaveChanges();
            }


            Console.WriteLine("Adding Movies");
            // Process each Movie.
            var addMovieTaskList = new List<Task<string>>();
            foreach (var movieDTO in movieDTOs)
            {
                addMovieTaskList.Add(Task.Run(() => AddMovie(movieDTO)));

                while(addMovieTaskList.Count >= MAX_DB_TASKS)
                {
                    var completedTask = await Task.WhenAny(addMovieTaskList);
                    if(completedTask == null)
                    {
                        Console.WriteLine("Add Movie Null Result");
                    }
                    else
                    {
                        Console.WriteLine(completedTask.Result);
                    }
                    addMovieTaskList.Remove(completedTask);
                }
            }
            // Await final AddMovie tasks
            while(addMovieTaskList.Count > 0)
            {
                var completedTask = await Task.WhenAny(addMovieTaskList);
                if(completedTask == null)
                {
                    Console.WriteLine("Add Movie Null Result");
                }
                else
                {
                    Console.WriteLine(completedTask.Result);
                }
                addMovieTaskList.Remove(completedTask);
            }
            return true;
        }

        /// <summary>
        /// If the word does not already exist in the database, its 
        /// definition is looked up and then it is added. If it qualifies
        /// as a Tag the tag is also added to the database if it does
        /// not already exist.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static async Task AddWord(string word)
        {
            using (var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                bool wordIsTag;

                Word dbWord = context.Words.FirstOrDefault(w => w.Word1 == word);
                if(dbWord == null)
                {
                    var wordObject = await Logic.ApiHelper.ApiProcessor.LoadDefinitionAsync(word);

                    wordIsTag = ProcessWordObject(word, wordObject, context);
                }
                else
                {
                    if(context.Tags.FirstOrDefault(t => t.TagName == dbWord.BaseWord) == null)
                    {
                        Tag tag = new Tag();
                        tag.TagName = dbWord.BaseWord;
                        tag.IsBanned = false;
                        context.Tags.Add(tag);
                    }
                }

                context.SaveChanges();
            }
            return;
        }

        /// <summary>
        /// Processes the word object, adding new words to the database
        /// with the appropriate properties.
        /// Returns true if the wordObject meets the criteria to be a tag.
        /// </summary>
        /// <param name="wordObject"></param>
        /// <returns></returns>
        private static bool ProcessWordObject(string originalWord, WordObject wordObject, Cinephiliacs_MovieContext context)
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

            if(wordIsTag)
            {
                if(context.Tags.FirstOrDefault(t => t.TagName == originalWord) == null)
                {
                    Tag tag = new Tag();
                    tag.TagName = originalWord;
                    tag.IsBanned = false;
                    context.Tags.Add(tag);
                }
            }
            
            if(context.Words.FirstOrDefault(w => w.Word1 == originalWord) == null)
            {
                context.Words.Add(newWord);
            }

            if(wordObject.Word != originalWord)
            {
                var baseWord = new Word();
                baseWord.IsTag = wordIsTag;
                baseWord.Word1 = wordObject.Word;
                baseWord.BaseWord = wordObject.Word;
                
                if(wordIsTag)
                {
                    if(context.Tags.FirstOrDefault(t => t.TagName == wordObject.Word) == null)
                    {
                        Tag tag = new Tag();
                        tag.TagName = wordObject.Word;
                        tag.IsBanned = false;
                        context.Tags.Add(tag);
                    }
                }

                context.SaveChanges();

                if(context.Words.FirstOrDefault(w => w.Word1 == wordObject.Word) == null)
                {
                    context.Words.Add(baseWord);
                }
            }
            return wordIsTag;
        }

        /// <summary>
        /// Returns true if the wordObject meets the criteria to be a tag.
        /// </summary>
        /// <param name="wordObject"></param>
        /// <returns></returns>
        private static bool WordQualifiesAsTag(WordObject wordObject)
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

        public static async Task<string> AddMovie(MovieDTO movieDTO)
        {
            using (var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                RepoLogic _repo = new RepoLogic(context);
                IMovieLogic _logic = new MovieLogic(_repo);

                if(await CreateMovie(movieDTO, context))
                {
                    return movieDTO.ImdbId + ": Success!";
                }
                else
                { 
                    return movieDTO.ImdbId + ": Movie already exists!";
                }
            }
        }
        
        public static async Task<bool> CreateMovie(MovieDTO movieDTO, Cinephiliacs_MovieContext context)
        {
            if(movieDTO.ImdbId == null || context.Movies.FirstOrDefault(m => m.ImdbId == movieDTO.ImdbId) != null)
            {
                return false;
            }

            Movie movie = new Movie();
            movie.ImdbId = movieDTO.ImdbId;

            if(!String.IsNullOrEmpty(movieDTO.Title))
            {
                movie.Title = movieDTO.Title;
            }

            if(!String.IsNullOrEmpty(movieDTO.RatingName))
            {
                Rating rating = context.Ratings.FirstOrDefault(r => r.RatingName == movieDTO.RatingName);
                if(rating != null)
                {
                    movie.RatingId = rating.RatingId;
                }
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

            context.Movies.Add(movie);
            context.SaveChanges();

            int counter = 0;
            var processWordTaskList = new List<Task<bool>>();
            foreach (var plotWord in movieDTO.MovieTags)
            {
                processWordTaskList.Add(ProcessWordInDb(plotWord, movieDTO.ImdbId, context));

                while(processWordTaskList.Count >= MAX_DB_TASKS)
                {
                    var completedTask = await Task.WhenAny(processWordTaskList);
                    if(completedTask.Result)
                    {
                        counter++;
                    }
                    processWordTaskList.Remove(completedTask);
                }
            }
            // Await final ProcessWordInDb tasks
            while(processWordTaskList.Count > 0)
            {
                var completedTask = await Task.WhenAny(processWordTaskList);
                if(completedTask.Result)
                {
                    counter++;
                }
                processWordTaskList.Remove(completedTask);
            }

            if(movieDTO.MovieActors != null)
            {
                foreach (var movieActorName in movieDTO.MovieActors)
                {
                    Actor actor = context.Actors.FirstOrDefault(l => l.ActorName == movieActorName);
                    
                    var movieActor = new MovieActor();
                    movieActor.ImdbId = movieDTO.ImdbId;
                    movieActor.ActorId = actor.ActorId;
                    context.MovieActors.Add(movieActor);     
                }
            }
            if(movieDTO.MovieDirectors != null)
            {
                foreach (var movieDirectorName in movieDTO.MovieDirectors)
                {
                    Director director = context.Directors.FirstOrDefault(l => l.DirectorName == movieDirectorName);
                    
                    var movieDirector = new MovieDirector();
                    movieDirector.ImdbId = movieDTO.ImdbId;
                    movieDirector.DirectorId = director.DirectorId;
                    context.MovieDirectors.Add(movieDirector);     
                }
            }
            if(movieDTO.MovieGenres != null)
            {
                foreach (var movieGenreName in movieDTO.MovieGenres)
                {
                    Genre genre = context.Genres.FirstOrDefault(g => g.GenreName == movieGenreName);
                    
                    var movieGenre = new MovieGenre();
                    movieGenre.ImdbId = movieDTO.ImdbId;
                    movieGenre.GenreId = genre.GenreId;
                    context.MovieGenres.Add(movieGenre);     
                }
            }
            if(movieDTO.MovieLanguages != null)
            {
                foreach (var movieLanguageName in movieDTO.MovieLanguages)
                {
                    Language language = context.Languages.FirstOrDefault(l => l.LanguageName == movieLanguageName);
                    
                    MovieLanguage movieLanguage = new MovieLanguage();
                    movieLanguage.ImdbId = movieDTO.ImdbId;
                    movieLanguage.LanguageId = language.LanguageId;
                    context.MovieLanguages.Add(movieLanguage);                    
                }
            }
            context.SaveChanges();
            return true;
        }

        private static async Task<bool> ProcessWordInDb(string word, string movieId, Cinephiliacs_MovieContext context)
        {
            Word dbWord = context.Words.FirstOrDefault(w => w.Word1 == word);
            if(dbWord != null)
            {
                if(dbWord.IsTag)
                {
                    var movieTagUser = new MovieTagUser();
                    movieTagUser.ImdbId = movieId;
                    movieTagUser.UserId = "~AutoGenerated";
                    movieTagUser.TagName = dbWord.BaseWord;
                    movieTagUser.IsUpvote = true;
                    
                    if(context.MovieTagUsers.FirstOrDefault(mtu => mtu.ImdbId == movieTagUser.ImdbId
                        && mtu.TagName == movieTagUser.TagName && mtu.UserId == movieTagUser.UserId) == null)
                    {
                        var movieTag = context.MovieTags.FirstOrDefault(mt => mt.ImdbId == movieTagUser.ImdbId
                            && mt.TagName == movieTagUser.TagName);
                        
                        if(movieTag == null)
                        {
                            movieTag = new MovieTag();
                            movieTag.ImdbId = movieTagUser.ImdbId;
                            movieTag.TagName = movieTagUser.TagName;
                            movieTag.VoteSum = 0;
                            context.MovieTags.Add(movieTag);
                        }

                        if(movieTag.VoteSum < int.MaxValue)
                            movieTag.VoteSum += 1;

                        context.MovieTagUsers.Add(movieTagUser);
                        context.SaveChanges();
                    }
                }
            }
            return true;
        }
    }
}