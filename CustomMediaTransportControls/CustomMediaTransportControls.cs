using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;

namespace CustomMediaTransportControls
{
    public sealed class CustomMediaTransportControls : MediaTransportControls
    {
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        public static readonly DependencyProperty TitleProperty =
        DependencyProperty.Register("Title", typeof(string), typeof(CustomMediaTransportControls), new PropertyMetadata(null));

        public event EventHandler<EventArgs> Liked;

        public CustomMediaTransportControls()
        {
            this.DefaultStyleKey = typeof(CustomMediaTransportControls);
        }

        protected override void OnApplyTemplate()
        {
            // This is where you would get your custom button and create an event handler for its click method.
            Button likeButton = GetTemplateChild("LikeButton") as Button;
            likeButton.Click += LikeButton_Click;

            base.OnApplyTemplate();
        }

        private void LikeButton_Click(object sender, RoutedEventArgs e)
        {
            // Raise an event on the custom control when 'like' is clicked
            Liked?.Invoke(this, EventArgs.Empty);
        }
    }
}
