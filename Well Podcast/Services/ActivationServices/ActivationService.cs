using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.ApplicationModel.Activation;
using Windows.UI.ViewManagement;

namespace Well_Podcast.Services.ActivationServices
{
    public class ActivationService
    {
        public static LaunchActivatedEventArgs activatedEventArgs;
        public static IActivatedEventArgs activationLaunchArgs;

        public async Task LaunchAsync(LaunchActivatedEventArgs e)
        {
            // Custom pre-launch service calls.
            await PreLaunchAsync(e);

            // Navigate
            Windows.UI.Xaml.Window.Current.EnsureRootFrame().Activate();

            // Custom post-launch service calls.
            PostLaunchAsync(e);
        }

        internal async Task ActivationLaunchAsync(IActivatedEventArgs args)
        {
            activationLaunchArgs = args;

            await new Services.ActivationServices.ActivationService().LaunchAsync(null);
        }

        private async Task PreLaunchAsync(LaunchActivatedEventArgs e)
        {
            try
            {
                activatedEventArgs = e;
                await Common.Singleton<FirstUseActivationService>.Instance.InitializeAsync();
                //await Common.Singleton<FileService>.Instance.InitializeAsync();

            }
            catch { }

        }

        private void PostLaunchAsync(LaunchActivatedEventArgs e)
        {
            try
            {
                activatedEventArgs = e;
                //Common.Singleton<YahooServices.YahooService>.Instance.Initialize();
                //NavigationService.Navigate(typeof(Views.MainPage));
            }
            catch { }
        }

        public async Task Initialize()
        {
            if (Windows.Foundation.Metadata.ApiInformation.IsTypePresent("Windows.UI.ViewManagement.StatusBar"))
            {
                var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                await statusBar.HideAsync();
            }
            else
            {
                var bounds = ApplicationView.GetForCurrentView().VisibleBounds;
                ApplicationView.PreferredLaunchViewSize = new Windows.Foundation.Size(bounds.Width, bounds.Height);
                ApplicationView.PreferredLaunchWindowingMode = ApplicationViewWindowingMode.PreferredLaunchViewSize;
                Windows.UI.Core.SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = Windows.UI.Core.AppViewBackButtonVisibility.Visible;
                ApplicationViewTitleBar titleBar = ApplicationView.GetForCurrentView().TitleBar;
                titleBar.BackgroundColor = Windows.UI.Color.FromArgb(1, 2, 102, 200);
                titleBar.ForegroundColor = Windows.UI.Colors.White;
                titleBar.ButtonBackgroundColor = Windows.UI.Color.FromArgb(1, 2, 102, 200);
                titleBar.ButtonForegroundColor = Windows.UI.Colors.White;
            }
        }
    }
}
