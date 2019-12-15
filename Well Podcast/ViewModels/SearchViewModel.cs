using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Well_Podcast.ViewModels
{
    public class SearchPodcast : Common.BindableBase
    {
        private string m_FeedId;
        public string FeedId
        {
            get { return m_FeedId; }
            set { Set(ref m_FeedId, value); }
        }

        private string m_ArtistName;
        public string ArtistName
        {
            get { return m_ArtistName; }
            set { Set(ref m_ArtistName, value); }
        }

        private string m_Name;
        public string Name
        {
            get { return m_Name; }
            set { Set(ref m_Name, value); }
        }

        private bool m_IsExplicit;
        public bool IsExplicit
        {
            get { return m_IsExplicit; }
            set { Set(ref m_IsExplicit, value); }
        }

        private string m_ArtworkUrl100;
        public string ArtworkUrl100
        {
            get { return m_ArtworkUrl100; }
            set { Set(ref m_ArtworkUrl100, value); }
        }

        private string m_GenreId;
        public string GenreId
        {
            get { return m_GenreId; }
            set { Set(ref m_GenreId, value); }
        }

        private string m_ReleaseDate;
        public string ReleaseDate
        {
            get { return m_ReleaseDate; }
            set { Set(ref m_ReleaseDate, value); }
        }

        private string m_FeedUrl;
        public string FeedUrl
        {
            get { return m_FeedUrl; }
            set { Set(ref m_FeedUrl, value); }
        }

        private string m_CollectionViewUrl;
        public string CollectionViewUrl
        {
            get { return m_CollectionViewUrl; }
            set { Set(ref m_CollectionViewUrl, value); }
        }

        private string m_TrackCount;
        public string TrackCount
        {
            get { return m_TrackCount; }
            set { Set(ref m_TrackCount, value); }
        }

        private bool? m_IsSubscribed;
        public bool? IsSubscribed
        {
            get { return m_IsSubscribed; }
            set { Set(ref m_IsSubscribed, value); }
        }
    }
}
