﻿using System;
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

        //    NavigationCacheMode = NavigationCacheMode.Enabled;

            // show system back button, handle back
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            SystemNavigationManager.GetForCurrentView().BackRequested += (s, a) =>
            {
                if (Frame.CanGoBack)
                {
                    Frame.GoBack();
                    a.Handled = true;
                }
            };
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

                    // set alignment and other styles
                    roundTotalTextBlock.HorizontalAlignment = HorizontalAlignment.Left;
                    roundAdjustmentTextBlock.HorizontalAlignment = HorizontalAlignment.Right;
                    roundAdjustmentTextBlock.FontSize = 10;
                    roundAdjustmentTextBlock.Foreground = roundAdjustmentTextBrush;

                    // add textblocks as a children of scoresgrid
                    scoresGrid.Children.Add(roundTotalTextBlock);
                    scoresGrid.Children.Add(roundAdjustmentTextBlock);

                    // set row and column for textblocks
                    Grid.SetRow(roundTotalTextBlock, roundRow);
                    Grid.SetColumn(roundTotalTextBlock, playerColumn);
                    Grid.SetRow(roundAdjustmentTextBlock, roundRow);
                    Grid.SetColumn(roundAdjustmentTextBlock, playerColumn);

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

            Frame.Navigate(typeof(EnterScoresPage), game);
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
            }

            SetPlayerNames();

            await SaveGameAsync();

            // if we haven't completed a round, we know it's a brand new game 
            // hide the summary; otherwise show them
            if (game.CurrentRound == 0)
                roundSummaryGrid.Visibility = Visibility.Collapsed;
            else
            {
                roundSummaryGrid.Visibility = Visibility.Visible;
                RenderRoundSummary(game.RoundSummaries[game.CurrentRound - 1]);
            }

            // a game ends after it goes around the table 4 times
            // when the dealer wins, though, it doesn't advance
            // so, calculating 16 rounds plus times dealer won to know when the game is over
            if (game.CurrentRound >= (16 + game.TimesDealerWon))
            {
                // don't show the score next round button
                scoreRoundButton.Visibility = Visibility.Collapsed;

                // show some text instead
                gameOverStackPanel.Visibility = Visibility.Visible;
                
                // set the game winner by checking all players' total scores against each other
                // and assigning the gameWinner
                //Player gameWinner = new Player();
                for (var i = 0; i < game.Players.Count; i++)
                {
                    game.Players[0].IsGameWinner = true;
                    if (i == 0)
                        continue;

                    if (game.Players[i].TotalScore > game.Players[i - 1].TotalScore)
                    {
                        // set winner
                        game.Players[i].IsGameWinner = true;

                        // set winner name for completed games save data
                        game.WinnerName = game.Players[i].Name;

                        // set text declaring the winner
                        gameOverTextBlock.Text = "The game is over. " + game.Players[i].Name + " wins with a total score of " + game.Players[i].TotalScore + "!";

                        // lastly, we save the game data once more so it's permanently over
                        await SaveGameAsync();
                    }
                }


                // TODO we should show a start new game button, and a message telling them that the game is over
            }
            else
            {
                // set the "Score next round" button text
                scoreRoundButton.Content = "Score round " + (game.CurrentRound + 1);
            }

            RenderRoundScores();

            base.OnNavigatedTo(e);

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
            Frame.Navigate(typeof(RoundSummariesPage), game);
        }

        private void StartNewGame_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StartPage));
        }
    }
}
