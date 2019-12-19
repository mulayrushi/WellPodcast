using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Well_Podcast.Common;
using Well_Podcast.Models.Apple.TopPodcast;
using Well_Podcast.Services;
using Windows.Media;
using Windows.Media.Playback;
using Windows.UI.Xaml.Controls;

namespace Well_Podcast.ViewModels
{
    public class ShellViewModel : BindableBase
    {
        private void Back(object parameter)
        {
            try
            {
                if (NavigationService.Frame.CurrentSourcePageType.Name == "PodcastPage")
                {
                    mediaPlayBackSession.PositionChanged -= MediaPlayBackSession_PositionChanged;
                }
                NavigationService.GoBack();
            }
            catch { }
        }

        private void Hamburger(object parameter)
        {
            _splitView.IsPaneOpen = !_splitView.IsPaneOpen;
        }

        private async void Trending(object parameter)
        {
            try
            {
                NavigationService.Navigate(typeof(Views.TopShowsPage));

                var region = Windows.System.UserProfile.GlobalizationPreferences.HomeGeographicRegion;

                var json = await Services.AppleServices.AppleService.GetTopFeed(region);
                foreach (var result in json.feed.results)
                {
                    var newFeed = new TopShows();
                    newFeed.ArtistName = result.artistName;
                    newFeed.ArtworkUrl100 = result.artworkUrl100;
                    newFeed.IsExplicit = result.contentAdvisoryRating == "Explicit" ? true : false;
                    newFeed.GenreId = result.genres[0].genreId;
                    newFeed.Name = result.name;
                    newFeed.ReleaseDate = result.releaseDate;
                    newFeed.FeedId = result.id;

                    var count = Subscribed.Where(r => r.PodcastId == result.id).Count();
                    if (count > 0)
                    {
                        newFeed.IsSubscribed = true;
                    }

                    topFeeds.Add(newFeed);
                }
            }
            catch { }
        }

        private async void SuggestBoxQuerySubmitted(object parameter)
        {
            try
            {
                var args = parameter as AutoSuggestBoxQuerySubmittedEventArgs;
                var url = "https://itunes.apple.com/search?term=" + args.QueryText + "&media=podcast";
                var searchString = await DatabaseHelper.GetJsonString(url);

                var json = DatabaseHelper.Deserialize<Models.Apple.Search.PodcastSearch>(searchString);

                if (NavigationService.Frame.CurrentSourcePageType.Name != "SearchPage")
                {
                    NavigationService.Navigate(typeof(Views.SearchPage));
                }

                Searches.Clear();

                foreach (var result in json.results)
                {
                    var search = new SearchPodcast();
                    search.ArtistName = result.artistName;
                    search.ArtworkUrl100 = result.artworkUrl600;
                    search.CollectionViewUrl = result.collectionViewUrl;
                    search.IsExplicit = result.contentAdvisoryRating == "Explicit" ? true : false;
                    search.FeedUrl = result.feedUrl;
                    search.Name = result.collectionName;
                    search.TrackCount = result.trackCount.ToString();
                    search.FeedId = result.trackId.ToString();

                    var count = Subscribed.Where(r => r.PodcastId == search.FeedId).Count();
                    if (count > 0)
                    {
                        search.IsSubscribed = true;
                    }
                    Searches.Add(search);
                }
            }
            catch { }
        }

