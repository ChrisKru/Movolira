using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Support.V4.App;
using Android.Util;
using Android.Views;
using Android.Widget;
using Newtonsoft.Json;
using Square.Picasso;

//backdrop 
//poster 
//title 
//genres 
//release date
//rating 
//overview

namespace Movolira {
    public class MovieDetailsFragment : Fragment {
        public override void OnCreate(Bundle savedInstanceState) {
            movie_data = JsonConvert.DeserializeObject<CardMovie>(Arguments.GetString("movie_data"));
            base.OnCreate(savedInstanceState);
        }
        public override View OnCreateView(LayoutInflater inflater, ViewGroup container, Bundle savedInstanceState) {
            View layout = inflater.Inflate(Resource.Layout.movie_details, container, false);
            ImageView backdrop = layout.FindViewById<ImageView>(Resource.Id.movie_details_backdrop);
            ImageView poster = layout.FindViewById<ImageView>(Resource.Id.movie_details_poster);
            Picasso.With(Activity).Load(movie_data.backdrop_path).Into(backdrop);
            Picasso.With(Activity).Load(movie_data.poster_path).Into(poster);
            return layout;
        }
        CardMovie movie_data;
    }
}