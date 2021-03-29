using System;
using System.Collections.Generic;
using System.Linq;

namespace Movolira {
	public static class Recommendation {
		public static Tuple<Show, string> getRecommendation(MainActivity main_activity) {
			var recommendation_show_pair = getRecommendationBasedOnStarredShows(main_activity, UserDataProvider.getFiveStarShows());
			if (recommendation_show_pair.Item1 == null) {
				recommendation_show_pair = getRecommendationBasedOnStarredShows(main_activity, UserDataProvider.getFourStarShows());
			}
			if (recommendation_show_pair.Item1 == null) {
				recommendation_show_pair = getRecommendationBasedOnPopularShows(main_activity);
			}


			if (recommendation_show_pair.Item1 == null) {
				main_activity.RunOnUiThread(() => {
					main_activity.setIsLoading(false);
					main_activity.showNetworkError();
				});


			} else {
				UserDataProvider.addToAlreadyRecommendedShows(recommendation_show_pair.Item1);
			}
			return recommendation_show_pair;
		}




		private static Tuple<Show, string> getRecommendationBasedOnStarredShows(MainActivity main_activity,
			List<SerializedRatedShow> starred_shows) {


			var recommended_show_pair = new Tuple<Show, string>(null, null);
			Random randomizer = new Random();
			while (starred_shows.Any()) {
				int i_show = randomizer.Next(0, starred_shows.Count - 1);
				SerializedRatedShow five_star_show = starred_shows[i_show];


				recommended_show_pair = getRecommendationFromSimilarShows(main_activity, five_star_show);
				if (recommended_show_pair.Item1 != null) {
					break;
				}
				starred_shows.RemoveAt(i_show);
			}


			return recommended_show_pair;
		}




		private static Tuple<Show, string> getRecommendationFromSimilarShows(MainActivity main_activity, SerializedRatedShow rated_show) {
			Show recommended_show = null;
			string recommended_show_type = null;
			int page_count = 1; // Default value, used before the proper value gets returned in a query.


			for (int i_page = 1; i_page <= page_count; ++i_page) {
				Tuple<List<Show>, int> similar_show_data = null;
				if (rated_show.Type == ShowType.Movie.ToString()) {
					similar_show_data = main_activity.MovieProvider
						.getMovies(rated_show.Id + "/similar", i_page).Result;
					recommended_show_type = "movie";


				} else {
					similar_show_data = main_activity.TvShowProvider
						.getTvShows(rated_show.Id + "/similar", i_page).Result;
					recommended_show_type = "tv_show";
				}


				if (similar_show_data == null) {
					main_activity.RunOnUiThread(() => {
						main_activity.setIsLoading(false);
						main_activity.showNetworkError();
					});
					return new Tuple<Show, string>(null, null);
				}


				List<Show> shows = similar_show_data.Item1;
				page_count = similar_show_data.Item2 / MainActivity.SHOWS_PER_PAGE;
				recommended_show = getFirstUnknownShowFromList(shows);
				if (recommended_show != null) {
					break;
				}
			}


			return new Tuple<Show, string>(recommended_show, recommended_show_type);
		}




		private static Tuple<Show, string> getRecommendationBasedOnPopularShows(MainActivity main_activity) {
			Show recommended_show = null;
			string recommended_show_type = null;
			int page_count = 1; // Default value, used before the proper value gets returned in a query.
			Random randomizer = new Random();


			for (int i_page = 1; i_page <= page_count; ++i_page) {
				Tuple<List<Show>, int> popular_show_data = null;
				if (randomizer.Next(0, 1) == 0) {
					popular_show_data = main_activity.MovieProvider.getMovies("popular", i_page).Result;
					recommended_show_type = "movie";
				} else {
					popular_show_data = main_activity.TvShowProvider.getTvShows("popular", i_page).Result;
					recommended_show_type = "tv_show";
				}


				if (popular_show_data == null) {
					main_activity.RunOnUiThread(() => {
						main_activity.setIsLoading(false);
						main_activity.showNetworkError();
					});
					return new Tuple<Show, string>(null, null);
				}


				List<Show> shows = popular_show_data.Item1;
				page_count = popular_show_data.Item2 / MainActivity.SHOWS_PER_PAGE;
				recommended_show = getFirstUnknownShowFromList(shows);
				if (recommended_show != null) {
					break;
				}
			}


			return new Tuple<Show, string>(recommended_show, recommended_show_type);
		}




		private static Show getFirstUnknownShowFromList(List<Show> shows) {
			if (shows.Any()) {
				for (int i_similar_shows = 0; i_similar_shows < shows.Count; ++i_similar_shows) {
					if (!UserDataProvider.getKnownShowIds().Contains(shows[i_similar_shows].Id)) {
						return shows[i_similar_shows];
					}
				}
			}


			return null;
		}
	}
}