using Android.Arch.Lifecycle;
using Android.OS;
using Android.Runtime;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;

namespace Movolira {
    public class TMDBController{
        public TMDBController() {
            http_cache = new Dictionary<string, JObject>();
            HttpClient http = new HttpClient();
            http.MaxResponseContentBufferSize = 256000;
            Uri request_uri = new Uri("https://api.themoviedb.org/3/configuration?api_key=" + ApiKeys.TMDB_KEY);
            var request = http.GetAsync(request_uri).Result;
            if(request.IsSuccessStatusCode) {
                var data = request.Content.ReadAsStringAsync().Result;
                JObject json = JObject.Parse(data);
                JToken result = json["images"];
                this.base_url = result["base_url"].Value<string>();
                IList<JToken> json_backdrop_sizes = result["backdrop_sizes"].Children().ToList();
                this.backdrop_sizes = new List<string>();
                foreach(JToken backdrop_size in json_backdrop_sizes) {
                    backdrop_sizes.Add(backdrop_size.Value<string>());
                }
            } else {
                //TMDB SERVER FAILED
            }
            request_uri = new Uri("https://api.themoviedb.org/3/genre/movie/list?api_key=" + ApiKeys.TMDB_KEY);
            request = http.GetAsync(request_uri).Result;
            if (request.IsSuccessStatusCode) {
                var data = request.Content.ReadAsStringAsync().Result;
                JObject json = JObject.Parse(data);
                IList<JToken> json_genres = json["genres"].Children().ToList();
                this.genres = new Dictionary<int, string>();
                foreach(JToken genre in json_genres) {
                    genres.Add(genre["id"].Value<int>(), genre["name"].Value<string>());
                }
            } else {
                //TMDB SERVER FAILED
            }
        }
        [JsonConstructor]
        public TMDBController(string base_url, List<string> backdrop_sizes, Dictionary<int, string> genres, 
                Dictionary<string, JObject> http_cache) {
            this.base_url = base_url;
            this.backdrop_sizes = backdrop_sizes;
            this.genres = genres;
            this.http_cache = http_cache;
        }
        public List<CardMovie> getPopularMovies() {
            JObject json_data;
            if (!http_cache.TryGetValue("popular", out json_data)) {
                HttpClient http = new HttpClient();
                http.MaxResponseContentBufferSize = 256000;
                Uri request_uri = new Uri("https://api.themoviedb.org/3/discover/movie?api_key=" + ApiKeys.TMDB_KEY + "&sort_by=popularity.desc");
                HttpResponseMessage request = http.GetAsync(request_uri).Result;
                if (request.IsSuccessStatusCode) {
                    var data = request.Content.ReadAsStringAsync().Result;
                    json_data = JObject.Parse(data);
                    http_cache.Add("popular", json_data);
                } else {
                    //TMDB SERVER FAILED
                    return null;
                }
            }
            IList<JToken> results = json_data["results"].Children().ToList();
            List<CardMovie> movie_data = new List<CardMovie>();
            foreach (JToken result in results) {
                string backdrop_path = base_url + backdrop_sizes[1] + result["backdrop_path"].Value<string>();
                string title = result["title"].Value<string>();
                double rating = result["vote_average"].Value<double>();
                string genre_text = "";
                IList<int> genre_ids = result["genre_ids"].Select(x => (int)x).ToList();
                for (int i_genre_ids = 0; i_genre_ids < genre_ids.Count; ++i_genre_ids) {
                    genre_text += genres[genre_ids[i_genre_ids]];
                    if (i_genre_ids + 1 < genre_ids.Count) {
                        genre_text += ", ";
                    }
                }
                CardMovie card_movie = new CardMovie(backdrop_path, title, genre_text, rating);
                movie_data.Add(card_movie);
            }
            return movie_data;
        }
        public string base_url { get; private set; }
        public List<string> backdrop_sizes { get; private set; }
        public Dictionary<int, string> genres { get; private set; }
        public Dictionary<string, JObject> http_cache { get; private set; }
    }
}