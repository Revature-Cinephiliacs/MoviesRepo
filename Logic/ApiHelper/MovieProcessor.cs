using System;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace Logic.ApiHelper
{
    public class MovieProcessor
    {
        public static async Task<MovieObject> LoadMovieAsync( string searchMovie)
        {
            var client = new HttpClient();
            client.DefaultRequestHeaders.Accept.Clear();
            client.DefaultRequestHeaders.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

            var request = new HttpRequestMessage
            {
                Method = HttpMethod.Get,
                RequestUri = new Uri($"https://movie-database-imdb-alternative.p.rapidapi.com/?i= {searchMovie}"),
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
    }
}
