using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using Android;
using Android.App;
using Android.Content;
using Android.Graphics;
using Android.Graphics.Drawables;
using Android.OS;
using Android.Runtime;
using Android.Support.V7.Widget;
using Android.Views;
using Android.Widget;
using Square.Picasso;
//****************
//Develop guards for tmdb server failure
//****************
namespace Movolira {
    class CardMovieAdapter : RecyclerView.Adapter{
        public CardMovieAdapter(List<CardMovie> movie_data, Context context) {
            this.movie_data = movie_data;
            this.context = context;
        }
        public override RecyclerView.ViewHolder OnCreateViewHolder(ViewGroup parent_view, int view_type) {
            View view = LayoutInflater.From(parent_view.Context).Inflate(Resource.Layout.card_movie_item, parent_view, false);
            CardMovieHolder holder = new CardMovieHolder(view, onClick);
            return holder;
        }
        public override void OnBindViewHolder(RecyclerView.ViewHolder holder, int position) {
            CardMovieHolder card_holder = holder as CardMovieHolder;
            card_holder.title_view.Text = movie_data[position].title;
            card_holder.genres_view.Text = movie_data[position].genres;
            double rating = movie_data[position].rating;
            card_holder.rating_view.Text = rating.ToString();         
            GradientDrawable rating_background = (GradientDrawable)card_holder.rating_view.Background;
            if(rating < 2) {
                rating_background.SetColor(Android.Support.V4.Content.ContextCompat.GetColorStateList(context, Resource.Color.rating_tragic));
            }else if(rating < 4) {
                rating_background.SetColor(Android.Support.V4.Content.ContextCompat.GetColorStateList(context, Resource.Color.rating_bad));
            }else if(rating < 7) {
                rating_background.SetColor(Android.Support.V4.Content.ContextCompat.GetColorStateList(context, Resource.Color.rating_average));
            }else if(rating < 9.5) {
                rating_background.SetColor(Android.Support.V4.Content.ContextCompat.GetColorStateList(context, Resource.Color.rating_good));
            }else {
                rating_background.SetColor(Android.Support.V4.Content.ContextCompat.GetColorStateList(context, Resource.Color.rating_godlike));
            }
            Picasso.With(context).Load(movie_data[position].backdrop_item_path).Into(card_holder.backdrop_view);
        }
        private void onClick(int position) {
            if(click_handler != null) {
                click_handler(this, position);
            }
        }
        public override int ItemCount { get { return movie_data.Count(); } }
        public List<CardMovie> movie_data { get; private set; }
        public event EventHandler<int> click_handler;
        private Context context;
    }
}