        private async void TopPodcastsSelectionChanged(object parameter)
        {
            try
            {
                SelectedPodcastChannel = new Podcast();
                string url = null;
                Models.Apple.Search.Result1 json = null;
                
                if(NavigationService.Frame.CurrentSourcePageType.Name != "SearchPage" && parameter == null)
                {
                    json = await Services.AppleServices.AppleService.GetFirstPodcast(SelectedFeed.Name);
                    SelectedPodcastChannel.Id = SelectedFeed.FeedId;
                    url = json.feedUrl;
                    if (SelectedFeed.IsSubscribed == true)
                    {
                        SelectedPodcastChannel.IsSubscribed = true;
                        SelectedPodcastChannel.SubscribedText = "Subscribed";
                    }
                    else
                    {
                        SelectedPodcastChannel.SubscribedText = "Subscribe";
                    }
                }
                else if(NavigationService.Frame.CurrentSourcePageType.Name != "SearchPage" && parameter != null)
                {
                    try
                    {
                        SelectedPodcastChannel.IsSubscribed = true;
                        SelectedPodcastChannel.SubscribedText = "Subscribed";
                        var gridView = parameter as GridView;
                        var selected = (SubscribeViewModel)gridView.SelectedItem;
                        url = selected.FeedUrl;
                    }
                    catch { }
                }
                else
                {
                    url = SelectedSearch.FeedUrl;
                    SelectedPodcastChannel.Id = SelectedSearch.FeedId;
                    if (SelectedSearch.IsSubscribed == true)
                    {
                        SelectedPodcastChannel.IsSubscribed = true;
                        SelectedPodcastChannel.SubscribedText = "Subscribed";
                    }
                    else
                    {
                        SelectedPodcastChannel.SubscribedText = "Subscribe";
                    }
                }
                NavigationService.Navigate(typeof(Views.PodcastPage));
                var xml = await Services.SyndicationServices.SyndicationService.GetFeed(new Uri(url));
                SelectedPodcastChannel.FeedUrl = url;
                SelectedPodcastChannel.Author = xml.Channel.Author;
                SelectedPodcastChannel.Title = xml.Channel.Title;
                SelectedPodcastChannel.Category = xml.Channel.Category[0].Text;
                SelectedPodcastChannel.TrackCount = json != null ? json.trackCount.ToString() : xml.Channel.Item.Count().ToString();
                SelectedPodcastChannel.Description = DatabaseHelper.ScrubHtml(xml.Channel.Description);
                SelectedPodcastChannel.Copyright = xml.Channel.Copyright != null ? xml.Channel.Copyright : null;
                SelectedPodcastChannel.Image = xml.Channel.Image != null ? xml.Channel.Image.Href : null;
                SelectedPodcastChannel.Image2 = xml.Channel.Image2 != null ? xml.Channel.Image2.Link2 : "";
                SelectedPodcastChannel.Language = xml.Channel.Language != null ? xml.Channel.Language : null;
                SelectedPodcastChannel.Summary = xml.Channel.Summary;
                Windows.Storage.StorageFolder folder = null;
                try
                {
                    var isPresent = await DatabaseHelper.isFolderPresent(SelectedFeed.FeedId);
                    if (isPresent)
                    {
                        folder = await DatabaseHelper.GetFolder(SelectedPodcastChannel.Id);
                        //folder = roamingFolders.Where(r => r.Name == SelectedFeed.FeedId).First();
                    }
                }
                catch { }

                if (xml.Channel.Explicit != null)
                {
                    if (xml.Channel.Explicit.ToLower() == "yes")
                    {
                        SelectedPodcastChannel.Explicit = true;
                    }
                }
                
                if(xml.Channel.Item.Count > 0)
                {
                    SelectedPodcastChannel.feeds.Clear();
                }
                
                foreach (var item in xml.Channel.Item)
                {
                    try
                    {
                        var feed = new Feeds();
                        feed.Description = item.Description;
                        feed.Duration = item.Duration;
                        feed.EnclosureType = item.Enclosure.Type;
                        feed.EnclosureUrl = item.Enclosure.Url;
                        feed.EnclosureLength = item.Enclosure.Length;
                        feed.Encoded = item.Encoded;
                        feed.Episode = item.Episode;
                        feed.Explicit = item.Explicit;
                        feed.Guid = item.Guid.Text;
                        feed.Image = item.Image != null ? item.Image.Href : null;
                        feed.PubDate = item.PubDate;
                        feed.Summary = item.Summary;
                        feed.Title = DatabaseHelper.ScrubHtml(item.Title[0]);
                        feed.IsNotPlaying = true;
                        
                        var split = item.Guid.Text.Split('/');

                        var count = split.Count();

                        if (count > 0)
                        {
                            feed.Id = split[count - 1];
                        }
                        else
                        {
                            feed.Id = item.Guid.Text;
                        }

                        try
                        {
                            if(folder != null)
                            {
                                var ext = System.IO.Path.GetExtension(feed.EnclosureUrl);
                                if (ext.Length > 4)
                                    ext = ext.Substring(ext.IndexOf('.'), 4);
                                var fileName = System.IO.Path.GetFileNameWithoutExtension(feed.EnclosureUrl) + ext;
                                //var file = await folder.GetFileAsync(fileName);
                                var item1 = await folder.TryGetItemAsync(fileName);
                                if (item1 != null)
                                {
                                    feed.IsDownloaded = true;
                                    feed.DownloadPercentage = 100;
                                }
                            }
                        }
                        catch(Exception ex) { }

                        SelectedPodcastChannel.feeds.Add(feed);
                    }
                    catch { }
                }
            }
            catch (Exception ex) { }
        }

