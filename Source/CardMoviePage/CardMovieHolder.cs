using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.Media;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;

namespace Movolira {
    class CardMovieHolder : RecyclerView.ViewHolder{
        public CardMovieHolder(View view, Action<int> listener) : base (view){
            view.Click += (sender, position) => listener(base.LayoutPosition);
            this.backdrop_view = view.FindViewById<ImageView>(Resource.Id.card_movie_backdrop);
            this.title_view = view.FindViewById<TextView>(Resource.Id.card_movie_title);
            this.genres_view = view.FindViewById<TextView>(Resource.Id.card_movie_genres);
            this.rating_view = view.FindViewById<TextView>(Resource.Id.card_movie_rating);
        }
        public ImageView backdrop_view { get; private set; }
        public TextView title_view { get; private set; }
        public TextView genres_view { get; private set; }
        public TextView rating_view { get; private set; }
    }
}