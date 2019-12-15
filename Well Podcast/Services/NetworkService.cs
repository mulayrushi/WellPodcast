using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading.Tasks;

namespace Well_Podcast.Services
{
    public class NetworkService
    {
        public static bool isInternetConnected;

        public void Initialize()
        {
            isInternetConnected = NetworkInterface.GetIsNetworkAvailable();
        }
    }
}
