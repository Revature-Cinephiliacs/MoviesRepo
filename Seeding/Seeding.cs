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

namespace Seeding
{
    public class Seeding
    {
        const int MAX_GET_MOVIE_TASKS = 10;
        const int MAX_ADD_WORD_TASKS = 10;
        const int MAX_ADD_MOVIE_TASKS = 10;
        const int BATCH_SIZE = 15;
        const int STARTING_LINE = 3400;
        const int MAX_NUMBER_OF_MOVIES = 100;
        readonly static DbContextOptions<Cinephiliacs_MovieContext> dbOptions = new DbContextOptionsBuilder<Cinephiliacs_MovieContext>()
                .UseSqlServer("Server=tcp:cinephiliacs.database.windows.net,1433;Initial Catalog=Cinephiliacs_Movie;"
                + "Persist Security Info=False;User ID=kugelsicher;Password=F36UWevqvcDxEmt;MultipleActiveResultSets="
                + "False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;").Options;
        
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

        public async static Task<bool> SeedDbFromCSV(string path)
        {

            var movieIdList = ReadMovieIdsFromFile(path);

            int limitCounter = 0;
            var getMovieTasks = new List<Task<MovieDTO>>();
            var movieDTOs = new List<MovieDTO>();
            var addMovieBatchTasks = new List<Task>();
            
            for (int index = STARTING_LINE; index < movieIdList.Count
                && limitCounter <= MAX_NUMBER_OF_MOVIES; index++)
            {
                getMovieTasks.Add(GetMovie(movieIdList[index]));

                limitCounter++;
                while(getMovieTasks.Count >= MAX_GET_MOVIE_TASKS)
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
                        addMovieBatchTasks.Remove(completedTask);
                    }
                    Console.WriteLine("Starting Batch with " + movieDTObatch.Count.ToString() + " Movies to Add!");
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
            // Start final batches of AddMovies Tasks
            Console.WriteLine("Starting Final AddMovies Tasks!");
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
                    addMovieBatchTasks.Remove(completedTask);
                }
                Console.WriteLine("Starting Batch with " + movieDTObatch.Count.ToString() + " Movies to Add!");
                addMovieBatchTasks.Add(AddMovies(movieDTObatch));
            }
            
            // Await the final AddMovies Task!
            Console.WriteLine("Await the final AddMovies Task!");
            while (addMovieBatchTasks.Count > 0)
            {
                var completedTask = await Task<List<MovieDTO>>.WhenAny(addMovieBatchTasks);
                addMovieBatchTasks.Remove(completedTask);
            }
  
            return true;
        }
        
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
        public static async Task AddMovies(List<MovieDTO> movieDTOs)
        {
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
                    if(plotWords[j].ToLower() == plotWords[i].ToLower())
                    {
                        plotWords.RemoveAt(i);
                        break;
                    }
                }
            }

            // Process each word, Adding any to the database that don't already exist.
            // Also add any new Tags.
            var addWordTasks = new List<Task>();
            foreach (var word in plotWords)
            {
                addWordTasks.Add(AddWord(word));

                while(addWordTasks.Count >= MAX_ADD_WORD_TASKS)
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

            // Process each Movie.
            var addMovieTaskList = new List<Task<string>>();
            foreach (var movieDTO in movieDTOs)
            {
                addMovieTaskList.Add(AddMovie(movieDTO));

                while(addMovieTaskList.Count >= MAX_ADD_MOVIE_TASKS)
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
                Word dbWord = context.Words.FirstOrDefault(w => w.Word1 == word);
                if(dbWord == null)
                {
                    var wordObject = await Logic.ApiHelper.ApiProcessor.LoadDefinitionAsync(word);

                    bool wordIsTag = ProcessWordObject(word, wordObject, context);

                    if(wordIsTag)
                    {
                        Tag tag = new Tag();
                        tag.TagName = word;
                        tag.IsBanned = false;
                        context.Tags.Add(tag);
                    }
                }
                else
                {
                    if(dbWord.IsTag)
                    {
                        Tag tag = new Tag();
                        tag.TagName = word;
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

                if(movieDTO != null)
                {
                    if(await _logic.CreateMovie(movieDTO))
                    {
                        return movieDTO.ImdbId + ": Success!";
                    }
                    else
                    { 
                        return movieDTO.ImdbId + ": Movie already exists!";
                    }
                }
                else
                {
                    return movieDTO.ImdbId + ": Movie not found!";
                }
            }
        }
    }
}