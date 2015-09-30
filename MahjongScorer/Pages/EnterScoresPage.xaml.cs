﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Text;
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
    public sealed partial class EnterScoresPage : Page
    {
        // holder for local Game object
        private Game game;

    //    private List<string> RoundSummaries;

        private int _currentBaseScore;
        private int _currentPungValue;
        private int _currentKongValue;
        
        // keep track of concealed pungs or kongs for "three concealed sets"
        private int _concealedPungsKongsCount;

        // keep track of terminals or honors pungs kongs for "all terminals or honors"
        private int _terminalsHonorsPungsKongsCount;

        // keep track of whether winning tile was self drawn
        private bool _selfDrawn;

        // keep track of whether the dealer won this round
        private bool _dealerWon;

        // keep track of which player we removed from the drawn from combo box each round
        private int _ineligableDrawnFromPlayerIndex;

        // holders for combo box lists
        private List<string> DealerComboBoxStrings;
        private List<string> WinComboBoxStrings;
        private List<string> DrawnComboBoxStrings;

        // holders for check box lists
        private List<SetCheckBox> PungTerminalsHonorsCheckBoxes;
        private List<SetCheckBox> PungConcealedCheckBoxes;
        private List<SetCheckBox> KongTerminalsHonorsCheckBoxes;
        private List<SetCheckBox> KongConcealedCheckBoxes;

        // lists of possible sets, and Set objects (we start with 4 and there can only ever be 4 total)
        List<Set> PossiblePungs;
        List<Set> PossibleKongs;
        List<WinCondition> PossibleWinConditions;
        Set noSets;
        Set set1;
        Set set2;
        Set set3;
        Set set4;

        public EnterScoresPage()
        {
            this.InitializeComponent();

            
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
        /// Initialize a normal combo box
        /// takes a List of strings and a comboBox as parameters
        /// </summary>
        private void InitializeComboBoxWithNames(List<string> ComboBoxNames, ComboBox comboBox)
        {
            ComboBoxNames = new List<string>();

            // populate combobox lists with names
            foreach (Player player in game.Players)
            {
                ComboBoxNames.Add(player.Name);
            }

            // set the list as the Items Source for the combo box
            comboBox.ItemsSource = ComboBoxNames;
        }


        /// <summary>
        /// initialize the Drawn From combo box, which has an extra option at the end (after player names) 
        /// </summary>
        private void InitializeDrawnFromComboBox()
        {
            DrawnComboBoxStrings = new List<string>();

            foreach (Player player in game.Players)
            {
                DrawnComboBoxStrings.Add("Drawn from " + player.Name);
            }
            
            // add final option for self-drawn to drawn from combo box
            DrawnComboBoxStrings.Add("Tile was self-drawn");
        }


        /// <summary>
        /// Set up the possible pungs as a List of Sets
        /// </summary>
        private void InitializePossiblePungs()
        {
            PossiblePungs = new List<Set>();

            PossiblePungs.Add(noSets = new Set());
            PossiblePungs.Add(set1 = new Set());
            PossiblePungs.Add(set2 = new Set());
            PossiblePungs.Add(set3 = new Set());
            PossiblePungs.Add(set4 = new Set());

            int setNum = 0;
            foreach (Set set in PossiblePungs)
            {
                set.Index = setNum;

                if (set.Index != 0)
                    set.Name = "pung " + setNum.ToString();
                else
                    set.Name = "No sets";

                setNum++;
            }
            pungCountComboBox.ItemsSource = PossiblePungs;
        }


        /// <summary>
        /// Set up possible kongs. This will change based on how many Pungs have been selected
        /// and resets the combo box when pungs are set
        /// </summary>
        /// <param name="pungComboBox"></param>
        private void InitializePossibleKongs(ComboBox pungComboBox)
        {
            if (pungCountComboBox.SelectedItem != null)
            {
                PossibleKongs = new List<Set>();
                PossibleKongs.Add(noSets = new Set());

                for (var i = 0; i < (4 - pungComboBox.SelectedIndex); i++)
                {
                    PossibleKongs.Add(PossiblePungs[i + 1]);
                }

                kongCountComboBox.ItemsSource = PossibleKongs;
            }
        }


        /// <summary>
        /// Sets up and resets the check boxes for pungs and kongs
        /// </summary>
        /// <param name="comboBox"></param>
        /// <param name="grid"></param>
        /// <param name="setType"></param>
        private void InitializeSetCheckBoxes(ComboBox comboBox, Grid grid, string setType)
        {
            //clear all the children (in case selection changes)
            grid.Children.Clear();

            // check if pung or kong, then set a new list
            if (setType == "pung")
            {
                PungTerminalsHonorsCheckBoxes = new List<SetCheckBox>();
                PungConcealedCheckBoxes = new List<SetCheckBox>();
            }
            else if (setType == "kong")
            {
                KongTerminalsHonorsCheckBoxes = new List<SetCheckBox>();
                KongConcealedCheckBoxes = new List<SetCheckBox>();
            }

            // use the selected index of the combobox
            for (var i = 0; i < comboBox.SelectedIndex; i++)
            {
                // create rows and add to the grid
                RowDefinition rd = new RowDefinition();
                grid.RowDefinitions.Insert(i, rd);

                // create 2 check boxes and a textblock for each row
                SetCheckBox terminalsHonorsCheckBox = new SetCheckBox();
                SetCheckBox concealedCheckBox = new SetCheckBox();
                TextBlock setNameTextBlock = new TextBlock();

                // set the SetType property for each check box ("pung" or "kong")
                terminalsHonorsCheckBox.SetType = setType;
                concealedCheckBox.SetType = setType;

                // not sure we need this property at all yet, JUST IN CASE!!!
                terminalsHonorsCheckBox.CheckBoxType = "terminalsHonors";
                concealedCheckBox.CheckBoxType = "concealed";

                terminalsHonorsCheckBox.Checked += SetCheckBox_Checked;
                concealedCheckBox.Checked += SetCheckBox_Checked;
                terminalsHonorsCheckBox.Unchecked += SetCheckBox_Unchecked;
                concealedCheckBox.Unchecked += SetCheckBox_Unchecked;

                // set the UI text for each item
                terminalsHonorsCheckBox.Content = "Terminals or honors";
                concealedCheckBox.Content = "Concealed";
                setNameTextBlock.Text = setType + " " + (i + 1) + ":";

                // set some styling on the textblock
                setNameTextBlock.VerticalAlignment = VerticalAlignment.Center;

                // check again if pungs or kongs, add generated check boxes to appropriate lists
                if (setType == "pung")
                {
                    PungTerminalsHonorsCheckBoxes.Add(terminalsHonorsCheckBox);
                    PungConcealedCheckBoxes.Add(concealedCheckBox);
                }
                else if (setType == "kong")
                {
                    KongTerminalsHonorsCheckBoxes.Add(terminalsHonorsCheckBox);
                    KongConcealedCheckBoxes.Add(concealedCheckBox);
                }

                // set each to appr. row and column
                Grid.SetRow(setNameTextBlock, i);
                Grid.SetRow(terminalsHonorsCheckBox, i);
                Grid.SetColumn(terminalsHonorsCheckBox, 1);
                Grid.SetRow(concealedCheckBox, i);
                Grid.SetColumn(concealedCheckBox, 2);

                // add each to the grid
                grid.Children.Add(terminalsHonorsCheckBox);
                grid.Children.Add(concealedCheckBox);
                grid.Children.Add(setNameTextBlock);
            }
        }


        /// <summary>
        /// A method that determines the winner, who dealt the winning tile (if anyone),
        /// and who was the dealer, then uses that info to process scores per player
        /// </summary>
        private void DetermineWinnerAndSetScores()
        {
            // create a StringBuilder instance to store the summary in
            game.CurrentRoundSummary = new StringBuilder();

            // initialize the current round score to the base score
            _currentBaseScore = ScoreValues.BASE_ROUND_SCORE;

            // PUNGS AND KONGS
            _terminalsHonorsPungsKongsCount = 0;
            foreach (SetCheckBox terminalsHonorsCheckbox in PungTerminalsHonorsCheckBoxes)
            {
                // create a temp int at the base score
                _currentPungValue = ScoreValues.BASE_PUNG_VALUE;

                // if it's terminal or honors, double it
                if (terminalsHonorsCheckbox.IsChecked == true)
                {
                    _currentPungValue *= 2;
                    _terminalsHonorsPungsKongsCount++;
                }
                    
                // if it's concealed, double it again
                if (PungConcealedCheckBoxes[PungTerminalsHonorsCheckBoxes.IndexOf(terminalsHonorsCheckbox)].IsChecked == true)
                    _currentPungValue *= 2;

                // add the pung value to the round score
                _currentBaseScore += _currentPungValue;

                // reset the current pung value int so we can use it again
                _currentPungValue = 0;
            }

            // same rules as above except for Kongs (lazy :P)
            foreach (SetCheckBox terminalsHonorsCheckBox in KongTerminalsHonorsCheckBoxes)
            {
                _currentKongValue = ScoreValues.BASE_KONG_VALUE;

                if (terminalsHonorsCheckBox.IsChecked == true)
                {
                    _currentKongValue *= 2;
                    _terminalsHonorsPungsKongsCount++;
                }
                   
                if (KongConcealedCheckBoxes[KongTerminalsHonorsCheckBoxes.IndexOf(terminalsHonorsCheckBox)].IsChecked == true)
                    _currentKongValue *= 2;

                _currentBaseScore += _currentKongValue;

                _currentKongValue = 0;
            }


            // need to check whether it was self drawn, and set concealed here, 
            // as a special case, because it can be points or a double
            // set it based on whether or not tile was self drawn this round
            // self drawn is always the last item in the combo box
            if (drawnFromComboBox.SelectedIndex == drawnFromComboBox.Items.Count - 1)
                game.WinConditions[2].IsDouble = true;
            else
                game.WinConditions[2].Score = ScoreValues.CONCEALED_SCORE;


            // SCORE VALUES
            foreach (WinCondition winCondition in winConditionsListView.SelectedItems)
            {
                if (winCondition.Score != null)
                    _currentBaseScore += winCondition.Score.Value;
            }


            // ROUND SCORE TO NEAREST 10 BEFORE APPLYING DOUBLES
            if (_currentBaseScore % 10 != 0)
                _currentBaseScore = ((int)Math.Round(_currentBaseScore / 10.0)) * 10;


            // APPLY DOUBLES TO SCORE
            foreach (WinCondition winCondition in winConditionsListView.SelectedItems)
            {
                if (winCondition.IsDouble)
                {
                    if (winConditionsListView.SelectedIndex == 6) // all one suit, 2 doubles
                        _currentBaseScore = ((_currentBaseScore * 2) * 2);
                    else if (winConditionsListView.SelectedIndex == 7) // one suit no honors, 4 doubles
                        _currentBaseScore = (((_currentBaseScore * 2) * 2) * 2);
                    else // all regular doubles
                        _currentBaseScore = _currentBaseScore * 2;
                }                  
            }

            // check for 3 concealed pungs or kongs, which is a double
            // special casing this because we can detect automatically (no need for player to select from list)
            _concealedPungsKongsCount = 0;
            foreach (SetCheckBox concealedPungs in PungConcealedCheckBoxes)
            {
                if (concealedPungs.IsChecked == true)
                    _concealedPungsKongsCount++;
            }

            foreach (SetCheckBox concealedKongs in KongConcealedCheckBoxes)
            {
                if (concealedKongs.IsChecked == true)
                    _concealedPungsKongsCount++;
            }

            if (_concealedPungsKongsCount >= 3)
            {
                _currentBaseScore = _currentBaseScore * 2;
            }
                
            // check for 4 terminals or honors sets, which is 2 doubles
            // special casing because we can detect automatically
            if (_terminalsHonorsPungsKongsCount == 4)
            {
                _currentBaseScore = (_currentBaseScore * 2) * 2;
            }

            // ENFORCE LIMIT
            // Mahjong hands have a max or "limit" value. Enforce that here
            if (_currentBaseScore > ScoreValues.MAX_ROUND_SCORE)
            {
                _currentBaseScore = ScoreValues.MAX_ROUND_SCORE;
                game.CurrentRoundSummary.AppendLine().AppendLine("This is a limit hand--meaning the winner's score was so high, it maxes out at " + 
                    ScoreValues.MAX_ROUND_SCORE + ".");
            }
                

            // SET WINNER. DEALER, AND PLAYER GONE OUT FROM
            for (var i = 0; i < game.Players.Count; i++)
            {
                // check if player is Dealer and set their property
                //if (dealerComboBox.SelectedIndex == i)
                //    game.Players[i].IsDealer = true;
                //else
                //    game.Players[i].IsDealer = false;

                // check if player is Round Winner and set their property
                if (winnerComboBox.SelectedIndex == i)
                    game.Players[i].IsRoundWinner = true;
                else
                    game.Players[i].IsRoundWinner = false;

                // check of the round winner was also the dealer this round
                if (game.Players[i].IsRoundWinner && game.Players[i].IsDealer)
                    _dealerWon = true;

                // check and set which person was drawn from (if any)
                // since drawn from combo box is dynamic, need to find the last item in the combo box
                if (drawnFromComboBox.SelectedIndex == drawnFromComboBox.Items.Count - 1 && _selfDrawn == false)
                    _selfDrawn = true;
                else
                {
                    // combo box is dynamic, and we know one item has been removed at "_ineligableDrawnFromPlayerIndex
                    // if the selected index is <, we know it will match i
                    if (drawnFromComboBox.SelectedIndex < _ineligableDrawnFromPlayerIndex)
                    {
                        if (drawnFromComboBox.SelectedIndex == i)
                            game.Players[i].DrawnFromThisRound = true;
                        else
                            game.Players[i].DrawnFromThisRound = false;
                    }
                    else
                    // if selected index >=, we know we need to subtract 1 to get i to match
                    {
                        if (drawnFromComboBox.SelectedIndex == i - 1)
                            game.Players[i].DrawnFromThisRound = true;
                        else
                            game.Players[i].DrawnFromThisRound = false;
                    }
                }
            }

            // based on the above, process the win conditions
            foreach (Player player in game.Players)
            {
                SetPlayerScore(player);
            }

            game.RoundSummaries.Add(game.CurrentRoundSummary.ToString());
        }


        /// <summary>
        /// Called in the DetermineWinnerAndSetScores method
        /// Processes scores per player based on selected conditions
        /// </summary>
        /// <param name="player"></param>
        private void SetPlayerScore(Player player)
        {
            if (_dealerWon) // dealer won so score is multiplied 6x
            {

                if (player.IsRoundWinner) // AWARD THE WINNER
                {
                    player.RoundScores.Add(_currentBaseScore * ScoreValues.X_ISWINNER_ISDEALER); // x6, positive

                    // add text info to the round summary
                    game.CurrentRoundSummary.AppendLine().AppendLine(player.Name + 
                        " won this round! Since they were also the dealer, they receive " + 
                        (_currentBaseScore * ScoreValues.X_ISWINNER_ISDEALER) + 
                        ": The base score of " + 
                        _currentBaseScore + 
                        " multiplied by 6.");

                    // add text about pungs and kongs
                    game.CurrentRoundSummary.AppendLine().AppendLine(player.Name + 
                        "'s hand included " + 
                        pungCountComboBox.SelectedIndex + 
                        " pung(s) and " + 
                        kongCountComboBox.SelectedIndex + 
                        " kong(s).");
                }
                else // TAKE FROM LOSERS
                {
                    // split up payment based on who winning tile was drawn from
                    if (_selfDrawn)
                    {
                        player.RoundScores.Add(_currentBaseScore * ScoreValues.X_ISLOSER_SELFDRAWN_DEALERWON); // x2, negative

                        // add text info to the round summary
                        game.CurrentRoundSummary.AppendLine().AppendLine(player.Name + 
                            " pays the winner " + 
                            ((_currentBaseScore * ScoreValues.X_ISLOSER_SELFDRAWN_DEALERWON) * -1) + 
                            ": The base score of " + _currentBaseScore + 
                            " doubled, because the winner was the dealer and the winning tile was self-drawn.");
                    }
                    else
                    {
                        if (player.DrawnFromThisRound)
                        {
                            player.RoundScores.Add(_currentBaseScore * ScoreValues.X_ISLOSER_ISDRAWNFROM_DEALERWON); // x6, negative

                            // add text info to the round summary
                            game.CurrentRoundSummary.AppendLine().AppendLine(player.Name + 
                                " pays for everybody because they coughed up the winning tile. They pay the winner the base score of " + 
                                _currentBaseScore + 
                                " multiplied by 6, because the winner was the dealer.");
                        }
                        else
                        {
                            // even though score change is 0, still need to add an entry in the player's RoundScores list
                            player.RoundScores.Add(0);
                        }
                    }
                }
            }
            else // non-dealer won so score is multiplied 4x
            {
                if (player.IsRoundWinner) // AWARD THE WINNER
                {
                    player.RoundScores.Add(_currentBaseScore * ScoreValues.X_ISWINNER); // x4, postive

                    // add text info to the round summary
                    game.CurrentRoundSummary.AppendLine().AppendLine(player.Name + 
                        " won this round! Since they were not the dealer, they receive " + 
                        (_currentBaseScore * ScoreValues.X_ISWINNER) + 
                        ": The base score of " + _currentBaseScore + 
                        " multiplied by 4.");

                    // add text about pungs and kongs
                    game.CurrentRoundSummary.AppendLine().AppendLine(player.Name + 
                        "'s hand included " + 
                        pungCountComboBox.SelectedIndex + 
                        " pung(s) and " + 
                        kongCountComboBox.SelectedIndex + 
                        " kong(s).");
                }
                else // TAKE FROM LOSERS
                {
                    // split up payment based on who winning tile was drawn from
                    if (_selfDrawn)
                    {
                        // if self drawn and a losing player was dealer, they pay double
                        if (player.IsDealer)
                        {
                            player.RoundScores.Add(_currentBaseScore * ScoreValues.X_ISLOSER_ISDEALER_SELFDRAWN); // x2, negative

                            // add text info to the round summary
                            game.CurrentRoundSummary.AppendLine().AppendLine(player.Name + 
                                " pays the winner the base score of " + 
                                _currentBaseScore + 
                                " multiplied by 2. They were the dealer and the winning tile was self-drawn, so they have to pay double.");
                        }
                        else
                        {
                            player.RoundScores.Add(_currentBaseScore * ScoreValues.X_ISLOSER_SELFDRAWN); // x1, negative

                            // add text info to the round summary
                            game.CurrentRoundSummary.AppendLine().AppendLine(player.Name + 
                                " pays the winner the base score of " + 
                                _currentBaseScore + 
                                ", because they were not the dealer and the winning tile was self-drawn.");
                        }                     
                    }
                    else
                    {
                        // if a player was drawn from, they pay for everyone
                        if (player.DrawnFromThisRound)
                        {
                            player.RoundScores.Add(_currentBaseScore * ScoreValues.X_ISLOSER_ISDRAWNFROM); // x4, negative

                            // add text info to the round summary
                            game.CurrentRoundSummary.AppendLine().AppendLine(player.Name + 
                                " pays for everybody because they coughed up the winning tile. They pay the winner the base score of " 
                                + _currentBaseScore + 
                                " multiplied by 4.");
                        }
                        else
                        {
                            // even though score change is 0, still need to add an entry in the player's RoundScores list
                            player.RoundScores.Add(0);
                        }
                    }
                }
            }

            // set each player's total score, -1 from currentround because the roundscores list indexes at 0
            player.TotalScore += player.RoundScores[game.CurrentRound - 1];
        }

        /// <summary>
        /// Called whenever we need to set up (or reset) the Win Conditions ListView
        /// Shows or hides certain items in the ListView, depending on checked boxes or ComboBox selections
        /// </summary>
        private void InitializeWinConditions()
        {
            PossibleWinConditions = new List<WinCondition>();
            bool showAllSimples = true;

            foreach (WinCondition winCondition in game.WinConditions)
            {
                switch (game.WinConditions.IndexOf(winCondition))
                {
                    // change "lucky pair" description based on prevailing wind
                    case 1:
                        winCondition.Description = "Winning pair is the prevailing wind (" + game.PrevailingWind + ") or the winner's lucky wind";
                        PossibleWinConditions.Add(winCondition);
                        break;

                    // only show "worthless" option if there are no pungs or kongs
                    case 3:
                        if (pungCountComboBox.SelectedIndex == 0 && kongCountComboBox.SelectedIndex == 0)
                            PossibleWinConditions.Add(winCondition);
                        break;

                    // Only show "all simples" win condition if we know there aren't any terminal/honor sets
                    case 4:
                        if (pungCountGrid.Visibility == Visibility.Visible)
                        {
                            foreach (SetCheckBox setCheckBox in PungTerminalsHonorsCheckBoxes)
                            {
                                if (setCheckBox.IsChecked == true)
                                {
                                    showAllSimples = false;
                                    break;
                                }
                            };
                        }

                        if (kongCountGrid.Visibility == Visibility.Visible)
                        {
                            foreach (SetCheckBox setCheckBox in KongTerminalsHonorsCheckBoxes)
                            {
                                if (setCheckBox.IsChecked == true)
                                {
                                    showAllSimples = false;
                                    break;
                                }
                            }
                        }

                        if (showAllSimples)
                            PossibleWinConditions.Add(winCondition);

                        break;

                    // only show "1-9 run" win condition if there are 1 or less sets
                    case 5:
                        if (!((pungCountComboBox.SelectedIndex > 1) ||
                            (kongCountComboBox.SelectedIndex > 1) ||
                            pungCountComboBox.SelectedIndex == 1 && kongCountComboBox.SelectedIndex == 1))
                        {
                            PossibleWinConditions.Add(winCondition);
                        }
                        break;

                    default:
                        PossibleWinConditions.Add(winCondition);
                        break;
                };


            }

            // set the item source of the listview to this Win Conditions List in the Game object
            winConditionsListView.ItemsSource = PossibleWinConditions;
        }


        private void DealerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            InitializeComboBoxWithNames(WinComboBoxStrings, winnerComboBox);
            winnerComboBox.Visibility = Visibility.Visible;
        }

        private void WinnerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // reset the drawn from combo box values in case something changes after the first selection
            InitializeDrawnFromComboBox();

            ComboBox WinnerComboBox = sender as ComboBox;

            // get the selected index of winner combobox, and remove that index from drawn from combobox
            for (var i = 0; i < WinnerComboBox.Items.Count; i++)
            {
                if (WinnerComboBox.SelectedIndex == i)
                {
                    DrawnComboBoxStrings.RemoveAt(i);
                    _ineligableDrawnFromPlayerIndex = i;
                }
            }

            // item source of drawn combobox can't be set until above item is removed
            drawnFromComboBox.ItemsSource = DrawnComboBoxStrings;

            //show drawn from combo box
            drawnFromComboBox.Visibility = Visibility.Visible;
        }

        private void DrawnFromComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            setsScoringStackPanel.Visibility = Visibility.Visible;


            InitializePossiblePungs();
        }


        private void pungCountComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;

            InitializePossibleKongs(cb);

            InitializeSetCheckBoxes(cb, pungCountGrid, "pung");

            pungCountGrid.Visibility = Visibility.Visible;
            kongCountComboBox.Visibility = Visibility.Visible;

            // preset kongs combobox to 0, since they are rare
            kongCountComboBox.SelectedIndex = 0;
        }


        private void kongCountComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            InitializeSetCheckBoxes(cb, kongCountGrid, "kong");
            kongCountGrid.Visibility = Visibility.Visible;

            InitializeWinConditions();

            pointsDoublesStackPanel.Visibility = Visibility.Visible;
            doneScoringButton.Visibility = Visibility.Visible;
        }


        private void SetCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // reset the win conditions list view, based on changes to the check boxes
            InitializeWinConditions();
        }

        private void SetCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // reset the win conditions list view, based on changes to the check boxes
            InitializeWinConditions();
        }




        private void DoneScoringButton_Click(object sender, RoutedEventArgs e)
        {
            DetermineWinnerAndSetScores();

            EndRound();

            

            Frame.Navigate(typeof(GameResultsPage), game);
        }

        private void EndRound()
        {
            if (_dealerWon)
                // increment property tracking how many times the dealer won
                game.TimesDealerWon++;
            else // only change dealer and prevailing wind if the dealer didn't win
            {
                // shift Winds and dealer counterclockwise around the table
                foreach (Player player in game.Players)
                {
                    switch (player.CurrentWind)
                    {
                        case Wind.East:
                            player.CurrentWind = Wind.South;
                            player.IsDealer = false;
                            break;
                        case Wind.South:
                            player.CurrentWind = Wind.West;
                            player.IsDealer = false;
                            break;
                        case Wind.West:
                            player.CurrentWind = Wind.North;
                            player.IsDealer = false;
                            break;
                        case Wind.North:
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
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Game)
            {
                game = e.Parameter as Game;
            }

            // increment round (starts at index 0, so 1st round = 1)
            game.CurrentRound++;

            // set up the first combobox (dealer)
            //dealerComboBox.Visibility = Visibility.Visible;
            //InitializeComboBoxWithNames(DealerComboBoxStrings, dealerComboBox);
            foreach (Player player in game.Players)
            {
                if (player.IsDealer)
                    dealerTextBlock.Text = player.Name;
            }

            prevailingWindTextBlock.Text = game.PrevailingWind.ToString();
            
            InitializeComboBoxWithNames(WinComboBoxStrings, winnerComboBox);
            winnerComboBox.Visibility = Visibility.Visible;

            base.OnNavigatedTo(e);

        }


    }
}
