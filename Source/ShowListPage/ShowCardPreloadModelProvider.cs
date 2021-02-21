using System.Collections.Generic;
using Android.Graphics;
using Android.Graphics.Drawables;
using Bumptech.Glide;
using Bumptech.Glide.Load.Resource.Drawable;
using Bumptech.Glide.Request;
using Java.Lang;
using Java.Util;
using IList = System.Collections.IList;




namespace Movolira {
	public class ShowCardPreloadModelProvider : Object, ListPreloader.IPreloadModelProvider {
		public List<Show> Shows { get; set; }
		private readonly MainActivity _main_activity;




		public ShowCardPreloadModelProvider(List<Show> shows, MainActivity main_activity) {
			this.Shows = shows;
			this._main_activity = main_activity;
		}




		public IList GetPreloadItems(int position) {
			if (position >= this.Shows.Count) {
				return Collections.EmptyList();
			}


			string poster_url = this.Shows[position].PosterUrl;
			if (poster_url == "") {
				return Collections.EmptyList();
			}
			return Collections.SingletonList(poster_url);
		}




		public RequestBuilder GetPreloadRequestBuilder(Object poster_url) {
			RequestOptions image_load_options = new RequestOptions()
				.Placeholder(new ColorDrawable(Color.Black)).CenterCrop();
			RequestOptions thumbnail_options = new RequestOptions().CenterCrop();


			return Glide.With(this._main_activity).Load(poster_url).Apply(image_load_options)
				.Transition(DrawableTransitionOptions.WithCrossFade())
				.Thumbnail(Glide.With(this._main_activity).Load(((string)poster_url)
				.Replace("/fanart/", "/preview/")).Apply(thumbnail_options)
				.Transition(DrawableTransitionOptions.WithCrossFade()));
		}
	}
}