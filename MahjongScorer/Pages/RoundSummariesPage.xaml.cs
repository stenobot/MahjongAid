using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MahjongScorer.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class RoundSummariesPage : Page
    {
        // holder for local Game object
        private Game game;

        private List<string> RoundNamesList = new List<string>();
        

        public RoundSummariesPage()
        {
            this.InitializeComponent();

            // show the system back button
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
        }




        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Game)
            {
                game = e.Parameter as Game;
            }

            for (var i = 0; i < game.RoundSummaries.Count; i++)
            {
                RoundNamesList.Add("Round " + (i + 1));
            }

            roundSummariesPivot.ItemsSource = game.RoundSummaries;

            RenderRoundSummaries();
            base.OnNavigatedTo(e);
        }

        private void RenderRoundSummaries()
        {
            foreach (string roundSummary in game.RoundSummaries)
            {

            }
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(GameResultsPage), game);
        }
    }
}
