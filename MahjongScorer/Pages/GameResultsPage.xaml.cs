using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Navigation;
using System.Runtime.Serialization.Json;
using Windows.Storage;
using Windows.UI.Text;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Shapes;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MahjongScorer.Pages


{
    public sealed partial class GameResultsPage : Page
    {
        // holder for local Game object
        private Game game;

        // color resource for winning cell in displayed scores
        SolidColorBrush accentColorBrush = Application.Current.Resources["MahjongAccentColorBrush"] as SolidColorBrush;
        SolidColorBrush grayBrush = Application.Current.Resources["MahjongGrayBrush"] as SolidColorBrush;
        SolidColorBrush redBrush = Application.Current.Resources["MahjongRedBrush"] as SolidColorBrush;
        Thickness lineMargin = new Thickness(0, 10, 0, 5);

        public GameResultsPage()
        {
            this.InitializeComponent();

            // we cache this page, in case user comes Back from AllRoundSummaries page
            NavigationCacheMode = NavigationCacheMode.Enabled;     
        }

        /// <summary>
        /// Create a new row in the Grid and show the new score values
        /// </summary>
        private void RenderRoundScores()
        {
            // always clear grid first to avoid conflicts when loading a different game
            scoresGrid.Children.Clear();

            // keep track of extra rows added for separator lines
            int lineRowsAdded = 0;

            // keep track of rounds per prevailing wind change, and reset to 1 when the wind changes
            int roundPerPrevWind = 1;

            for (var round = 0; round < game.CurrentRound; round++)
            {
                // create one grid row for each round
                RowDefinition scoresRd = new RowDefinition();
                scoresGrid.RowDefinitions.Insert(round + lineRowsAdded, scoresRd);

                // if the prevailing wind changes, we need to add a row for a separator line
                // this can't happen before round 5
                if (round > 3)
                {
                    if (game.PrevailingWinds[round] != game.PrevailingWinds[round - 1])
                    {
                        lineRowsAdded++;
                        roundPerPrevWind = 1;

                        // create an extra row for the separator line
                        RowDefinition lineRd = new RowDefinition();
                        scoresGrid.RowDefinitions.Add(lineRd);

                        // create the line, set it's properties
                        Line line = new Line();
                        line.X2 = 365;
                        line.StrokeThickness = 1;
                        line.Stroke = accentColorBrush;
                        line.HorizontalAlignment = HorizontalAlignment.Center;
                        line.Margin = lineMargin;
                        scoresGrid.Children.Add(line);
                        Grid.SetRow(line, round + lineRowsAdded - 1);
                        Grid.SetColumnSpan(line, 5);
                    }
                }

                // create textblock for the prevailing wind
                TextBlock prevailingWindTextBlock = new TextBlock();

                prevailingWindTextBlock.Text = game.PrevailingWinds[round].ToString() + " " + roundPerPrevWind + ":";
                prevailingWindTextBlock.Foreground = grayBrush;
                prevailingWindTextBlock.HorizontalAlignment = HorizontalAlignment.Right;
                prevailingWindTextBlock.FontSize = 12;
                prevailingWindTextBlock.VerticalAlignment = VerticalAlignment.Center;

                scoresGrid.Children.Add(prevailingWindTextBlock);
                Grid.SetRow(prevailingWindTextBlock, round + lineRowsAdded);

                roundPerPrevWind++;


                for (var player = 0; player < game.Players.Count; player++)
                {
                    // create textblocks for the player's round total, and the amount their score was adjusted by
                    TextBlock roundTotalTextBlock = new TextBlock();
                    TextBlock roundAdjustmentTextBlock = new TextBlock();

                    // create a horizontal stack panel to hold the round score and adjustment
                    StackPanel stackPanel = new StackPanel();

                    // set alignment and other styles
                    stackPanel.Orientation = Orientation.Horizontal;
                    stackPanel.HorizontalAlignment = HorizontalAlignment.Center;
                    roundTotalTextBlock.FontSize = 20;                    
                    roundAdjustmentTextBlock.FontSize = 10;
                    roundTotalTextBlock.Width = 54;
                    roundAdjustmentTextBlock.Width = 34;
                    roundTotalTextBlock.TextAlignment = TextAlignment.Right;
                    roundAdjustmentTextBlock.TextAlignment = TextAlignment.Left;
                    roundTotalTextBlock.FontWeight = FontWeights.SemiBold;
                    roundAdjustmentTextBlock.FontWeight = FontWeights.SemiBold;
                    roundAdjustmentTextBlock.Foreground = grayBrush;

                    // set margin for one text block (to give them some breathing room)
                    Thickness margin = roundTotalTextBlock.Margin;
                    margin.Right = 6;
                    roundTotalTextBlock.Margin = margin;

                    // add sp to grid, set row and column
                    scoresGrid.Children.Add(stackPanel);
                    Grid.SetRow(stackPanel, round + lineRowsAdded);
                    Grid.SetColumn(stackPanel, player + 1);

                    // add textblocks as a children of stackPanel
                    stackPanel.Children.Add(roundTotalTextBlock);
                    stackPanel.Children.Add(roundAdjustmentTextBlock);

                    // calculate the round score for each round, for each player
                    // in a temporary int, start with starting score
                    int roundScore = ScoreValues.STARTING_SCORE;

                    // calculate the round score by adding successive values in round scores list, 
                    // depending on which row we're rendering in
                    for (var roundScoreCounter = 0; roundScoreCounter < (round + 1); roundScoreCounter++)
                    {
                        roundScore += game.Players[player].RoundScores[roundScoreCounter];
                    }

                    // set the text for the round total; if it's less than zero, just show zero
                    if (roundScore < 0)
                        roundTotalTextBlock.Foreground = redBrush;
                    
                    roundTotalTextBlock.Text = roundScore.ToString();

                    // set text for the adjustment; we'll show negative or positive here
                    roundAdjustmentTextBlock.Text = game.Players[player].RoundScores[round].ToString();

                    // if the player's round adjustment is positive (not negative, or zero)
                    if (Math.Sign(game.Players[player].RoundScores[round]) == 1)
                    {
                        // change the color of their round score
                        roundTotalTextBlock.Foreground = accentColorBrush;
                        // add a plus sign to the beginning of their round adjustment text
                        roundAdjustmentTextBlock.Text = "+" + roundAdjustmentTextBlock.Text;
                    }                                       
                }

                //// create line separator when prevailing wind changes
                //if (game.PrevailingWind == Wind.East)
                //{
                //    // create a grid row for line
                //    scoresGrid.RowDefinitions.Insert(roundRow + 1, rd);

                //    // create line
                //    Line line = new Line();
                //    line.Stroke = roundWinnerTextBrush;
                //    line.X2 = 320;
                //    line.HorizontalAlignment = HorizontalAlignment.Center;
                //    line.StrokeThickness = 1;
                //}

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

            Frame.Navigate(typeof(EnterScoresPage), game, new DrillInNavigationTransitionInfo());
        }

        private void SetPlayerNames()
        {
            playerOneNameTextBlock.Text = game.Players[0].Name;
            playerOneWindTextBlock.Text = game.Players[0].CurrentWind.ToString();
            playerTwoNameTextBlock.Text = game.Players[1].Name;
            playerTwoWindTextBlock.Text = game.Players[1].CurrentWind.ToString();
            playerThreeNameTextBlock.Text = game.Players[2].Name;
            playerThreeWindTextBlock.Text = game.Players[2].CurrentWind.ToString();
            playerFourNameTextBlock.Text = game.Players[3].Name;
            playerFourWindTextBlock.Text = game.Players[3].CurrentWind.ToString();
        }

        private void SetNextRoundInfo()
        {
            nextDealerTextBlock.Text = game.CurrentDealerName;
            nextPrevailingWindTextBlock.Text = game.PrevailingWind.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        protected override async void OnNavigatedTo(NavigationEventArgs e)
        {
            // always hide the back button on this page
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

            if (e.Parameter is Game)
            {
                game = e.Parameter as Game;

                // check whether game is in progress, or over, and change UI based on that
                DetermineInProgressStatus();
            }   

            SetPlayerNames();
            SetNextRoundInfo();

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
            // if we haven't completed a round, we know it's a brand new game
            if (game.CurrentRound == 0)
            {
                // hide the summary
                roundSummaryGrid.Visibility = Visibility.Collapsed;
                // hide the app bar button
                roundSummariesAppBarButton.Visibility = Visibility.Visible;
            }
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

                    // don't show next round info
                    nextDealerWindGrid.Visibility = Visibility.Collapsed;

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
                else // the game is in progress
                {
                    //set the "Score next round" button text
                    scoreRoundButton.Content = "Score Round " + (game.CurrentRound + 1);
                }

                // we don't show the round summary before the game has begun, but we do when it's in progress or over
                roundSummaryGrid.Visibility = Visibility.Visible;
                RenderRoundSummary(game.RoundSummaries[game.CurrentRound - 1]);

                // show the Round Summaries app bar button
                roundSummariesAppBarButton.Visibility = Visibility.Visible;
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

                // show a quick confirmation message about the saved game, then fade out
                if (!game.LoadedFromSave)
                {
                    if (game.CurrentRound == 0)
                        savedMessageTextBlock.Text = "New game created";
                    else
                        savedMessageTextBlock.Text = "Round " + game.CurrentRound + " info saved";
                }
                else
                {
                    savedMessageTextBlock.Text = "Saved game loaded";
                }

                savedMessageTextBlock.Opacity = 1;
                FadeOutMessage.Begin();
            }
        }

        private void RoundSummariesButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(RoundSummariesPage), game, new DrillInNavigationTransitionInfo());
        }

        private void StartNewGame_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StartPage));
        }


        private void LearnToPlayButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LearnToPlayPage), new DrillInNavigationTransitionInfo());

        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StartPage), new DrillInNavigationTransitionInfo());
        }
    }
}
