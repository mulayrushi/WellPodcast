using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Well_Podcast.Common;

namespace Well_Podcast.ViewModels
{
    public class Podcast : BindableBase
    {
        private string m_Id;
        public string Id
        {
            get { return m_Id; }
            set { Set(ref m_Id, value); }
        }

        private string m_Author;
        public string Author
        {
            get { return m_Author; }
            set { Set(ref m_Author, value); }
        }

        private string m_Category;
        public string Category
        {
            get { return m_Category; }
            set { Set(ref m_Category, value); }
        }

        private string m_Copyright;
        public string Copyright
        {
            get { return m_Copyright; }
            set { Set(ref m_Copyright, value); }
        }

        private string m_Description;
        public string Description
        {
            get { return m_Description; }
            set { Set(ref m_Description, value); }
        }

        private bool m_Explicit;
        public bool Explicit
        {
            get { return m_Explicit; }
            set { Set(ref m_Explicit, value); }
        }

        private string m_Generator;
        public string Generator
        {
            get { return m_Generator; }
            set { Set(ref m_Generator, value); }
        }

        private string m_Image;
        public string Image
        {
            get { return m_Image; }
            set { Set(ref m_Image, value); }
        }

        private string m_Image2;
        public string Image2
        {
            get { return m_Image2; }
            set { Set(ref m_Image2, value); }
        }

        private string m_Language;
        public string Language
        {
            get { return m_Language; }
            set { Set(ref m_Language, value); }
        }

        private string m_Summary;
        public string Summary
        {
            get { return m_Summary; }
            set { Set(ref m_Summary, value); }
        }

        private string m_Title;
        public string Title
        {
            get { return m_Title; }
            set { Set(ref m_Title, value); }
        }

        private string m_TrackCount;
        public string TrackCount
        {
            get { return m_TrackCount; }
            set { Set(ref m_TrackCount, value); }
        }

        private string m_FeedUrl;
        public string FeedUrl
        {
            get { return m_FeedUrl; }
            set { Set(ref m_FeedUrl, value); }
        }

        private string m_SubscribedText;
        public string SubscribedText
        {
            get { return m_SubscribedText; }
            set { Set(ref m_SubscribedText, value); }
        }

        private bool? m_IsSubscribed;
        public bool? IsSubscribed
        {
            get { return m_IsSubscribed; }
            set { Set(ref m_IsSubscribed, value); }
        }

        public Podcast()
        {
            feeds = new ObservableCollection<Feeds>();
        }

        public ObservableCollection<Feeds> feeds { get; private set; }
    }

    public class Feeds : BindableBase
    {
        private string m_Id;
        public string Id
        {
            get { return m_Id; }
            set { Set(ref m_Id, value); }
        }

        private string m_Description;
        public string Description
        {
            get { return m_Description; }
            set { Set(ref m_Description, value); }
        }

        private string m_Duration;
        public string Duration
        {
            get { return m_Duration; }
            set { Set(ref m_Duration, value); }
        }

        private string m_EnclosureType;
        public string EnclosureType
        {
            get { return m_EnclosureType; }
            set { Set(ref m_EnclosureType, value); }
        }

        private string m_Encoded;
        public string Encoded
        {
            get { return m_Encoded; }
            set { Set(ref m_Encoded, value); }
        }

        private string m_Episode;
        public string Episode
        {
            get { return m_Episode; }
            set { Set(ref m_Episode, value); }
        }

        private string m_Explicit;
        public string Explicit
        {
            get { return m_Explicit; }
            set { Set(ref m_Explicit, value); }
        }

        private string m_Guid;
        public string Guid
        {
            get { return m_Guid; }
            set { Set(ref m_Guid, value); }
        }

        private string m_Image;
        public string Image
        {
            get { return m_Image; }
            set { Set(ref m_Image, value); }
        }

        private string m_PubDate;
        public string PubDate
        {
            get { return m_PubDate; }
            set { Set(ref m_PubDate, value); }
        }

        private string m_Summary;
        public string Summary
        {
            get { return m_Summary; }
            set { Set(ref m_Summary, value); }
        }

        private string m_Title;
        public string Title
        {
            get { return m_Title; }
            set { Set(ref m_Title, value); }
        }

        private string m_EnclosureUrl;
        public string EnclosureUrl
        {
            get { return m_EnclosureUrl; }
            set { Set(ref m_EnclosureUrl, value); }
        }

        private string m_EnclosureLength;
        public string EnclosureLength
        {
            get { return m_EnclosureLength; }
            set { Set(ref m_EnclosureLength, value); }
        }

        private bool m_IsDownloaded;
        public bool IsDownloaded
        {
            get { return m_IsDownloaded; }
            set { Set(ref m_IsDownloaded, value); }
        }

        private int m_DownloadPercentage;
        public int DownloadPercentage
        {
            get { return m_DownloadPercentage; }
            set { Set(ref m_DownloadPercentage, value); }
        }

        private bool m_IsNotPlaying;
        public bool IsNotPlaying
        {
            get { return m_IsNotPlaying; }
            set { Set(ref m_IsNotPlaying, value); }
        }

        private double m_PlayPercentage;
        public double PlayPercentage
        {
            get { return m_PlayPercentage; }
            set { Set(ref m_PlayPercentage, value); }
        }
    }

    public class PlayList : BindableBase
    {
        private string m_AlbumName;
        public string AlbumName
        {
            get { return m_AlbumName; }
            set { Set(ref m_AlbumName, value); }
        }

        private string m_ArtistName;
        public string ArtistName
        {
            get { return m_ArtistName; }
            set { Set(ref m_ArtistName, value); }
        }

        private string m_SongName;
        public string SongName
        {
            get { return m_SongName; }
            set { Set(ref m_SongName, value); }
        }

        private string m_SongImageUrl;
        public string SongImageUrl
        {
            get { return m_SongImageUrl; }
            set { Set(ref m_SongImageUrl, value); }
        }

        private string m_SongUrl;
        public string SongUrl
        {
            get { return m_SongUrl; }
            set { Set(ref m_SongUrl, value); }
        }

        private bool m_IsNotPlaying;
        public bool IsNotPlaying
        {
            get { return m_IsNotPlaying; }
            set { Set(ref m_IsNotPlaying, value); }
        }

        private double m_PlayPercentage;
        public double PlayPercentage
        {
            get { return m_PlayPercentage; }
            set { Set(ref m_PlayPercentage, value); }
        }
    }

}
