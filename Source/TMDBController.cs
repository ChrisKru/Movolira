using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
//****************
//Develop guards for tmdb server failure
//****************
namespace Movolira {
    public class TMDBController {
        public TMDBController() {
            HttpClient http = new HttpClient();
            http.MaxResponseContentBufferSize = 256000;
            Uri request_uri = new Uri("https://api.themoviedb.org/3/configuration?api_key=" + ApiKeys.TMDB_KEY);
            var request = http.GetAsync(request_uri).Result;
            if(request.IsSuccessStatusCode) {
                var data = request.Content.ReadAsStringAsync().Result;
                JObject json = JObject.Parse(data);
                JToken result = json["images"];
                this.base_url = result["base_url"].Value<string>();
                //IList<JToken> json_backdrop_sizes = result["backdrop_sizes"].Children().ToList();
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
        public List<CardMovie> getPopularMovies() {
            HttpClient http = new HttpClient();
            http.MaxResponseContentBufferSize = 256000;
            Uri request_uri = new Uri("https://api.themoviedb.org/3/discover/movie?api_key=" + ApiKeys.TMDB_KEY + "&sort_by=popularity.desc");
            var request = http.GetAsync(request_uri).Result;
            if(request.IsSuccessStatusCode) {
                var data = request.Content.ReadAsStringAsync().Result;
                JObject json = JObject.Parse(data);
                IList<JToken> results = json["results"].Children().ToList();
                List<CardMovie> movie_data = new List<CardMovie>();
                foreach(JToken result in results) {
                    string backdrop_path = base_url + backdrop_sizes[1] + result["backdrop_path"].Value<string>();
                    string title = result["title"].Value<string>();
                    double rating = result["vote_average"].Value<double>();
                    string genre_text = "";
                    IList<int> genre_ids = result["genre_ids"].Select(x => (int)x).ToList();
                    for(int i_genre_ids = 0; i_genre_ids < genre_ids.Count; ++i_genre_ids) {
                        genre_text += genres[genre_ids[i_genre_ids]];
                        if(i_genre_ids + 1 < genre_ids.Count) {
                            genre_text += ", ";
                        }
                    }
                    /*IList<JToken> genres = result["genres"].Value<IList<JToken>>();
                    for(int i_genres = 0; i_genres < genres.Count; ++i_genres) {
                        genre_text += genres[i_genres]["name"].Value<string>();
                        if (i_genres + 1 != genres.Count) {
                            genre_text += ", ";
                        }
                    }*/
                    CardMovie card_movie = new CardMovie(backdrop_path, title, genre_text, rating);
                    movie_data.Add(card_movie);
                }
                return movie_data;
            } else {
                //TMDB SERVER FAILED
                return null;
            }
        }
        public string base_url { get; private set; }
        public List<string> backdrop_sizes { get; private set; }
        public Dictionary<int, string> genres { get; private set; }
    }
}