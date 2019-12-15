using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Well_Podcast.Common;

namespace Well_Podcast.ViewModels
{
    public class SettingsViewModel : BindableBase
    {
        public Windows.UI.Xaml.Media.Imaging.BitmapImage Logo { get; set; }

        public string DisplayName { get; set; }

        public string Publisher { get; set; }

        public string Version { get; set; }

        public SettingsViewModel()
        {

        }

        public void Initialize()
        {
            try
            {
                DisplayName = Windows.ApplicationModel.Package.Current.DisplayName;
                Publisher = Windows.ApplicationModel.Package.Current.PublisherDisplayName;
                var ver = Windows.ApplicationModel.Package.Current.Id.Version;
                Version = ver.Major.ToString() + "." + ver.Minor.ToString() + "." + ver.Build.ToString() + "." + ver.Revision.ToString();

                var uri = Windows.ApplicationModel.Package.Current.Logo;
                Logo = new Windows.UI.Xaml.Media.Imaging.BitmapImage(uri);
            }
            catch { }

            InitializeAsync();
        }

        private async void InitializeAsync()
        {
            try
            {
                var file = await Windows.Storage.StorageFile.GetFileFromApplicationUriAsync(new Uri("ms-appx:///Web/privacy.txt"));
                var properties = await file.GetBasicPropertiesAsync();
                var stream = await file.OpenAsync(Windows.Storage.FileAccessMode.Read);
                var reader = new Windows.Storage.Streams.DataReader(stream.GetInputStreamAt(0));
                await reader.LoadAsync((uint)properties.Size);
                string text = reader.ReadString(reader.UnconsumedBufferLength);
                PrivacyText = text;
            }
            catch { }
        }

        private string m_PrivacyText;
        public string PrivacyText
        {
            get { return m_PrivacyText; }
            set { Set(ref m_PrivacyText, value); }
        }
    }
}
