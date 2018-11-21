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

namespace Movolira {
    public class CardMovie {
        public CardMovie(string backdrop_path, string title, string genres, double rating) {
            this.backdrop_path = backdrop_path;
            this.title = title;
            this.genres = genres;
            this.rating = rating;
        }
        public string backdrop_path { get; private set; }
        public string title { get; private set; }
        public string genres { get; private set; }
        public double rating { get; private set; }
    }
}