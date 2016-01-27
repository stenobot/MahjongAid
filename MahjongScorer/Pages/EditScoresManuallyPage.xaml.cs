using System;
using System.Linq;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MahjongScorer.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class EditScoresManuallyPage : Page
    {
        // holder for local Game object
        private Game game;

        int p1Score = 0;
        int p2Score = 0;
        int p3Score = 0;
        int p4Score = 0;

        public EditScoresManuallyPage()
        {
            this.InitializeComponent();
        }


        /// <summary>
        /// Similar to EndRound() on EnterScoresPage, but slightly customized for manual entry of scores
        /// </summary>
        /// <param name="dealerWon">True if dealer won, false if not</param>
        private void EndRound(bool dealerWon)
        {
            if (dealerWon)
                // increment property tracking how many times the dealer won
                game.TimesDealerWon++;
            else // only change dealer and lucky wind if the dealer didn't win
            {
                // shift Winds and dealer counterclockwise around the table
                foreach (Player player in game.Players)
                {
                    switch (player.CurrentWind)
                    {
                        case Wind.East:
                            player.CurrentWind = Wind.North;
                            player.IsDealer = false;
                            break;
                        case Wind.North:
                            player.CurrentWind = Wind.West;
                            player.IsDealer = false;
                            break;
                        case Wind.West:
                            player.CurrentWind = Wind.South;
                            player.IsDealer = false;
                            break;
                        case Wind.South:
                            player.CurrentWind = Wind.East;
                            player.IsDealer = true;
                            // set name for save data display
                            game.CurrentDealerName = player.Name;
                            break;
                    }
                }

                // if we've gone around the table once, prevailing wind changes
                if ((game.CurrentRound - game.TimesDealerWon) % 4 == 0)
                    game.PrevailingWind++;
            }

            // always add current prevailing wind to prevailing winds list (even if it doesn't change)
            game.PrevailingWinds.Add(game.PrevailingWind);

            game.LoadedFromSave = false;
        }


        #region CONTROL EVENTS

        private void PlayerScoreTextBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
        {
            if ((PlayerOneScoreTextBox.Text.Count() > 0) &&
                (PlayerTwoScoreTextBox.Text.Count() > 0) &&
                (PlayerThreeScoreTextBox.Text.Count() > 0) &&
                (PlayerFourScoreTextBox.Text.Count() > 0))
                scoreRoundButton.Visibility = Visibility.Visible;
            else
                scoreRoundButton.Visibility = Visibility.Collapsed;
        }

        private async void ScoreRoundButton_Click(object sender, RoutedEventArgs e)
        {
            // make sure all four scores successfully parse to ints
            // otherwise, show error dialog
            if ((int.TryParse(PlayerOneScoreTextBox.Text, out p1Score)) &&
                (int.TryParse(PlayerTwoScoreTextBox.Text, out p2Score)) &&
                (int.TryParse(PlayerThreeScoreTextBox.Text, out p3Score)) &&
                (int.TryParse(PlayerFourScoreTextBox.Text, out p4Score)))
            {
                // get the winning score's index
                int[] scores = { p1Score, p2Score, p3Score, p4Score };

                int winnerIndex = 0;
                int winningScoreCount = 0;

                foreach (int score in scores)
                {
                    if (score == scores.Max())
                    {
                        // find out how many winning scores there are (there should only be one)
                        for (int i = 0; i < scores.Length; i++)
                        {
                            if (score == scores[i])
                                winningScoreCount++;
                        }

                        // break the loop so winnerIndex no longer increments 
                        break;
                    }
                    winnerIndex++;
                }

                // make sure all 4 scores add up to zero, and there's only one winning score
                // otherwise, show an error
                if (((p1Score + p2Score + p3Score + p4Score) == 0) &&
                    (winningScoreCount == 1))
                {
                    // Show a confirmation dialog, to double check since we're permanently committing round scores
                    var confirmDialog = new MessageDialog("End this round with " +
                        game.Players[winnerIndex].Name +
                        " as the winner, earning " +
                        scores[winnerIndex]
                        + " points?");

                    // Add command and callback for finalizing the round scores and ending the round
                    confirmDialog.Commands.Add(new UICommand("Yes", (command) =>
                    {
                        // add each player's score to RoundScores in the game instance
                        for (int i = 0; i < game.Players.Count; i++)
                        {
                            game.Players[i].RoundScores.Add(scores[i]);
                        }

                        // add a round summary
                        game.RoundSummaries.Add(game.Players[winnerIndex].Name + 
                            " won this round, with a winning score of " + 
                            scores[winnerIndex] + ".");

                        // end the round, passing in true if the winning player is also the dealer
                        EndRound(game.Players[winnerIndex].IsDealer);

                        Frame.Navigate(typeof(GameResultsPage), game, new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
                    }));

                    // Add command and callback for canceling
                    confirmDialog.Commands.Add(new UICommand("Not yet"));

                    // Set the default
                    confirmDialog.DefaultCommandIndex = 1;

                    // Set the command to be invoked when escape is pressed
                    confirmDialog.CancelCommandIndex = 1;

                    // Show the message dialog
                    await confirmDialog.ShowAsync();
                }
                else
                {
                    var scoresErrorDialog = new MessageDialog("Looks like there's a problem with the scores. Make sure they all add up to zero, and that there's only one winning score.");
                    scoresErrorDialog.Commands.Add(new UICommand("Close"));

                    await scoresErrorDialog.ShowAsync();
                }
            }
            else
            {
                var parseErrorDialog = new MessageDialog("The player scores must be numbers.");
                parseErrorDialog.Commands.Add(new UICommand("Close"));

                await parseErrorDialog.ShowAsync();
            }
        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StartPage), new DrillInNavigationTransitionInfo());
        }

        #endregion


        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Game)
            {
                game = e.Parameter as Game;

                // Set XAML element text
                editScoresManuallyPageTitle.Text = "ENTER ROUND " + game.CurrentRound + " SCORES";
                scoreRoundButton.Content = "Score Round " + game.CurrentRound;
                playerOneNameTextBlock.Text = game.Players[0].Name;
                playerTwoNameTextBlock.Text = game.Players[1].Name;
                playerThreeNameTextBlock.Text = game.Players[2].Name;
                playerFourNameTextBlock.Text = game.Players[3].Name;                
            }
        }       
    }
}
