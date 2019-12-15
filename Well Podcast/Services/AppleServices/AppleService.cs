using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Well_Podcast.Common;

namespace Well_Podcast.Services.AppleServices
{
    public class AppleService
    {
        public static async Task<Models.Apple.TopPodcast.TopFeed> GetTopFeed(string region)
        {
            var jsonString = await DatabaseHelper.GetJsonString("https://rss.itunes.apple.com/api/v1/" + region + "/podcasts/top-podcasts/all/100/explicit.json");
            var json = DatabaseHelper.Deserialize<Models.Apple.TopPodcast.TopFeed>(jsonString);
            return json;
        }

        public static async Task<Models.Apple.Search.Result1> GetFirstPodcast(string term)
        {
            string url = "https://itunes.apple.com/search?term=" + term + "&country=US&media=podcast";
            var jsonString = await DatabaseHelper.GetJsonString(url);
            var json = DatabaseHelper.Deserialize<Models.Apple.Search.PodcastSearch>(jsonString);
            return json.results[0];
        }
    }
}
