using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation.Metadata;
using Windows.Phone.UI.Input;
using Windows.UI.Core;

namespace Well_Podcast.Services.ActivationServices
{
    public class FirstUseActivationService
    {
        private static readonly EventHandler<BackRequestedEventArgs> GoBackHandler =
            (s, e) => NavigationService.GoBack();

        private static readonly EventHandler<BackPressedEventArgs> GoBackPhoneHandler =
             (s, e) => NavigationService.GoBack();

        public static bool IsFirstRun { get; set; }

        private static bool DetectIfFirstUse()
        {
            if (Common.DatabaseHelper.ContainsKey(nameof(IsFirstRun)))
            {
                return false;
            }
            else
            {
                Common.DatabaseHelper.SaveSettings(nameof(IsFirstRun), false);
                return true;
            }
        }

        public FirstUseActivationService()
        {
            IsFirstRun = DetectIfFirstUse();
        }

        public async Task InitializeAsync()
        {
            try
            {
                if (ApiInformation.IsTypePresent("Windows.Phone.UI.Input.HardwareButtons"))
                {
                    HardwareButtons.BackPressed -= GoBackPhoneHandler;
                    HardwareButtons.BackPressed += GoBackPhoneHandler;

                    var statusBar = Windows.UI.ViewManagement.StatusBar.GetForCurrentView();
                    await statusBar.HideAsync();

                    return;
                }

                var navManager = SystemNavigationManager.GetForCurrentView();
                navManager.AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
                navManager.BackRequested -= GoBackHandler;
                navManager.BackRequested += GoBackHandler;
            }
            catch { }
        }
    }
}
