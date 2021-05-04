using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;
using Model;
using Newtonsoft.Json;

namespace Logic.ApiHelper
{
    public class ApiProcessor
    {
        private static readonly string _userapi = "http://20.45.2.119/user/";
        /// <summary>
        /// Retrieves the details for a movie based on an IMDB identification number, imdbId,
        /// from a public API endpoint. Returns null if the imdbId is not found.
        /// </summary>
        /// <param name="searchMovie"></param>
        /// <returns></returns>
        public static async Task<MovieObject> LoadMovieAsync(string imdbId)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://movie-database-imdb-alternative.p.rapidapi.com/?i= {imdbId}"),
                Headers =
                {
                    { "x-rapidapi-key", "e157b8d687msh431e30623e70dd3p174a1cjsn7ea0d090c0f9" },
                    { "x-rapidapi-host", "movie-database-imdb-alternative.p.rapidapi.com" },
                },
            };
            MovieObject movieObject;
            using(var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                movieObject = await response.Content.ReadFromJsonAsync<MovieObject>();
            }
            if(String.IsNullOrEmpty(movieObject.imdbID))
            {
                return null;
            }
            return movieObject;
        }

        /// <summary>
        /// Retrieves the a list of movie recommendations based on an imdbId.
        /// </summary>
        /// <param name="imdbId"></param>
        /// <returns></returns>
        public static async Task<List<string>> LoadRecommendedMovies(string imdbId)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://imdb8.p.rapidapi.com/title/get-more-like-this?tconst={imdbId}&currentCountry=US&purchaseCountry=US"),
                Headers =
                {
                    { "x-rapidapi-key", "6d0ccdf5b5msh34c1c1dd38ee7f3p126f2cjsn773549c669d8" },
                    { "x-rapidapi-host", "imdb8.p.rapidapi.com" },
                },
            };
            using (var response = await client.SendAsync(request))
            {
                response.EnsureSuccessStatusCode();
                var movieRecommended = await response.Content.ReadFromJsonAsync<List<string>>();
                return movieRecommended;
            }
        }

        /// <summary>
        /// Retrieves a word's definition(s) and the part-of-speech associated with each.
        /// </summary>
        /// <param name="word"></param>
        /// <returns></returns>
        public static async Task<WordObject> LoadDefinitionAsync(string word)
        {
            var client = new HttpClient();
            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://wordsapiv1.p.rapidapi.com/words/{word}/definitions"),
                Headers =
                {
                    { "x-rapidapi-key", "6d0ccdf5b5msh34c1c1dd38ee7f3p126f2cjsn773549c669d8" },
                    { "x-rapidapi-host", "wordsapiv1.p.rapidapi.com" },
                },
            };
            using (var response = await client.SendAsync(request))
            {
                if(response.StatusCode != System.Net.HttpStatusCode.OK)
                {
                    return null;
                }
                response.EnsureSuccessStatusCode();
                var wordObject = await response.Content.ReadFromJsonAsync<WordObject>();
                return wordObject;
            }
        }

        /// <summary>
        /// Sends the forum notification on to Users with the list of users who follow the movie the new forum topic belongs to.
        /// </summary>
        /// <param name="forumNotification"></param>
        /// <returns></returns>
        public static async Task<bool> SendForumNotification(ForumNotification forumNotification)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            HttpClient client = new HttpClient(clientHandler);
            string path = $"{_userapi}notification/discussion";
            HttpResponseMessage response = await client.PostAsJsonAsync(path, forumNotification);
            if(response.IsSuccessStatusCode)
            {   
                return true;
            }
            else
            {
                return false;
            }
            
        }

        /// <summary>
        /// Sends the review notification on to Users, with the list of users who follow the movie the new movie review belongs to.
        /// </summary>
        /// <param name="reviewNotification"></param
        /// <returns></returns>
        public static async Task<bool> SendReviewNotification(ReviewNotification reviewNotification)
        {
            HttpClientHandler clientHandler = new HttpClientHandler();
            clientHandler.ServerCertificateCustomValidationCallback = (sender, cert, chain, sslPolicyErrors) => { return true; };
            HttpClient client = new HttpClient(clientHandler);
            string path = $"{_userapi}notification/review";
            var json = JsonConvert.SerializeObject(reviewNotification);
            var data = new StringContent(json, Encoding.UTF8, "application/json");
            HttpResponseMessage response = await client.PostAsync(path, data);
            if(response.IsSuccessStatusCode)
            {   
                return true;
            }
            else
            {
                return false;
            }
        }
    }
}
