using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Logic.ApiHelper
{
    public class MovieProcessor
    {
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
                    { "x-rapidapi-key", "e157b8d687msh431e30623e70dd3p174a1cjsn7ea0d090c0f9" },
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
    }
}
