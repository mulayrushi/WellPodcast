using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Well_Podcast.Common;

namespace Well_Podcast.Services.SyndicationServices
{
    public class SyndicationService
    {
        public async static Task<Models.Apple.Rss.Rss> GetFeed(Uri uri)
        {
            var xmlString = await DatabaseHelper.GetJsonString(uri.ToString());
            var xml = DatabaseHelper.DeserializeObject<Models.Apple.Rss.Rss>(xmlString);
            return xml;
        }
    }
}