        private void PlayAllPodcast(object parameter)
        {
            try
            {
                foreach (var feed in SelectedPodcastChannel.feeds)
                {
                    var mediaSource = Windows.Media.Core.MediaSource.CreateFromUri(new Uri(feed.EnclosureUrl));
                    var mediaPlaybackItem = new MediaPlaybackItem(mediaSource);

                    _mediaPlaybackList.Items.Add(mediaPlaybackItem);
                    _mediaPlaybackList.CurrentItemChanged += _mediaPlaybackList_CurrentItemChanged;
                    var play = new PlayList();

                    play.AlbumName = SelectedPodcastChannel.Title;
                    play.ArtistName = SelectedPodcastChannel.Author;
                    play.SongName = feed.Title;
                    play.SongImageUrl = feed.Image;
                    play.SongUrl = feed.EnclosureUrl;

                    playLists.Add(play);

                    _mediaPlayer.Source = _mediaPlaybackList;
                    _mediaPlayerElement.MediaPlayer.Play();
                }
            }
            catch { }
        }

        private void ClearAllPodcast(object parameter)
        {
            try
            {
                _mediaPlayerElement.MediaPlayer.Pause();
                _mediaPlayerElement.Source = null;
                _mediaPlaybackList.Items.Clear();
                playLists.Clear();
            }
            catch { }
        }

