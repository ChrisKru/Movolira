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
                if(json_backdrop_sizes.Count == 1) {
                    this.backdrop_item_size = json_backdrop_sizes[0].Value<string>();
                    this.backdrop_size = json_backdrop_sizes[0].Value<string>();
                } else {
                    this.backdrop_item_size = json_backdrop_sizes[json_backdrop_sizes.Count / 2 - 1].Value<string>();
                    this.backdrop_size = json_backdrop_sizes[json_backdrop_sizes.Count - 1].Value<string>();
                }
                IList<JToken> json_poster_sizes = result["poster_sizes"].Children().ToList();
                if(json_poster_sizes.Count == 1) {
                    this.poster_size = json_poster_sizes[0].Value<string>();
                } else {
                    this.poster_size = json_poster_sizes[json_poster_sizes.Count / 2].Value<string>();
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
        public TMDBController(string base_url, string backdrop_item_size, string backdrop_size, string poster_size, Dictionary<int, string> genres, 
                Dictionary<string, JObject> http_cache) {
            this.base_url = base_url;
            this.backdrop_item_size = backdrop_item_size;
            this.backdrop_size = backdrop_size;
            this.poster_size = poster_size;
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
                int id = result["id"].Value<int>();
                string backdrop_item_path = base_url + backdrop_item_size + result["backdrop_path"].Value<string>();
                string backdrop_path = base_url + backdrop_size + result["backdrop_path"].Value<string>();
                string poster_path = base_url + poster_size + result["poster_path"].Value<string>();
                string title = result["title"].Value<string>();
                string overview = result["overview"].Value<string>();
                string release_date = result["release_date"].Value<string>();
                double rating = result["vote_average"].Value<double>();
                string genre_text = "";
                IList<int> genre_ids = result["genre_ids"].Select(x => (int)x).ToList();
                for (int i_genre_ids = 0; i_genre_ids < genre_ids.Count; ++i_genre_ids) {
                    genre_text += genres[genre_ids[i_genre_ids]];
                    if (i_genre_ids + 1 < genre_ids.Count) {
                        genre_text += ", ";
                    }
                }
                CardMovie card_movie = new CardMovie(id, backdrop_item_path, backdrop_path, poster_path, title, overview, genre_text, release_date,
                                                     rating);
                movie_data.Add(card_movie);
            }
            return movie_data;
        }
        public DetailedMovie getMovieDetails(int id) {
            JObject json_data;
            if (!http_cache.TryGetValue("movie" + id.ToString(), out json_data)) {
                HttpClient http = new HttpClient();
                http.MaxResponseContentBufferSize = 256000;
                Uri request_uri = new Uri("https://api.themoviedb.org/3/movie/" + id.ToString() + "?api_key=" + ApiKeys.TMDB_KEY 
                                          + "&sort_by=popularity.desc");
                HttpResponseMessage request = http.GetAsync(request_uri).Result;
                if (request.IsSuccessStatusCode) {
                    var data = request.Content.ReadAsStringAsync().Result;
                    json_data = JObject.Parse(data);
                    http_cache.Add("movie" + id.ToString(), json_data);
                } else {
                    //TMDB SERVER FAILED
                    return null;
                }
            }
            string backdrop_item_path = base_url + backdrop_item_size + json_data["backdrop_path"].Value<string>();
            string backdrop_path = base_url + backdrop_size + json_data["backdrop_path"].Value<string>();
            string poster_path = base_url + poster_size + json_data["poster_path"].Value<string>();
            string title = json_data["title"].Value<string>();
            string overview = json_data["overview"].Value<string>();
            string release_date = json_data["release_date"].Value<string>();
            double rating = json_data["vote_average"].Value<double>();
            int runtime_min = json_data["runtime"].Value<int>();
            int runtime_hours = 0;
            while(runtime_min >= 60) {
                ++runtime_hours;
                runtime_min -= 60;
            }
            string runtime_text = runtime_hours.ToString() + "h " + runtime_min.ToString() + "min";
            string genre_text = "";
            IList<JToken> json_genres = json_data["genres"].Children().ToList();
            for (int i_genres = 0; i_genres < json_genres.Count; ++i_genres) {
                int genre_id = json_genres[i_genres]["id"].Value<int>();
                genre_text += genres[genre_id];
                if (i_genres + 1 < json_genres.Count) {
                    genre_text += ", ";
                }
            }
            DetailedMovie movie_data = new DetailedMovie(backdrop_item_path, backdrop_path, poster_path, title, overview, genre_text, release_date, 
                                                        rating, runtime_text);
            return movie_data;
        }
        public string base_url { get; private set; }
        public string backdrop_item_size { get; private set; }
        public string backdrop_size { get; private set; }
        public string poster_size { get; private set; }
        public Dictionary<int, string> genres { get; private set; }
        public Dictionary<string, JObject> http_cache { get; private set; }
    }
}