using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MahjongScorer.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class LearnToPlayPage : Page
    {
        public LearnToPlayPage()
        {
            this.InitializeComponent();

            // show the system back button
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

            // get the current version number
            Windows.ApplicationModel.Package pkg = Windows.ApplicationModel.Package.Current;
            Windows.ApplicationModel.PackageVersion version = pkg.Id.Version;
            aboutVersionNumberTextBlock.Text += string.Format("{0}.{1}.{2}.{3}",
                version.Major.ToString(), version.Minor.ToString(), version.Build.ToString(), version.Revision.ToString());
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StartPage), new DrillInNavigationTransitionInfo());
        }
    }
}