        private async void PlaySinglePodcast(object parameter)
        {
            try
            {
                _mediaPlayer.Pause();

                MediaPlayer_CurrentStateChanged(_mediaPlayer, parameter);

                SelectedPodcastItem = parameter as Feeds;

                var url = SelectedPodcastItem.Image != null ? SelectedPodcastItem.Image : SelectedPodcastChannel.Image;
                Windows.Media.Core.MediaSource mediaSource;

                var updater = _systemMediaTransportControls.DisplayUpdater;

                updater.Type = MediaPlaybackType.Music;
                updater.MusicProperties.Title = SelectedPodcastItem.Title;
                //updater.MusicProperties.AlbumArtist = SelectedPodcastItem.

                if (SelectedPodcastItem.IsDownloaded)
                {
                    try
                    {
                        var folder = await DatabaseHelper.GetFolder(SelectedPodcastChannel.Id);
                        var ext = System.IO.Path.GetExtension(SelectedPodcastItem.EnclosureUrl);
                        if (ext.Length > 4)
                            ext = ext.Substring(ext.IndexOf('.'), 4);
                        var fileName = System.IO.Path.GetFileNameWithoutExtension(SelectedPodcastItem.EnclosureUrl) + ext;
                        var file = await folder.GetFileAsync(fileName);

                        mediaSource = Windows.Media.Core.MediaSource.CreateFromStorageFile(file);
                    }
                    catch (Exception ex)
                    {
                        mediaSource = Windows.Media.Core.MediaSource.CreateFromUri(new Uri(SelectedPodcastItem.EnclosureUrl));
                    }
                }
                else
                {
                    mediaSource = Windows.Media.Core.MediaSource.CreateFromUri(new Uri(SelectedPodcastItem.EnclosureUrl));
                }


                //mediaSource.CustomProperties
                var mediaPlaybackItem = new MediaPlaybackItem(mediaSource);
                //mediaPlaybackItem.
                _mediaPlaybackList.Items.Add(mediaPlaybackItem);
                _mediaPlaybackList.CurrentItemChanged += _mediaPlaybackList_CurrentItemChanged;
                var play = new PlayList();

                play.AlbumName = SelectedPodcastChannel.Title;
                play.ArtistName = SelectedPodcastChannel.Author;
                play.SongName = SelectedPodcastItem.Title;
                play.SongImageUrl = SelectedPodcastItem.Image;
                play.SongUrl = SelectedPodcastItem.EnclosureUrl;
                //var inPlayList = false;
                //var inde = 0;
                //try
                //{
                //    var pl = playLists.Where(r => r.SongUrl == play.SongUrl).First();
                //    inPlayList = true;
                //    inde = playLists.IndexOf(pl);
                //}
                //catch { }

                //if(inPlayList)
                //{
                //    //var bytes = _mediaPlayerE
                //    _mediaPlaybackList.MoveTo((uint)inde);
                //    SelectedPlayListItem = playLists[inde];
                //}
                //else
                //{

                //}

                playLists.Add(play);

                _mediaPlayer.Source = _mediaPlaybackList;

                SelectedPlayListItem = playLists[playLists.IndexOf(play)];

                position = SelectedPodcastChannel.feeds.IndexOf(SelectedPodcastChannel.feeds.Where(X => X == SelectedPodcastItem).FirstOrDefault());
                _mediaPlayerElement.MaxHeight = 100;
                mediaPlayBackSession = _mediaPlayer.PlaybackSession;
                _mediaPlayerElement.MediaPlayer.MediaOpened += MediaPlayer_MediaOpened;
                _mediaPlayerElement.MediaPlayer.CurrentStateChanged += MediaPlayer_CurrentStateChanged;
                mediaPlayBackSession.PositionChanged += MediaPlayBackSession_PositionChanged;

                //if(_mediaPlaybackList.Items.Count > 1)
                //{
                //    _mediaPlaybackList.MoveNext();
                //}

                _mediaPlayerElement.MediaPlayer.Play();



                _mediaPlayerElement.PosterSource = new Windows.UI.Xaml.Media.Imaging.BitmapImage(new Uri(url));
            }
            catch { }
        }

        private void _mediaPlaybackList_CurrentItemChanged(MediaPlaybackList sender, CurrentMediaPlaybackItemChangedEventArgs args)
        {
            
        }

        private void MediaPlayer_CurrentStateChanged(MediaPlayer sender, object args)
        {
            if(sender.CurrentState == MediaPlayerState.Paused && args != null)
            {
                var index = SelectedPodcastChannel.feeds.IndexOf(SelectedPodcastItem);
                SelectedPodcastChannel.feeds[index].IsNotPlaying = true;
                SelectedPodcastChannel.feeds[index].DownloadPercentage = 0;
            }
        }

