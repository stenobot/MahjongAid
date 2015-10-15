using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Runtime.Serialization.Json;
using Windows.Storage;
using Windows.UI.Text;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MahjongScorer.Pages

    
{
    public sealed partial class GameResultsPage : Page
    {
        // holder for local Game object
        private Game game;

        // color resource for winning cell in displayed scores
        SolidColorBrush roundWinnerTextBrush = Application.Current.Resources["MahjongHeaderBrush"] as SolidColorBrush;
        SolidColorBrush roundAdjustmentTextBrush = Application.Current.Resources["MahjongGrayBrush"] as SolidColorBrush;

        public GameResultsPage()
        {
            this.InitializeComponent();

            // we cache this page, in case user comes Back from AllRoundSummaries page
            NavigationCacheMode = NavigationCacheMode.Enabled;

            // show the system back button
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

          
        }

        /// <summary>
        /// Create a new row in the Grid and show the new score values
        /// </summary>
        private void RenderRoundScores()
        {
            for (var roundRow = 0; roundRow < game.CurrentRound; roundRow++)
            {
                // create a grid row for each round
                RowDefinition rd = new RowDefinition();
                scoresGrid.RowDefinitions.Insert(roundRow, rd);


                for (var playerColumn = 0; playerColumn < game.Players.Count; playerColumn++)
                {
                    // create textblocks for the player's round total and the amount their score was adjusted by
                    TextBlock roundTotalTextBlock = new TextBlock();
                    TextBlock roundAdjustmentTextBlock = new TextBlock();
                    StackPanel stackPanel = new StackPanel();

                    // set alignment and other styles
                    stackPanel.Orientation = Orientation.Horizontal;
                    stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
                    roundTotalTextBlock.FontSize = 20;
                    roundAdjustmentTextBlock.FontSize = 10;
                    roundTotalTextBlock.FontWeight = FontWeights.SemiBold;
                    roundAdjustmentTextBlock.FontWeight = FontWeights.SemiBold;
                    roundAdjustmentTextBlock.Foreground = roundAdjustmentTextBrush;

                    // set margin for one text block (to give them some breathing room)
                    Thickness margin = roundTotalTextBlock.Margin;
                    margin.Right = 6;
                    roundTotalTextBlock.Margin = margin;

                    // add sp to grid, set row and column
                    scoresGrid.Children.Add(stackPanel);
                    Grid.SetRow(stackPanel, roundRow);
                    Grid.SetColumn(stackPanel, playerColumn);

                    // add textblocks as a children of stackPanel
                    stackPanel.Children.Add(roundTotalTextBlock);
                    stackPanel.Children.Add(roundAdjustmentTextBlock);

                    // calculate the round score for each round, for each player
                    // in a temporary int, start with starting score
                    int roundScore = ScoreValues.STARTING_SCORE;

                    // calculate the round score by adding successive values in round scores list, 
                    // depending on which row we're rendering in
                    for (var roundScoreCounter = 0; roundScoreCounter < (roundRow + 1); roundScoreCounter++)
                    {
                        roundScore += game.Players[playerColumn].RoundScores[roundScoreCounter];
                    }

                    // set the text for the round total
                    roundTotalTextBlock.Text = roundScore.ToString();
                    roundAdjustmentTextBlock.Text = game.Players[playerColumn].RoundScores[roundRow].ToString();

                    // if the player's round adjustment is positive (not negative, or zero)
                    if (Math.Sign(game.Players[playerColumn].RoundScores[roundRow]) == 1)
                    {
                        // change the color of their round score
                        roundTotalTextBlock.Foreground = roundWinnerTextBrush;
                        // add a plus sign to the beginning of their round adjustment text
                        roundAdjustmentTextBlock.Text = "+" + roundAdjustmentTextBlock.Text;
                    }
                                       
                }
            }
        }

        /// <summary>
        /// Show a summary of the most recent round
        /// </summary>
        private void RenderRoundSummary(string summary)
        {
            // set the title with the current round number
            roundSummaryTitleTextBlock.Text = "ROUND " + game.CurrentRound + " SUMMARY";

            // pass the string-built text into the textblock
            roundSummaryTextBlock.Text = summary;
        }



        private async void ScoreButton_Click(object sender, RoutedEventArgs e)
        {
            // save the game, replacing the existing matching Game object unless it's a brand new game
            await SaveGameAsync();

            Frame.Navigate(typeof(EnterScoresPage), game, new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
            // initialize the first 2 combo boxes (which are not dependent on one another)
            // the 3rd combo box will be initialized when the selection changes on the 2nd one
         //   InitializeComboBoxWithNames(DealerComboBoxStrings, dealerComboBox);




            // make the first section of Scoring View visible
     //       basicScoringStackPanel.Visibility = Visibility.Visible;

            // this fixes a bug 
      //      winnerComboBox.Visibility = Visibility.Collapsed;
            // TEST
      //      testBlock.Text = "";
        }

        private void SetPlayerNames()
        {
            playerOneNameTextBlock.Text = game.Players[0].Name;
            playerOneWindTextBlock.Text = "(" + game.Players[0].CurrentWind.ToString() + ")";
            playerTwoNameTextBlock.Text = game.Players[1].Name;
            playerTwoWindTextBlock.Text = "(" + game.Players[1].CurrentWind.ToString() + ")";
            playerThreeNameTextBlock.Text = game.Players[2].Name;
            playerThreeWindTextBlock.Text = "(" + game.Players[2].CurrentWind.ToString() + ")";
            playerFourNameTextBlock.Text = game.Players[3].Name;
            playerFourWindTextBlock.Text = "(" + game.Players[3].CurrentWind.ToString() + ")";
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Game)
            {
                game = e.Parameter as Game;

                // check whether game is in progress, or over, and change UI based on that
                DetermineInProgressStatus();
            }   

            SetPlayerNames();


            if (e.NavigationMode != NavigationMode.Back)
            {
                // save the game so it's state is set if the user leaves and reloads the save data later
                await SaveGameAsync();

                // show the scores
                RenderRoundScores();
            }
          

            base.OnNavigatedTo(e);
        }

        private void DetermineInProgressStatus()
        {
            // if we haven't completed a round, we know it's a brand new game, so hide the summary
            if (game.CurrentRound == 0)
                roundSummaryGrid.Visibility = Visibility.Collapsed;
            else
            {
                // a game is over only after it goes around the table 4 times
                // when the dealer wins, though, it doesn't advance
                // so, calculating 16 rounds plus times dealer won to know when the game is over
                if (game.CurrentRound >= (16 + game.TimesDealerWon))
                {
                    // the game is over
                    game.InProgress = false;

                    // don't show the score next round button
                    scoreRoundButton.Visibility = Visibility.Collapsed;

                    // show game over UI
                    gameOverStackPanel.Visibility = Visibility.Visible;

                    // calculate the winner
                    foreach (Player player in game.Players)
                    {
                        if (player.TotalScore == Math.Max(Math.Max(game.Players[0].TotalScore, game.Players[1].TotalScore),
                        Math.Max(game.Players[2].TotalScore, game.Players[3].TotalScore)))
                        {
                            // set winner property
                            player.IsGameWinner = true;
                            // set winner name (so we can show it in save data)
                            game.WinnerName = player.Name;
                            // set text in new UI declaring the winner
                            gameOverTextBlock.Text = "The game is over. " + player.Name + " wins with a total score of " + player.TotalScore + "!";
                        }
                        else
                            player.IsGameWinner = false;
                    }
                }
                else
                {
                    // the game is still in progress, set the "Score next round" button tex
                    scoreRoundButton.Content = "Score round " + (game.CurrentRound + 1);
                }

                // we don't show the round summary before the game has begun, but we do when it's in progress or over
                roundSummaryGrid.Visibility = Visibility.Visible;
                RenderRoundSummary(game.RoundSummaries[game.CurrentRound - 1]);
            }
        }

        /// <summary>
        /// case1: new game - Game object passed as parameter from EnterNamesPage
        /// case2: load in progress game - Object passed as parameter from ExistingGamesPage
        /// case3: load completed game - Object passed as parameter from ExistingGamesPage
        /// case4: continue current game session - Object passed as parameter from EnterScoresPage
        /// in all cases, we need to find and overwrite the existing object
        /// </summary>
        /// <returns></returns>
        private async Task SaveGameAsync()
        {
            // create new instance of saved games list
            List<Game> SavedGamesList = new List<Game>();

            // read saved games list from Json data
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Game>));

            var stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync("mahjong-data.json");

            // retrieve the list of game objects add assign to our new List
            SavedGamesList = (List<Game>)serializer.ReadObject(stream);

            // check our current game against the saved games
            foreach (Game savedGame in SavedGamesList)
            {
                // if the game objects match
                if (savedGame.Guid == game.Guid)
                {
                    // remove the existing saved game from the master list
                    SavedGamesList.Remove(savedGame);
                    // add the new version of the game to the list
                    SavedGamesList.Add(game);
                    // break out of the loop (this is important, otherwise crashy crashy)
                    break;
                }      
            }
            
            // write saved games list back to storage
            using (stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync("mahjong-data.json", CreationCollisionOption.ReplaceExisting))
            {
                // pass game list into the serializer
                serializer.WriteObject(stream, SavedGamesList);

                if (game.CurrentRound == 0)
                    savedMessageTextBlock.Text = "New game created";
                else
                    savedMessageTextBlock.Text = "Round " + game.CurrentRound + " info saved";

                FadeOutMessage.Begin();
            }

            
        }

        private void RoundSummariesButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RoundSummariesPage), game, new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
        }

        private void StartNewGame_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StartPage));
        }
    }
}
