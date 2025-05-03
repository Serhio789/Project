using System.Windows;
using System.Windows.Controls;

namespace WPFBookStore.Pages
{
    /// <summary>
    ///Home.xaml
    /// </summary>
    public partial class Home : Page
    {
        public Home()
        {
            InitializeComponent();
        }

        private void MySlider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (MyScrollViewer != null)
            {
                MyScrollViewer.ScrollToVerticalOffset(e.NewValue);
            }
        }
    }
}