        private async void MediaPlayBackSession_PositionChanged(MediaPlaybackSession sender, object args)
        {
            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                SelectedPodcastChannel.feeds[position].IsNotPlaying = false;
                SelectedPlayListItem.PlayPercentage = SelectedPodcastChannel.feeds[position].PlayPercentage = ((double)sender.Position.TotalSeconds / (double)sender.NaturalDuration.TotalSeconds) * 100;
            });
        }

        private async void MediaPlayer_MediaOpened(MediaPlayer sender, object args)
        {
            //MediaPlaybackSessionOutputDegradationPolicyState info = sender.PlaybackSession.GetOutputDegradationPolicyState();


            await Windows.ApplicationModel.Core.CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(Windows.UI.Core.CoreDispatcherPriority.Normal, () =>
            {
                SelectedPodcastChannel.feeds[position].PlayPercentage = ((double)sender.Position.TotalSeconds / (double)sender.NaturalDuration.TotalSeconds) * 100;
            });
            
        }

        private void PlayListViewSelectionChanged(object parameter)
        {
            try
            {
                var index = playLists.IndexOf(SelectedPlayListItem);
                _mediaPlaybackList.MoveTo((uint)index);
            }
            catch { }
        }

        private void Subscribe(object parameter)
        {
            try
            {
                if (SelectedPodcastChannel.IsSubscribed == true)
                {
                    var sub = new SubscribeViewModel();
                    sub.ArtistName = SelectedFeed != null ? SelectedFeed.ArtistName : SelectedSearch.ArtistName;
                    sub.PodcastId = SelectedFeed != null ? SelectedFeed.FeedId : SelectedSearch.FeedId;
                    sub.ArtworkUrl100 = SelectedFeed != null ? SelectedFeed.ArtworkUrl100 : SelectedSearch.ArtworkUrl100;
                    sub.FeedUrl = SelectedPodcastChannel.FeedUrl;
                    sub.LastSeen = DateTime.Now;
                    sub.LastTrackDate = DateTime.Parse(SelectedPodcastChannel.feeds[0].PubDate);
                    sub.PodcastName = SelectedFeed != null ? SelectedFeed.Name : SelectedSearch.Name;
                    sub.TrackCount = SelectedPodcastChannel.feeds.Count;
                    Subscribed.Add(sub);
                    var channel = DatabaseHelper.Serialize(Subscribed);
                    DatabaseHelper.WriteTotextFileAsync("Subscribed", channel);
                }
                else
                {
                    var sub = Subscribed.Where(r => r.PodcastId == SelectedFeed.FeedId).First();
                    SelectedPodcastChannel.IsSubscribed = false;
                    SelectedPodcastChannel.SubscribedText = "Subscribe";
                    Subscribed.Remove(sub);
                    var channel = DatabaseHelper.Serialize(Subscribed);
                    DatabaseHelper.WriteTotextFileAsync("Subscribed", channel);
                }
            }
            catch { }
        }

        private async void DownloadSinglePodcast(object parameter)
        {
            try
            {
                SelectedPodcastItem = parameter as Feeds;
                //SelectedPlayListItem.
                var folder = await Windows.Storage.ApplicationData.Current.LocalFolder.CreateFolderAsync(SelectedPodcastChannel.Id, Windows.Storage.CreationCollisionOption.OpenIfExists);
                var ext = System.IO.Path.GetExtension(SelectedPodcastItem.EnclosureUrl);
                if (ext.Length > 4)
                    ext = ext.Substring(ext.IndexOf('.'), 4);
                var fileName = System.IO.Path.GetFileNameWithoutExtension(SelectedPodcastItem.EnclosureUrl) + ext;
                var file = await folder.CreateFileAsync(fileName);
                var downloader = new Windows.Networking.BackgroundTransfer.BackgroundDownloader();
                var uri = new Uri(SelectedPodcastItem.EnclosureUrl);
                // Create a new download operation.
                var download = downloader.CreateDownload(uri, file);
                //var progress = await download.StartAsync();

                Progress<Windows.Networking.BackgroundTransfer.DownloadOperation> progress = new Progress<Windows.Networking.BackgroundTransfer.DownloadOperation>(progressChanged);
                var cancellationToken = new System.Threading.CancellationTokenSource();
                try
                {
                    await download.StartAsync().AsTask(cancellationToken.Token, progress);
                }
                catch (TaskCanceledException)
                {

                }
            }
            catch { }
        }

        private void progressChanged(Windows.Networking.BackgroundTransfer.DownloadOperation downloadOperation)
        {
            try
            {
                int progress = SelectedPodcastItem.DownloadPercentage = (int)(100 * ((double)downloadOperation.Progress.BytesReceived / (double)downloadOperation.Progress.TotalBytesToReceive));
                //Statustext.Text = String.Format("{0} of {1} kb. downloaded - %{2} complete.", downloadOperation.Progress.BytesReceived / 1024, downloadOperation.Progress.TotalBytesToReceive / 1024, progress);
                switch (downloadOperation.Progress.Status)
                {
                    case Windows.Networking.BackgroundTransfer.BackgroundTransferStatus.Running:
                        {
                            break;
                        }
                    case Windows.Networking.BackgroundTransfer.BackgroundTransferStatus.PausedByApplication:
                        {
                            break;
                        }
                    case Windows.Networking.BackgroundTransfer.BackgroundTransferStatus.PausedCostedNetwork:
                        {
                            break;
                        }
                    case Windows.Networking.BackgroundTransfer.BackgroundTransferStatus.PausedNoNetwork:
                        {
                            break;
                        }
                    case Windows.Networking.BackgroundTransfer.BackgroundTransferStatus.Error:
                        {
                            //Statustext.Text = "An error occured while downloading.";
                            break;
                        }
                }
                if (progress >= 100)
                {
                    downloadOperation = null;
                }
            }
            catch { }
        }

        private void Search(object parameter)
        {
            NavigationService.Navigate(typeof(Views.SearchPage));
        }

        private void Settings(object parameter)
        {
            NavigationService.Navigate(typeof(Views.SettingsPage));
        }

        private void ChangeTheme(object parameter)
        {
            try
            {
                if (SelectedThemeIndex == 0)
                {
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["Theme"] = 0;
                    (NavigationService.Frame).RequestedTheme = Windows.UI.Xaml.ElementTheme.Light;
                }
                else
                {
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["Theme"] = 1;
                    (NavigationService.Frame).RequestedTheme = Windows.UI.Xaml.ElementTheme.Dark;
                    
                }

            }
            catch { }
        }

        private async void Feedback(object parameter)
        {
            try
            {
                var subject = "Feedback about Well podcast app...";
                var body = "Hi";
                var mailto = new Uri("mailto:rushi.programmer@gmail.com?subject=" + subject + "&body=" + body);
                await Windows.System.Launcher.LaunchUriAsync(mailto);
            }
            catch { }
        }

        private async void Privacy(object parameter)
        {
            try
            {
                await Windows.System.Launcher.LaunchUriAsync(new Uri("https://wellpodcast.blogspot.com/2019/11/privacy-policy.html"));
            }
            catch { }
        }

        private int position;
        private SplitView _splitView;
        private MediaPlayerElement _mediaPlayerElement;
        private MediaPlayer _mediaPlayer;
        public MediaPlaybackList _mediaPlaybackList;
        private MediaPlaybackSession mediaPlayBackSession;
        private SystemMediaTransportControls _systemMediaTransportControls;

        private IReadOnlyList<Windows.Storage.StorageFolder> roamingFolders;

        private bool CommandEnabled { get { return true; } }

        public ShellViewModel()
        {
            SelectedPodcastChannel = new Podcast();
        }

        public void Initialize(SplitView splitView, MediaPlayerElement mediaElement)
        {
            _splitView = splitView;
            _mediaPlayerElement = mediaElement;

            _splitView.Content = NavigationService.Frame;

            topFeeds = new ObservableCollection<TopShows>();
            Searches = new ObservableCollection<SearchPodcast>();
            playLists = new ObservableCollection<PlayList>();
            Subscribed = new ObservableCollection<SubscribeViewModel>();

            BackCommand = new DelegateCommand((parameter) => this.Back(parameter), () => this.CommandEnabled);
            HamburgerCommand = new DelegateCommand((parameter) => this.Hamburger(parameter), () => this.CommandEnabled);

            TrendingCommand = new DelegateCommand((parameter) => this.Trending(parameter), () => this.CommandEnabled);

            SearchCommand = new DelegateCommand((parameter) => this.Search(parameter), () => this.CommandEnabled);
            SettingsCommand = new DelegateCommand((parameter) => this.Settings(parameter), () => this.CommandEnabled);

            SuggestBoxQuerySubmittedCommand = new DelegateCommand((parameter) => this.SuggestBoxQuerySubmitted(parameter), () => this.CommandEnabled);

            TopPodcastsSelectionChangedCommand = new DelegateCommand((parameter) => this.TopPodcastsSelectionChanged(parameter), () => this.CommandEnabled);

            PlaySinglePodcastCommand = new DelegateCommand((parameter) => this.PlaySinglePodcast(parameter), () => this.CommandEnabled);
            PlayAllPodcastCommand = new DelegateCommand((parameter) => this.PlayAllPodcast(parameter), () => this.CommandEnabled);
            ClearAllPodcastCommand = new DelegateCommand((parameter) => this.ClearAllPodcast(parameter), () => this.CommandEnabled);

            DownloadSinglePodcastCommand = new DelegateCommand((parameter) => this.DownloadSinglePodcast(parameter), () => this.CommandEnabled);
            DownloadAllPodcastCommand = new DelegateCommand((parameter) => this.PlayAllPodcast(parameter), () => this.CommandEnabled);

            //PodcastListViewSelectionChangedCommand = new DelegateCommand((parameter) => this.PodcastListViewSelectionChanged(parameter), () => this.CommandEnabled);
            PlayListViewSelectionChangedCommand = new DelegateCommand((parameter) => this.PlayListViewSelectionChanged(parameter), () => this.CommandEnabled);
            //PlayListDragStartingCommand = new DelegateCommand((parameter) => this.PlayListDragStarting(parameter), () => this.CommandEnabled);

            SubscribeCommand = new DelegateCommand((parameter) => this.Subscribe(parameter), () => this.CommandEnabled);

            this.FeedbackCommand = new DelegateCommand((parameter) => this.Feedback(parameter), () => this.CommandEnabled);
            this.PrivacyCommand = new DelegateCommand((parameter) => this.Privacy(parameter), () => this.CommandEnabled);
            ChangeThemeCommand = new DelegateCommand((parameter) => this.ChangeTheme(parameter), () => this.CommandEnabled);

            InitializeMediaPlayer();

            IsInternetAvailable = NetworkService.IsConnected;
            if (IsInternetAvailable)
            {
                Trending(null);
                InitializeAsync();
            }

            try
            {
                Windows.UI.Core.SystemNavigationManager.GetForCurrentView().BackRequested += ShellViewModel_BackRequested; ;
                if (Windows.Storage.ApplicationData.Current.LocalSettings.Values.ContainsKey("Theme"))
                {
                    SelectedThemeIndex = int.Parse(Windows.Storage.ApplicationData.Current.LocalSettings.Values["Theme"].ToString());
                    ChangeTheme(null);
                }
                else
                {
                    Windows.Storage.ApplicationData.Current.LocalSettings.Values["Theme"] = 0;
                    SelectedThemeIndex = 0;
                    (NavigationService.Frame).RequestedTheme = Windows.UI.Xaml.ElementTheme.Light;
                }
            }
            catch { }
        }

        private void ShellViewModel_BackRequested(object sender, Windows.UI.Core.BackRequestedEventArgs e)
        {
            Back(null);
        }

        private void InitializeMediaPlayer()
        {
            try
            {
                this._mediaPlayer = new MediaPlayer();
                _systemMediaTransportControls = _mediaPlayer.SystemMediaTransportControls;
                _mediaPlayer.AudioCategory = MediaPlayerAudioCategory.Media;

                this._mediaPlayer.SystemMediaTransportControls.IsEnabled = true;
                this._mediaPlayer.SystemMediaTransportControls.AutoRepeatMode = MediaPlaybackAutoRepeatMode.Track;
                _mediaPlaybackList = new MediaPlaybackList();
                this._mediaPlayerElement.SetMediaPlayer(this._mediaPlayer);
            }
            catch { }
        }

        private async void InitializeAsync()
        {
            try
            {
                var isPresent = await DatabaseHelper.isFilePresent("Subscribe");
                if (isPresent)
                {
                    var subscribe = await DatabaseHelper.ReadTextFileAsync("Subscribe");
                    Subscribed = DatabaseHelper.Deserialize<ObservableCollection<SubscribeViewModel>>(subscribe);
                    foreach (var sub in Subscribed)
                    {
                        var client = new Windows.Web.Syndication.SyndicationClient();
                        client.SetRequestHeader("User-Agent", "Mozilla/5.0 (compatible; MSIE 10.0; Windows NT 6.2; WOW64; Trident/6.0)");
                        try
                        {
                            var currentFeed = await client.RetrieveFeedAsync(new Uri(sub.FeedUrl));

                            if (currentFeed != null)
                            {
                                var item = currentFeed.Items[0];
                                var url = item.Links;
                                if (item.PublishedDate > sub.LastTrackDate)
                                {
                                    var value = currentFeed.Items.Count - sub.TrackCount;
                                    Subscribed[Subscribed.IndexOf(sub)].NotifyNumber = value;
                                }
                            }
                        }
                        catch (Exception ex)
                        {
                            //SyndicationErrorStatus status = SyndicationError.GetStatus(ex.HResult);
                        }
                    }
                }
            }
            catch { }
            //roamingFolders[0].
        }

        private PlayList m_SelectedPlayListItem;
        public PlayList SelectedPlayListItem
        {
            get { return m_SelectedPlayListItem; }
            set { Set(ref m_SelectedPlayListItem, value); }
        }

        private TopShows m_SelectedFeed;
        public TopShows SelectedFeed
        {
            get { return m_SelectedFeed; }
            set { Set(ref m_SelectedFeed, value); }
        }

        private Podcast m_SelectedPodcastChannel;
        public Podcast SelectedPodcastChannel
        {
            get { return m_SelectedPodcastChannel; }
            set { Set(ref m_SelectedPodcastChannel, value); }
        }

        private Feeds m_SelectedPodcastItem;
        public Feeds SelectedPodcastItem
        {
            get { return m_SelectedPodcastItem; }
            set { Set(ref m_SelectedPodcastItem, value); }
        }

        private SearchPodcast m_SelectedSearch;
        public SearchPodcast SelectedSearch
        {
            get { return m_SelectedSearch; }
            set { Set(ref m_SelectedSearch, value); }
        }

        private bool m_IsInternetAvailable;
        public bool IsInternetAvailable
        {
            get { return m_IsInternetAvailable; }
            set { Set(ref m_IsInternetAvailable, value); }
        }

        private int m_SelectedThemeIndex;
        public int SelectedThemeIndex
        {
            get { return m_SelectedThemeIndex; }
            set { Set(ref m_SelectedThemeIndex, value); }
        }

        public ICommand BackCommand { get; set; }
        public ICommand HamburgerCommand { get; set; }

        public ICommand TrendingCommand { get; set; }
        public ICommand DiscoverCommand { get; set; }

        public ICommand SearchCommand { get; set; }
        public ICommand SettingsCommand { get; set; }

        public ICommand SubscribeCommand { get; set; }

        public ICommand SuggestBoxQuerySubmittedCommand { get; set; }

        public ICommand TopPodcastsSelectionChangedCommand { get; set; }

        public ICommand PlaySinglePodcastCommand { get; set; }
        public ICommand PlayAllPodcastCommand { get; set; }
        public ICommand ClearAllPodcastCommand { get; set; }

        public ICommand DownloadSinglePodcastCommand { get; set; }
        public ICommand DownloadAllPodcastCommand { get; set; }

        public ICommand PodcastListViewSelectionChangedCommand { get; set; }
        public ICommand PlayListViewSelectionChangedCommand { get; set; }
        public ICommand PlayListDragStartingCommand { get; set; }

        public ICommand ChangeThemeCommand { get; set; }
        public ICommand FeedbackCommand { get; set; }
        public ICommand PrivacyCommand { get; set; }

        public ObservableCollection<PlayList> playLists { get; set; }
        public ObservableCollection<TopShows> topFeeds { get; private set; }
        public ObservableCollection<SearchPodcast> Searches { get; private set; }
        public ObservableCollection<SubscribeViewModel> Subscribed { get; private set; }
    }
}
