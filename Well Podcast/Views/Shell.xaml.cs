using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Well_Podcast.ViewModels;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace Well_Podcast.Views
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class Shell : Page
    {
        public ShellViewModel ViewModel { get; } = new ShellViewModel();

        public Shell()
        {
            this.InitializeComponent();
            DataContext = ViewModel;
            ViewModel.Initialize(ShellSplitView, ShellMediaPlayer);
        }

        private void Rectangle_ManipulationDelta(object sender, ManipulationDeltaRoutedEventArgs e)
        {
            try
            {
                if (e.Position.X < 280)
                {
                    ShellSplitView.OpenPaneLength = e.Position.X;
                    ShellSplitView.IsPaneOpen = true;
                    e.Complete();
                    ShellSplitView.OpenPaneLength = 240;
                }
            }
            catch { }
        }
    }
}
