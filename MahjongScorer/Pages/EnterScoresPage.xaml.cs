using System;
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

        // keep track of which player we removed from the drawn from combo box each round
        private int _ineligableDrawnFromPlayerIndex;


        private StringBuilder _specialRulesSummary = new StringBuilder();

        private StringBuilder _inProgressTips = new StringBuilder();
        


        // holders for combo box lists
        //private List<string> DealerComboBoxStrings;
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


        List<Rule> AllPossibleRules;

        List<Rule> PossibleCommonRules;
        List<Rule> PossibleUncommonRules;

        Set noSets;
        Set set1;
        Set set2;
        Set set3;
        Set set4;

        public EnterScoresPage()
        {
            this.InitializeComponent();
        }



        /// <summary>
        /// Initialize the winner combo box
        /// </summary>
        private void InitializeWinnerComboBox()
        {
            WinComboBoxStrings = new List<string>();

            // populate combobox lists with names
            foreach (Player player in game.Players)
            {
                WinComboBoxStrings.Add(player.Name);
            }

            // set the list as the Items Source for the combo box
            winnerComboBox.ItemsSource = WinComboBoxStrings;
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
        /// Calculates scores for the different sets. We'll run this method twice: Once for Pungs, once for Kongs
        /// </summary>
        /// <param name="TerminalsHonorsSetCheckBoxes"></param>
        /// <param name="ConcealedSetCheckBoxes"></param>
        /// <param name="baseValue"></param>
        private int CalculateSetScores(List<SetCheckBox> TerminalsHonorsSetCheckBoxes, List<SetCheckBox> ConcealedSetCheckBoxes, int baseValue)
        {
            int finalSetValue = 0;

            foreach (SetCheckBox terminalsHonorsCheckBox in TerminalsHonorsSetCheckBoxes)
            {
                // set the base value for this type of set
                int thisSetValue = baseValue;

                // if it's terminal or honors, double it
                if (terminalsHonorsCheckBox.IsChecked == true)
                    thisSetValue *= 2;

                // if it's concealed, double it
                if (ConcealedSetCheckBoxes[TerminalsHonorsSetCheckBoxes.IndexOf(terminalsHonorsCheckBox)].IsChecked == true)
                    thisSetValue *= 2;

                // add the final value to the total round base score
                finalSetValue += thisSetValue;
            }

            return finalSetValue;
        }


        /// <summary>
        /// Adds text to the special rules summary, which is later added to the current round summary
        /// </summary>
        /// <param name="summary"></param>
        private void AddToSpecialRulesSummary(string summary)
        {
            // if summary already has text, add a space between sentences
            if (_specialRulesSummary.Length > 0)
                _specialRulesSummary.Insert(_specialRulesSummary.Length, " ");

            // add the next sentence of summary text
            _specialRulesSummary.Insert(_specialRulesSummary.Length, summary);
        }


        /// <summary>
        /// Adds text about the winner's pungs and kongs to the current round summary
        /// </summary>
        /// <param name="player">takes an instance of Player as a parameter</param>
        private void AddPungKongSummary(Player player)
        {
            // TODO: add concealed and terminals/honors info to summary
            if (pungCountComboBox.SelectedIndex > 0)
            {
                if (kongCountComboBox.SelectedIndex > 0)
                {
                    game.CurrentRoundSummary.Insert(game.CurrentRoundSummary.Length, player.Name +
                        "'s hand included " +
                        pungCountComboBox.SelectedIndex +
                        " pung(s) and " +
                        kongCountComboBox.SelectedIndex +
                        " kong(s).");
                }
                else
                {
                    game.CurrentRoundSummary.Insert(game.CurrentRoundSummary.Length, player.Name +
                        "'s hand included " +
                        pungCountComboBox.SelectedIndex +
                        " pung(s).");
                }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="reset"></param>
        private void GenerateInProgressTips()
        {
            inProgressTipsGrid.Visibility = Visibility.Visible;
            _inProgressTips = new StringBuilder();


            if (DealerWon())
                _inProgressTips.Insert(
                    _inProgressTips.Length, 
                    winnerComboBox.SelectedItem + " wins and is also the dealer, so their score will be multiplied by <span style='color: blue;'6</span>."
                    );
            else
                _inProgressTips.Insert(
                    _inProgressTips.Length, 
                    winnerComboBox.SelectedItem + " wins and is not the dealer, so their score will be multiplied by 4."
                    );


            if (SelfDrawn() && !Worthless())
            {
                _inProgressTips.Append(Environment.NewLine);
                _inProgressTips.Append(Environment.NewLine);

                _inProgressTips.Insert(
                    _inProgressTips.Length, 
                    "The winning tile is Self-Drawn, which will give " + 
                    winnerComboBox.SelectedItem + 
                    " 2 extra points."
                    );
            } else if (PlayerDrawnFrom() != null)
            {
                _inProgressTips.Append(Environment.NewLine);
                _inProgressTips.Append(Environment.NewLine);

                if (DealerWon())
                {
                    _inProgressTips.Insert(
                        _inProgressTips.Length,
                        "The winning tile was drawn from " + 
                        PlayerDrawnFrom().Name + 
                        ". Since the dealer also won, they will pay the winning score multiplied by 6."
                        );
                }
                else
                {
                    _inProgressTips.Insert(
                        _inProgressTips.Length,
                        "The winning tile was drawn from " +
                        PlayerDrawnFrom().Name +
                        ". They will pay the winning score multiplied by 4."
                        );
                }              
            }


            if (Worthless())
            {
                _inProgressTips.Append(Environment.NewLine);
                _inProgressTips.Append(Environment.NewLine);

                _inProgressTips.Insert(
                    _inProgressTips.Length,
                    "This hand is currently Worthless, because there are no sets or Lucky Pair. " +
                    winnerComboBox.SelectedItem +
                    " will get a Double for this. "
                    );

                _inProgressTips.Append(Environment.NewLine);
                _inProgressTips.Append(Environment.NewLine);

                if (SelfDrawn() && OneChance())
                    _inProgressTips.Insert(_inProgressTips.Length, "Self-Drawn and One Chance points will not count, because the hand is Worthless.");
                else if (SelfDrawn())
                    _inProgressTips.Insert(_inProgressTips.Length, "Self-Drawn points will not count because the hand is Worthless.");
                else if (OneChance())
                    _inProgressTips.Insert(_inProgressTips.Length, "One Chance points will not count because the hand is Worthless.");
            }

            if (pungCountComboBox.SelectedItem != null)
            {
                ShowInProgressPlayerScores();
            }
            

            inProgressTipsText.Text = _inProgressTips.ToString();        
        }


        private void ShowInProgressPlayerScores()
        {
            _inProgressTips.Append(Environment.NewLine);
            _inProgressTips.Append(Environment.NewLine);
            _inProgressTips.Insert(_inProgressTips.Length, "Current Scores:");
            _inProgressTips.Append(Environment.NewLine);

            foreach (Player player in game.Players)
            {
                if (player == RoundWinner())
                    _inProgressTips.Insert(_inProgressTips.Length, player.Name + ": " + (player.TotalScore + CurrentBaseScore())); 

                _inProgressTips.Append(Environment.NewLine);
            }
        }

        private void GenerateRoundSummary()
        {
            int finalBaseScore = CurrentBaseScore();

            // create a StringBuilder instance to store the summary in
            game.CurrentRoundSummary = new StringBuilder();

            // show winner's score and info, based on whether they're dealer or not
            if (DealerWon())
            {
                game.CurrentRoundSummary.Insert(game.CurrentRoundSummary.Length, RoundWinner().Name + 
                    " won this round! Since they were also the dealer, they receive " +
                    (finalBaseScore * ScoreValues.X_ISWINNER_ISDEALER) +
                    ": The base score of " +
                    finalBaseScore +
                    " multiplied by 6.");

                InsertRoundSummaryParaBreak();

                // if not self drawn, show drawn from player's score and info
                if (!SelfDrawn())
                {
                    game.CurrentRoundSummary.Insert(game.CurrentRoundSummary.Length, PlayerDrawnFrom().Name +
                        " pays the winner " +
                        ((finalBaseScore * ScoreValues.X_ISLOSER_ISDRAWNFROM_DEALERWON) * -1) +
                        ": The base score of " + finalBaseScore +
                        " multiplied by 6, because they coughed up the winning tile.");
                }
                else
                {
                    // self drawn, so everybody pays x2
                    game.CurrentRoundSummary.Insert(game.CurrentRoundSummary.Length,
                        "Since the winning tile was self-drawn, everybody else pays " +
                        RoundWinner().Name + " " +
                        ((finalBaseScore * ScoreValues.X_ISLOSER_SELFDRAWN_DEALERWON) * -1) +
                        ": The base score of " + finalBaseScore +
                        " multiplied by 2.");
                }
            }
            else
            {
                // dealer didn't win, show their info
                game.CurrentRoundSummary.Insert(game.CurrentRoundSummary.Length, RoundWinner().Name + 
                    " won this round! Since they were not the dealer, they receive " +
                    (finalBaseScore * ScoreValues.X_ISWINNER) +
                    ": The base score of " + finalBaseScore +
                    " multiplied by 4.");

                InsertRoundSummaryParaBreak();

                // if not self drawn, show drawn from player's score and info
                if (!SelfDrawn())
                {
                    game.CurrentRoundSummary.Insert(game.CurrentRoundSummary.Length, PlayerDrawnFrom().Name +
                        " pays the winner " +
                        ((finalBaseScore * ScoreValues.X_ISLOSER_ISDRAWNFROM) * -1) +
                        ": The base score of " + finalBaseScore +
                        " multiplied by 4, because they coughed up the winning tile.");
                }
                else
                {
                    // self drawn, so dealer pays x2 and everybody else pays base score
                    game.CurrentRoundSummary.Insert(game.CurrentRoundSummary.Length, RoundDealer().Name + 
                        " pays " + 
                        RoundWinner().Name + " " +
                        ((finalBaseScore * ScoreValues.X_ISLOSER_ISDEALER_SELFDRAWN) * -1) +
                        ": The base score of " + finalBaseScore + 
                        " multiplied by 2, because " + 
                        RoundDealer().Name + 
                        " was the dealer. Everybody else pays " + 
                        RoundWinner().Name + " " +
                        finalBaseScore + ".");
                }
            }

            InsertRoundSummaryParaBreak();

            // add round winner's pung/kong info
            AddPungKongSummary(RoundWinner());

            InsertRoundSummaryParaBreak();

            // add all special rules summaries TODO: is there a better way to do this?
            if (_specialRulesSummary != null)
            {
                game.CurrentRoundSummary.Insert(game.CurrentRoundSummary.Length, _specialRulesSummary.ToString());
                InsertRoundSummaryParaBreak();
            }

            // finalize the current round summary by adding it to the List
            game.RoundSummaries.Add(game.CurrentRoundSummary.ToString());
        }

        /// <summary>
        /// Just add a paragraph break into the CurrentRoundSummary String Builder
        /// </summary>
        private void InsertRoundSummaryParaBreak()
        {
            game.CurrentRoundSummary.Append(Environment.NewLine);
            game.CurrentRoundSummary.Append(Environment.NewLine);
        }

        private int CurrentBaseScore()
        {   
            // set base value
            int baseScore = ScoreValues.BASE_ROUND_SCORE;

            // PUNGS AND KONGS
            // check which pung check boxes are checked, and adjust the score
             baseScore += CalculateSetScores(PungTerminalsHonorsCheckBoxes, PungConcealedCheckBoxes, ScoreValues.BASE_PUNG_VALUE);

            // check which kong check boxes are checked, and adjust the score
            baseScore += CalculateSetScores(KongTerminalsHonorsCheckBoxes, KongConcealedCheckBoxes, ScoreValues.BASE_KONG_VALUE);

            // APPLY SCORE VALUES AND DOUBLES
            baseScore = ApplySelectedRuleValues(baseScore);
            baseScore = ApplySelfDrawnPoints(baseScore);
            baseScore = ApplySelectedRuleDoubles(baseScore);
            baseScore = ApplyAutomaticRuleDoubles(baseScore);

            // ROUND SCORE TO NEAREST 10 AFTER APPLYING DOUBLES
            if (baseScore % 10 != 0)
                baseScore = ((int)Math.Round(baseScore / 10.0)) * 10;

            // ENFORCE LIMIT
            // Mahjong hands have a max or "limit" value. Enforce that here
            if (baseScore > ScoreValues.MAX_ROUND_SCORE)
            {
                baseScore = ScoreValues.MAX_ROUND_SCORE;
                game.CurrentRoundSummary.AppendLine().AppendLine("This hand has reached the max limit of " +
                    ScoreValues.MAX_ROUND_SCORE + ".");
            }

            return baseScore;
        }


        /// <summary>
        /// A method that determines the winner, who dealt the winning tile (if anyone),
        /// and who was the dealer, then uses that info to process scores per player
        /// </summary>
        private void SetFinalScores()
        {
            // get the final base score
            int _finalBaseScore = CurrentBaseScore();

            // process each player's score using the round's final base score
            foreach (Player player in game.Players)
            {
                SetPlayerScore(player, _finalBaseScore);
            }

            // create the round summary and add it to the round summaries list
            GenerateRoundSummary();
        }


        /// <summary>
        /// Calculate whether or not winning hand is worthless
        /// </summary>
        /// <returns>true if worthless hand, false if not</returns>
        private bool Worthless()
        {
            if (pungCountComboBox.SelectedIndex == 0 &&
                kongCountComboBox.SelectedIndex == 0 &&
                !commonRulesListView.SelectedItems.Contains(game.Rules[2]))
                return true;
            else
                return false;
        }

        private bool OneChance()
        {
            if (commonRulesListView.SelectedItems.Contains(game.Rules[1]))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Calculate whether or not winning tile was selfdrawn
        /// </summary>
        /// <returns>true if selfdrawn, false if not</returns>
        private bool SelfDrawn()
        {
            if (drawnFromComboBox.SelectedIndex == drawnFromComboBox.Items.Count - 1)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Calculate whether or not the dealer won this round
        /// </summary>
        /// <returns>true if dealer won, false if not</returns>
        private bool DealerWon()
        {
            for (var i = 0; i < game.Players.Count; i++)
            {
                // check if the Round Winner is also the round dealer
                if (winnerComboBox.SelectedIndex == i && game.Players[i].IsDealer)
                    return true;
            }
            return false;
        }


        /// <summary>
        /// Calculate round winner based on combo box selectiom
        /// </summary>
        /// <returns>the player who is the round winner</returns>
        private Player RoundWinner()
        {
            if (winnerComboBox.SelectedItem != null)
            {
                foreach (Player player in game.Players)
                {
                    if (winnerComboBox.SelectedIndex == game.Players.IndexOf(player))
                        return player;
                }
            }
            return null;
        }

        /// <summary>
        /// Calculate which player was drawn from
        /// If none (selfdrawn), return null
        /// </summary>
        /// <returns>the player drawn from or null</returns>
        private Player PlayerDrawnFrom()
        {
            if (!SelfDrawn() && drawnFromComboBox.SelectedItem != null)
            {
                foreach (Player player in game.Players)
                {
                    // since self drawn is false, we know one item has been removed at _ineligableDrawnFromPlayerIndex
                    // if the selected index is <, we know it will match player's index
                    if (drawnFromComboBox.SelectedIndex < _ineligableDrawnFromPlayerIndex)
                    {
                        if (drawnFromComboBox.SelectedIndex == game.Players.IndexOf(player))
                            return player;
                    }
                    else
                    // if selected index >=, we know we need to subtract 1 to get player's index to match
                    {
                        if (drawnFromComboBox.SelectedIndex == game.Players.IndexOf(player) - 1)
                            return player;
                    }
                } 
            }
            return null;
        }

        /// <summary>
        /// Calculates which player is the current round dealer
        /// </summary>
        /// <returns>the dealer of this round</returns>
        private Player RoundDealer()
        {
            foreach (Player player in game.Players)
            {
                if (player.IsDealer)
                    return player;

            }
            return null;
        }

        /// <summary>
        /// Go through selected Rules and apply point values
        /// </summary>
        private int ApplySelectedRuleValues(int baseScore)
        {
            foreach (Rule rule in commonRulesListView.SelectedItems)
            {
                if (rule.Score != null) // if a rule is selected and has a Score value
                {
                    // as special case, need to first check one chance and worthless
                    if (Worthless() && rule == game.Rules[1])
                    {
                        AddToSpecialRulesSummary("Since this hand is worthless, points for " + rule.Name + " are not added to the score.");
                    }
                    else
                    {
                        // add points
                        baseScore += rule.Score.Value;

                        // add descripition to the summary
                        AddToSpecialRulesSummary(rule.Score + " is added to the base score because " + rule.Description + ".");
                    }                   
                }
            }

            foreach (Rule rule in uncommonRulesListView.SelectedItems)
            {
                if (rule.Score != null) // if a rule is selected and has a Score value
                {
                    // add points
                    baseScore += rule.Score.Value;

                    // add descripition to the summary
                    AddToSpecialRulesSummary(rule.Score + " is added to the base score because " + rule.Description + ".");
                }
            }

            return baseScore;
        }


        /// <summary>
        /// Set if winning tile was selfdrawn, and apply points
        /// self drawn is index 0 in the game.Rules list
        ///  if last item in the combo box is selected, the user has selected "self drawn" option
        /// </summary>
        private int ApplySelfDrawnPoints(int baseScore)
        {
            if (SelfDrawn())
            {
                // only apply self drawn points if hand isn't worthless
                if (!Worthless())
                {
                    baseScore += game.Rules[0].Score.Value;
                    AddToSpecialRulesSummary(game.Rules[0].Score + " is added to the base score because " + game.Rules[0].Description + ".");
                }
                else
                {
                    AddToSpecialRulesSummary("Even though winning tile was self-drawn, " + game.Rules[0].Score + " points are not added because the hand was worthless.");
                }
            }

            return baseScore;
        }


        /// <summary>
        /// Go through selected Rules and apply doubles
        /// </summary>
        private int ApplySelectedRuleDoubles(int baseScore)
        {
            foreach (Rule rule in commonRulesListView.SelectedItems)
            {
                // for all selected rules, current score gets doubled as many times per it's Double value
                baseScore = DoubleScore(baseScore, rule.Double);

                // add to summary based on Double value
                switch (rule.Double)
                {
                    case 1:
                        AddToSpecialRulesSummary(
                            "Score was doubled because " + 
                            rule.Description + 
                            " (" + rule.Name + ")."
                            );
                        break;
                    case 2:
                        AddToSpecialRulesSummary(
                            "Score was doubled twice because " + 
                            rule.Description + 
                            " (" + rule.Name + ")."
                            );
                        break;
                    case 4:
                        AddToSpecialRulesSummary(
                            "Score was doubled four times because " + 
                            rule.Description + 
                            " (" + rule.Name + ")."
                            );
                        break;
                }
            }

            foreach (Rule rule in uncommonRulesListView.SelectedItems)
            {
                // for all selected rules, current score gets doubled as many times per it's Double value
                baseScore = DoubleScore(baseScore, rule.Double);

                // add to summary based on Double value
                switch (rule.Double)
                {
                    case 1:
                        AddToSpecialRulesSummary("Score was doubled because " + rule.Description + ".");
                        break;
                    case 2:
                        AddToSpecialRulesSummary("Score was doubled twice because " + rule.Description + ".");
                        break;
                    case 4:
                        AddToSpecialRulesSummary("Score was doubled four times because " + rule.Description + ".");
                        break;
                }
            }

            return baseScore;
        }



        /// <summary>
        /// Go through all Rules and apply doubles for special cases
       ///  where we can detect automatically, without asking the user
        /// </summary>
        private int ApplyAutomaticRuleDoubles(int baseScore)
        {
            foreach (Rule rule in game.Rules)
            {
                if (!rule.ShowInList)
                {
                    switch (game.Rules.IndexOf(rule))
                    {
                        // Worthless hand - apply double if there are no pungs or kongs, and no lucky pair
                        // if this is active, "one chance" and "self drawn" points must be subtracted
                        case 5:
                            if (Worthless())
                            {
                                baseScore = DoubleScore(baseScore, rule.Double);
                                AddToSpecialRulesSummary("Because this is a worthless hand, the score is doubled.");
                            }
                            break;
                        // Three concealed pungs - apply double if there are 3 or more concealed pungs or kongs
                        case 8:
                            if (ConcealedPungsKongs() >= 3)
                            {
                                baseScore = DoubleScore(baseScore, rule.Double);
                                AddToSpecialRulesSummary("Score was doubled twice because " + rule.Description + ".");
                            }
                            break;

                        // all pungs - apply double if there are a total of 4 pungs/kongs
                        case 20:
                            if (pungCountComboBox.SelectedIndex + kongCountComboBox.SelectedIndex == 4)
                            {
                                baseScore = DoubleScore(baseScore, rule.Double);
                                AddToSpecialRulesSummary("Score was doubled twice because " + rule.Description + ".");
                            }
                            break;

                        // three kongs - apply double if there are 3 or more kongs
                        case 22:
                            if (kongCountComboBox.SelectedIndex >= 3)
                            {
                                baseScore = DoubleScore(baseScore, rule.Double);
                                AddToSpecialRulesSummary("Score was doubled twice because " + rule.Description + ".");
                            }
                            break;
                    }
                }
            }

            return baseScore;
        }


        private int DoubleScore(int score, int timesDoubled)
        {
            for (int i = 0; i < timesDoubled; i++)
            {
                score *= 2;
            }
            return score;
        }

        /// <summary>
        /// Called in the DetermineWinnerAndSetScores method
        /// Processes scores per player based on selected conditions
        /// </summary>
        /// <param name="player"></param>
        private void SetPlayerScore(Player player, int finalBaseScore)
        {
            if (DealerWon()) // dealer won so score is multiplied 6x
            {

                if (player == RoundWinner()) // AWARD THE WINNER
                {
                    player.RoundScores.Add(finalBaseScore * ScoreValues.X_ISWINNER_ISDEALER); // x6, positive
                }
                else // TAKE FROM LOSERS
                {
                    // split up payment based on who winning tile was drawn from
                    if (SelfDrawn())
                    {
                        player.RoundScores.Add(finalBaseScore * ScoreValues.X_ISLOSER_SELFDRAWN_DEALERWON); // x2, negative
                    }
                    else
                    {
                        if (player == PlayerDrawnFrom())
                        {
                            player.RoundScores.Add(finalBaseScore * ScoreValues.X_ISLOSER_ISDRAWNFROM_DEALERWON); // x6, negative
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
                if (player == RoundWinner()) // AWARD THE WINNER
                {
                    player.RoundScores.Add(finalBaseScore * ScoreValues.X_ISWINNER); // x4, postive
                }
                else // TAKE FROM LOSERS
                {
                    // split up payment based on who winning tile was drawn from
                    if (SelfDrawn())
                    {
                        // if self drawn and a losing player was dealer, they pay double
                        if (player.IsDealer)
                        {
                            player.RoundScores.Add(finalBaseScore * ScoreValues.X_ISLOSER_ISDEALER_SELFDRAWN); // x2, negative
                        }
                        else
                        {
                            player.RoundScores.Add(finalBaseScore * ScoreValues.X_ISLOSER_SELFDRAWN); // x1, negative
                        }                     
                    }
                    else
                    {
                        // if a player was drawn from, they pay for everyone
                        if (player == PlayerDrawnFrom())
                        {
                            player.RoundScores.Add(finalBaseScore * ScoreValues.X_ISLOSER_ISDRAWNFROM); // x4, negative
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
        /// Returns the count of concealed pungs and kongs, based on selected check boxes
        /// </summary>
        /// <returns></returns>
        private int ConcealedPungsKongs()
        {
            int concealedPungsKongsCount = 0;
            foreach (SetCheckBox concealedPungs in PungConcealedCheckBoxes)
            {
                if (concealedPungs.IsChecked == true)
                    concealedPungsKongsCount++;
            }

            foreach (SetCheckBox concealedKongs in KongConcealedCheckBoxes)
            {
                if (concealedKongs.IsChecked == true)
                    concealedPungsKongsCount++;
            }

            return concealedPungsKongsCount;
        }

        /// <summary>
        /// Returns the count of terminals/honors pungs and kongs, based on selected check boxes
        /// </summary>
        /// <returns></returns>
        private int TerminalsHonorsPungsKongs()
        {
            int terminalsHonorsPungsKongsCount = 0;
            foreach (SetCheckBox terminalsHonorsPungs in PungTerminalsHonorsCheckBoxes)
            {
                if (terminalsHonorsPungs.IsChecked == true)
                    terminalsHonorsPungsKongsCount++;
            }

            foreach (SetCheckBox terminalsHonorsKongs in KongTerminalsHonorsCheckBoxes)
            {
                if (terminalsHonorsKongs.IsChecked == true)
                    terminalsHonorsPungsKongsCount++;
            }

            return terminalsHonorsPungsKongsCount;
        }

        /// <summary>
        /// Called whenever we need to set up (or reset) the Rules ListView
        /// Shows or hides certain items in the ListView, depending on checked boxes or ComboBox selections
        /// </summary>
        private void InitializeRules()
        {
            // set up list of possible rules
            AllPossibleRules = new List<Rule>();

            PossibleCommonRules = new List<Rule>();
            PossibleUncommonRules = new List<Rule>();

            foreach (Rule rule in game.Rules)
            {
                // switch statement checks all rules that are "sometimes" shown, and determines whether they should be shown
                switch (game.Rules.IndexOf(rule))
                {
                    // "Lucky pair" - change description based on prevailing wind
                    case 2:
                        rule.Description = "the winning pair is the prevailing wind (" + game.PrevailingWind + "), lucky wind, or dragons";

                        break;

                    // "Semi-concealed" (partial) - show if there are less than 2 unconcealed sets, and winning tile is NOT self drawn
                    case 3:
                        if ((pungCountComboBox.SelectedIndex +
                            kongCountComboBox.SelectedIndex) < (ConcealedPungsKongs() + 2) &&
                            drawnFromComboBox.SelectedIndex != drawnFromComboBox.Items.Count - 1)
                            rule.ShowInList = true;
                        else
                            rule.ShowInList = false;

                        break;

                    // "Concealed" (fully) - show if all pungs/kongs are concealed and winning tile IS self drawn
                    case 4:
                        if (((pungCountComboBox.SelectedIndex +
                            kongCountComboBox.SelectedIndex) == ConcealedPungsKongs()) &&
                            ConcealedPungsKongs() > 0 &&
                            drawnFromComboBox.SelectedIndex == drawnFromComboBox.Items.Count - 1)
                            rule.ShowInList = true;
                        else
                            rule.ShowInList = false;

                        break;

                    // "All simples" - show if there are no terminals/honors pungs or kongs
                    case 6:
                        if (TerminalsHonorsPungsKongs() == 0)
                            rule.ShowInList = true;
                        else
                            rule.ShowInList = false;

                        break;

                    // 1-9 run - show if there are 1 or less sets
                    case 7:
                        if (!((pungCountComboBox.SelectedIndex > 1) ||
                            (kongCountComboBox.SelectedIndex > 1) ||
                            pungCountComboBox.SelectedIndex == 1 && kongCountComboBox.SelectedIndex == 1))
                            rule.ShowInList = true;
                        else
                            rule.ShowInList = false;

                        break;

                    // one suit no honors - show if there are no terminals/honors pungs or kongs
                    case 10:
                        if (TerminalsHonorsPungsKongs() == 0)
                            rule.ShowInList = true;
                        else
                            rule.ShowInList = false;

                        break;

                    // all honors - show if every set has a terminal or honor
                    case 11:
                        if ((pungCountComboBox.SelectedIndex +
                            kongCountComboBox.SelectedIndex) == TerminalsHonorsPungsKongs())
                            rule.ShowInList = true;
                        else
                            rule.ShowInList = false;
                        break;

                    // lucky set - show if if at least one pung or kong has terminals/honors checked
                    case 12:
                        if (TerminalsHonorsPungsKongs() > 0)
                            rule.ShowInList = true;
                        else
                            rule.ShowInList = false;
                        break;

                    // double lucky set - show if if at least one pung or kong has terminals/honors checked
                    case 13:
                        if (TerminalsHonorsPungsKongs() > 0)
                            rule.ShowInList = true;
                        else
                            rule.ShowInList = false;
                        break;

                    // off the dead wall - show if there's at least one kong
                    case 14:
                        if (kongCountComboBox.SelectedIndex > 0)
                            rule.ShowInList = true;
                        else
                            rule.ShowInList = false;
                        break;

                    // all pairs - show if there are no pungs or kongs
                    case 18:
                        if (pungCountComboBox.SelectedIndex == 0 && kongCountComboBox.SelectedIndex == 0)
                            rule.ShowInList = true;
                        else
                            rule.ShowInList = false;
                        break;

                    // triple pung - show if there are 3 or more pungs or kongs
                    case 19:
                        if (pungCountComboBox.SelectedIndex + kongCountComboBox.SelectedIndex >= 3)
                            rule.ShowInList = true;
                        else
                            rule.ShowInList = false;
                        break;

                    // little three dragons - show if there are at least 2 pungs or kongs that are terminals/honors
                    case 21:
                        if (TerminalsHonorsPungsKongs() >= 2)
                            rule.ShowInList = true;
                        else
                            rule.ShowInList = false;
                        break;
                };

                // add all eligible rules to the list
                if (rule.ShowInList == true)
                    AllPossibleRules.Add(rule);
            }


            foreach (Rule rule in AllPossibleRules)
            {
                if (rule.IsUncommon)
                    PossibleUncommonRules.Add(rule);
                else
                    PossibleCommonRules.Add(rule);
            }

            // set the item source of the listview to this Win Conditions List in the Game object
            commonRulesListView.ItemsSource = PossibleCommonRules;

            uncommonRulesListView.ItemsSource = PossibleUncommonRules;
        }


        /// <summary>
        /// 
        /// </summary>
        private void EndRound()
        {
            if (DealerWon())
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

                game.LoadedFromSave = false;
            }
        }


        //private void DealerComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        //{
        //    InitializeWinnerComboBox();
        //    winnerComboBox.Visibility = Visibility.Visible;

        //    GenerateinProgressSummary();
        //}

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

            GenerateInProgressTips();
        }

        private void DrawnFromComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            setsScoringStackPanel.Visibility = Visibility.Visible;

            InitializePossiblePungs();

            GenerateInProgressTips();
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

            GenerateInProgressTips();
        }


        private void kongCountComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;
            InitializeSetCheckBoxes(cb, kongCountGrid, "kong");
            kongCountGrid.Visibility = Visibility.Visible;

            InitializeRules();
            pointsDoublesStackPanel.Visibility = Visibility.Visible;

            doneScoringButton.Visibility = Visibility.Visible;

            GenerateInProgressTips();
        }


        private void SetCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // reset the win conditions list view, based on changes to the check boxes
            InitializeRules();
        }

        private void SetCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // reset the win conditions list view, based on changes to the check boxes
            InitializeRules();
        }


        private void DoneScoringButton_Click(object sender, RoutedEventArgs e)
        {
            SetFinalScores();

            EndRound();          

            Frame.Navigate(typeof(GameResultsPage), game, new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
        }

        private void RulesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            GenerateInProgressTips();
        }

        

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // always hide the back button on this page
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;

            if (e.Parameter is Game)
            {
                game = e.Parameter as Game;
            }

            // increment round (starts at index 0, so 1st round = 1)
            game.CurrentRound++;


            foreach (Player player in game.Players)
            {
                if (player.IsDealer)
                    dealerTextBlock.Text = player.Name;
            }

            prevailingWindTextBlock.Text = game.PrevailingWind.ToString();
            
            InitializeWinnerComboBox();
            winnerComboBox.Visibility = Visibility.Visible;

            base.OnNavigatedTo(e);

        }
    }
}
