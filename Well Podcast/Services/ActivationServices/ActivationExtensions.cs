using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace Well_Podcast.Services.ActivationServices
{
    public static class ActivationExtensions
    {
        public static Window EnsureRootFrame(this Window window)
        {
            Frame rootFrame = Window.Current.Content as Frame;
            if (rootFrame == null)
            {
                rootFrame = new Frame();
                rootFrame.NavigationFailed += (sender, e) => { throw new Exception("Failed to load Page " + e.SourcePageType.FullName); };
                NavigationService.Frame = rootFrame;
                Window.Current.Content = new Views.Shell();
            }

            return window;
        }
    }
}
