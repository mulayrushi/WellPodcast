using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Well_Podcast.Models.Apple.TopPodcast
{
    public class Author
    {
        public string name { get; set; }
        public string uri { get; set; }
    }

    public class Link
    {
        public string self { get; set; }
        public string alternate { get; set; }
    }

    public class Genre
    {
        public string genreId { get; set; }
        public string name { get; set; }
        public string url { get; set; }
    }

    public class Result
    {
        public string artistName { get; set; }
        public string id { get; set; }
        public string releaseDate { get; set; }
        public string name { get; set; }
        public string kind { get; set; }
        public string copyright { get; set; }
        public string artistId { get; set; }
        public string contentAdvisoryRating { get; set; }
        public string artistUrl { get; set; }
        public string artworkUrl100 { get; set; }
        public List<Genre> genres { get; set; }
        public string url { get; set; }
    }

    public class Feed
    {
        public string title { get; set; }
        public string id { get; set; }
        public Author author { get; set; }
        public List<Link> links { get; set; }
        public string copyright { get; set; }
        public string country { get; set; }
        public string icon { get; set; }
        public string updated { get; set; }
        public List<Result> results { get; set; }
    }

    public class TopFeed
    {
        public Feed feed { get; set; }
    }
}
