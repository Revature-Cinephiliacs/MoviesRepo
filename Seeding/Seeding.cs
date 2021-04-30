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

namespace Seeding
{
    public class Seeding
    {
        const int MAX_NUMBER_OF_TASKS = 10;
        const int BATCH_SIZE = 10;
        const int STARTING_LINE = 1200;
        const int MAX_NUMBER_OF_MOVIES = 200;
        readonly static DbContextOptions<Cinephiliacs_MovieContext> dbOptions = new DbContextOptionsBuilder<Cinephiliacs_MovieContext>()
                .UseSqlServer("Server=tcp:cinephiliacs.database.windows.net,1433;Initial Catalog=Cinephiliacs_Movie;"
                + "Persist Security Info=False;User ID=kugelsicher;Password=F36UWevqvcDxEmt;MultipleActiveResultSets="
                + "False;Encrypt=True;TrustServerCertificate=False;Connection Timeout=30;").Options;
        
        public async static Task<bool> SeedDbFromCSV(string path)
        {
            int limitCounter = 0;

            var movieDTOs = new List<MovieDTO>();
            var seedTaskList = new List<Task<MovieDTO>>();

            StreamReader reader = new StreamReader(path);
            for (int i = 0; i < STARTING_LINE; i++)
            {
                reader.ReadLine();
            }

            try
            {
                do
                {
                    if(movieDTOs.Count >= BATCH_SIZE)
                    {
                        AddMovies(movieDTOs);
                        movieDTOs.Clear();
                    }
                    else
                    {
                        var movieId = reader.ReadLine();
                        movieId = movieId.Trim();

                        seedTaskList.Add(GetMovie(movieId));

                        limitCounter++;
                        while(seedTaskList.Count >= MAX_NUMBER_OF_TASKS)
                        {
                            var completedTask = await Task<MovieDTO>.WhenAny(seedTaskList);
                            movieDTOs.Add(completedTask.Result);
                            seedTaskList.Remove(completedTask);
                        }
                    }
                }
                while(reader.Peek()!= -1 && limitCounter < MAX_NUMBER_OF_MOVIES);

            }
            finally
            {
                reader.Close();
            }
            while(seedTaskList.Count > 0)
            {
                var completedTask = await Task<MovieDTO>.WhenAny(seedTaskList);
                movieDTOs.Add(completedTask.Result);
                seedTaskList.Remove(completedTask);
            }
            AddMovies(movieDTOs);
            return true;
        }
        
        public static async Task<MovieDTO> GetMovie(string movieId)
        {
            Logic.ApiHelper.MovieObject movieObject = await Logic.ApiHelper.PublicAPIProcessor
                .LoadMovieAsync(movieId);
            if(movieObject == null)
            {
                return null;
            }
            MovieDTO newMovieDTO = Mapper.MovieObjectToMovieDTO(movieObject);
            return newMovieDTO;
        }

        public static async void AddMovies(List<MovieDTO> movieDTOs)
        {
            // Use temporary object lists to accumulate all of the
            // Actors, Directors, Genres, Languages, and Tags that exist
            // in the movies to be added. These need to be added ahead of
            // time; so that the remainder of the process can be parallelized
            var actors = new List<Actor>();
            var directors = new List<Director>();
            var genres = new List<Genre>();
            var languages = new List<Language>();
            var tags = new List<Tag>();

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
                foreach (var movieTag in movieDTOs[i].MovieTags)
                {
                    tags.Add(new Tag() {TagName = movieTag});
                }
            }

            // Remove duplicate entries
            actors = actors.GroupBy(a => a.ActorName).Select(a => a.First()).ToList<Actor>();
            directors = directors.GroupBy(d => d.DirectorName).Select(a => a.First()).ToList<Director>();
            genres = genres.GroupBy(g => g.GenreName).Select(a => a.First()).ToList<Genre>();
            languages = languages.GroupBy(l => l.LanguageName).Select(a => a.First()).ToList<Language>();
            tags = tags.GroupBy(t => t.TagName).Select(a => a.First()).ToList<Tag>();

            using (var context = new Cinephiliacs_MovieContext(dbOptions))
            {
                RepoLogic _repo = new RepoLogic(context);

                Console.WriteLine("Adding Entities--------------");

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
                foreach (var tag in tags)
                {
                    if(!_repo.TagExists(tag.TagName))
                    {
                        context.Tags.Add(tag);
                    }
                }
                context.SaveChanges();
            }

            var addMovieTaskList = new List<Task>();

            foreach (var movieDTO in movieDTOs)
            {
                addMovieTaskList.Add(AddMovie(movieDTO));
            }

            while(addMovieTaskList.Count > 0)
            {
                var completedTask = await Task.WhenAny(addMovieTaskList);
                addMovieTaskList.Remove(completedTask);
            }
        }

        public static async Task AddMovie(MovieDTO movieDTO)
        {
            await Task.Run(() => {
                using (var context = new Cinephiliacs_MovieContext(dbOptions))
                {
                    RepoLogic _repo = new RepoLogic(context);
                    IMovieLogic _logic = new MovieLogic(_repo);

                    if(movieDTO != null)
                    {
                        if(!_repo.MovieExists(movieDTO.ImdbId))
                        {
                            if(_logic.CreateMovie(movieDTO))
                            {
                                Console.WriteLine(movieDTO.ImdbId + ": Success!");
                            }
                            else
                            { 
                                Console.WriteLine(movieDTO.ImdbId + ": Creation failed!");
                            }
                        }
                        else
                        {
                            Console.WriteLine(movieDTO.ImdbId + ": Movie already exists!");
                        }
                    }
                    else
                    {
                        Console.WriteLine(movieDTO.ImdbId + ": Movie not found!");
                    }
                }
            });
        }
    }
}