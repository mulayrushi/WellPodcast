using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Core;
using Windows.Networking.Connectivity;
using Windows.UI.Core;

namespace Well_Podcast.Services
{
    public class InternetConnectionChangedEventArgs : EventArgs
    {
        public InternetConnectionChangedEventArgs(bool isConnected)
        {
            this.isConnected = isConnected;
        }

        private bool isConnected;
        public bool IsConnected
        {
            get { return isConnected; }
        }
    }

    public class NetworkService
    {
        public event EventHandler<InternetConnectionChangedEventArgs>
            InternetConnectionChanged;

        public void StartListeningAsync()
        {
            NetworkInformation.NetworkStatusChanged += NetworkInformation_NetworkStatusChanged;

        }

        public void StopListening()
        {
            NetworkInformation.NetworkStatusChanged -= NetworkInformation_NetworkStatusChanged;
        }

        private async void NetworkInformation_NetworkStatusChanged(object sender)
        {
            await CoreApplication.MainView.CoreWindow.Dispatcher.RunAsync(CoreDispatcherPriority.Normal, () =>
            {
                var arg = new InternetConnectionChangedEventArgs(IsConnected);
                InternetConnectionChanged?.Invoke(this, arg);
            });
        }

        public static bool IsConnected
        {
            get
            {
                var profile = NetworkInformation.GetInternetConnectionProfile();
                var isConnected = (profile != null
                    && profile.GetNetworkConnectivityLevel() ==
                    NetworkConnectivityLevel.InternetAccess);
                return isConnected;
            }
        }
    }
}
