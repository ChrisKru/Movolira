using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;

namespace Movolira{
    public class DetailedMovie{
        [JsonConstructor]
        public DetailedMovie(string backdrop_item_path, string backdrop_path, string poster_path, string title, string overview, string genres,
                string release_date, double rating, string runtime) {
            this.backdrop_item_path = backdrop_item_path;
            this.backdrop_path = backdrop_path;
            this.poster_path = poster_path;
            this.title = title;
            this.overview = overview;
            this.genres = genres;
            this.release_date = release_date;
            this.rating = rating;
            this.runtime = runtime;
        }
        public string backdrop_item_path { get; private set; }
        public string backdrop_path { get; private set; }
        public string poster_path { get; private set; }
        public string title { get; private set; }
        public string overview { get; private set; }
        public string genres { get; private set; }
        public string release_date { get; private set; }
        public double rating { get; private set; }
        public string runtime { get; private set; }
    }
}