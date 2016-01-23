using System;
using System.Collections.Generic;
using System.Text;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Media.Animation;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MahjongScorer.Pages
{
    /// <summary>
    /// Page where scores are entered for a round
    /// </summary>
    public sealed partial class EnterScoresPage : Page
    {
        // holder for local Game object
        private Game game;

        // keep track of which player we removed from the drawn from combo box each round
        private int _ineligableDrawnFromPlayerIndex;

        // summary String Builders
        private StringBuilder _specialRulesSummary = new StringBuilder();
        private StringBuilder _inProgressTips = new StringBuilder();
        
        // holders for combo box lists
        private List<string> WinComboBoxStrings;
        private List<string> DrawnComboBoxStrings;

        // set check box lists
        private List<CheckBox> PungTerminalsHonorsCheckBoxes;
        private List<CheckBox> PungConcealedCheckBoxes;
        private List<CheckBox> KongTerminalsHonorsCheckBoxes;
        private List<CheckBox> KongConcealedCheckBoxes;

        // set radio button lists
        private List<RadioButton> PungNormalRadioButtons;
        private List<RadioButton> PungLuckyRadioButtons;
        private List<RadioButton> PungDoubleLuckyRadioButtons;
        private List<RadioButton> KongNormalRadioButtons;
        private List<RadioButton> KongLuckyRadioButtons;
        private List<RadioButton> KongDoubleLuckyRadioButtons;

        // Set Lists
        List<Set> PossiblePungs;
        List<Set> PossibleKongs;

        // Rule Lists
        List<Rule> AllPossibleRules;
        List<Rule> PossibleCommonRules;
        List<Rule> PossibleUncommonRules;

        //Set objects (we start with 4 and there can only ever be 4 total)
        Set noSets;
        Set set1;
        Set set2;
        Set set3;
        Set set4;

        bool setRadioButtonsShown;

        public EnterScoresPage()
        {
            this.InitializeComponent();
        }

        #region "INITIALIZE" METHODS

        /// <summary>
        /// Add XAML check box and radio button elements to Lists
        /// </summary>
        private void InitializeSetElementLists()
        {
            setRadioButtonsShown = false;

            PungTerminalsHonorsCheckBoxes = new List<CheckBox>();
            PungConcealedCheckBoxes = new List<CheckBox>();
            KongTerminalsHonorsCheckBoxes = new List<CheckBox>();
            KongConcealedCheckBoxes = new List<CheckBox>();
            PungNormalRadioButtons = new List<RadioButton>();
            PungLuckyRadioButtons = new List<RadioButton>();
            PungDoubleLuckyRadioButtons = new List<RadioButton>();
            KongNormalRadioButtons = new List<RadioButton>();
            KongLuckyRadioButtons = new List<RadioButton>();
            KongDoubleLuckyRadioButtons = new List<RadioButton>();

            // add pung check boxes to Lists
            PungTerminalsHonorsCheckBoxes.Add(pungOneTerminalsHonorsCheckBox);
            PungTerminalsHonorsCheckBoxes.Add(pungTwoTerminalsHonorsCheckBox);
            PungTerminalsHonorsCheckBoxes.Add(pungThreeTerminalsHonorsCheckBox);
            PungTerminalsHonorsCheckBoxes.Add(pungFourTerminalsHonorsCheckBox);
            PungConcealedCheckBoxes.Add(pungOneConcealedCheckBox);
            PungConcealedCheckBoxes.Add(pungTwoConcealedCheckBox);
            PungConcealedCheckBoxes.Add(pungThreeConcealedCheckBox);
            PungConcealedCheckBoxes.Add(pungFourConcealedCheckBox);

            // add kong check boxes to Lists
            KongTerminalsHonorsCheckBoxes.Add(kongOneTerminalsHonorsCheckBox);
            KongTerminalsHonorsCheckBoxes.Add(kongTwoTerminalsHonorsCheckBox);
            KongTerminalsHonorsCheckBoxes.Add(kongThreeTerminalsHonorsCheckBox);
            KongTerminalsHonorsCheckBoxes.Add(kongFourTerminalsHonorsCheckBox);
            KongConcealedCheckBoxes.Add(kongOneConcealedCheckBox);
            KongConcealedCheckBoxes.Add(kongTwoConcealedCheckBox);
            KongConcealedCheckBoxes.Add(kongThreeConcealedCheckBox);
            KongConcealedCheckBoxes.Add(kongFourConcealedCheckBox);

            // add pung radio buttons to Lists
            PungNormalRadioButtons.Add(pungOneNormalRadioButton);
            PungNormalRadioButtons.Add(pungTwoNormalRadioButton);
            PungNormalRadioButtons.Add(pungThreeNormalRadioButton);
            PungNormalRadioButtons.Add(pungFourNormalRadioButton);
            PungLuckyRadioButtons.Add(pungOneLuckyRadioButton);
            PungLuckyRadioButtons.Add(pungTwoLuckyRadioButton);
            PungLuckyRadioButtons.Add(pungThreeLuckyRadioButton);
            PungLuckyRadioButtons.Add(pungFourLuckyRadioButton);
            PungDoubleLuckyRadioButtons.Add(pungOneDoubleLuckyRadioButton);
            PungDoubleLuckyRadioButtons.Add(pungTwoDoubleLuckyRadioButton);
            PungDoubleLuckyRadioButtons.Add(pungThreeDoubleLuckyRadioButton);
            PungDoubleLuckyRadioButtons.Add(pungFourDoubleLuckyRadioButton);

            // add kong radio buttons to Lists
            KongNormalRadioButtons.Add(kongOneNormalRadioButton);
            KongNormalRadioButtons.Add(kongTwoNormalRadioButton);
            KongNormalRadioButtons.Add(kongThreeNormalRadioButton);
            KongNormalRadioButtons.Add(kongFourNormalRadioButton);
            KongLuckyRadioButtons.Add(kongOneLuckyRadioButton);
            KongLuckyRadioButtons.Add(kongTwoLuckyRadioButton);
            KongLuckyRadioButtons.Add(kongThreeLuckyRadioButton);
            KongLuckyRadioButtons.Add(kongFourLuckyRadioButton);
            KongDoubleLuckyRadioButtons.Add(kongOneDoubleLuckyRadioButton);
            KongDoubleLuckyRadioButtons.Add(kongTwoDoubleLuckyRadioButton);
            KongDoubleLuckyRadioButtons.Add(kongThreeDoubleLuckyRadioButton);
            KongDoubleLuckyRadioButtons.Add(kongFourDoubleLuckyRadioButton);
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
            PossibleKongs = new List<Set>();
            PossibleKongs.Add(noSets = new Set());

            if (pungCountComboBox.SelectedItem != null)
            {
                for (var i = 0; i < (4 - pungComboBox.SelectedIndex); i++)
                {
                    PossibleKongs.Add(PossiblePungs[i + 1]);
                }
            }
            kongCountComboBox.ItemsSource = PossibleKongs;
        }

        /// <summary>
        /// manually reset check boxes and radio buttons, so we don't have hidden "checked" options
        /// The exception here are "Normal" radio buttons. these should always reset to "checked"
        /// this helps us avoid having no radio button selected, and normal radio buttons have no additional score value anyway
        /// </summary>
        /// <param name="setType"></param>
        /// <param name="upToSetNum"></param>
        private void ResetSetCheckBoxes(string setType, int upToSetNum = 4)
        {
            // first time fired, just check all normal radio butons 
            if (!setRadioButtonsShown)
            {
                pungOneNormalRadioButton.IsChecked = true;
                pungTwoNormalRadioButton.IsChecked = true;
                pungThreeNormalRadioButton.IsChecked = true;
                pungFourNormalRadioButton.IsChecked = true;
                kongOneNormalRadioButton.IsChecked = true;
                kongTwoNormalRadioButton.IsChecked = true;
                kongThreeNormalRadioButton.IsChecked = true;
                kongFourNormalRadioButton.IsChecked = true;

                setRadioButtonsShown = true;
            }

            if (setType == "pung")
            {
                pungFourConcealedCheckBox.IsChecked = false;
                pungFourTerminalsHonorsCheckBox.IsChecked = false;
                pungFourNormalRadioButton.IsChecked = true;
                pungFourLuckyRadioButton.IsChecked = false;
                pungFourDoubleLuckyRadioButton.IsChecked = false;              

                if (upToSetNum < 4)
                {
                    pungThreeConcealedCheckBox.IsChecked = false;
                    pungThreeTerminalsHonorsCheckBox.IsChecked = false;
                    pungThreeNormalRadioButton.IsChecked = true;
                }

                if (upToSetNum < 3)
                {
                    pungTwoConcealedCheckBox.IsChecked = false;
                    pungTwoTerminalsHonorsCheckBox.IsChecked = false;
                    pungTwoNormalRadioButton.IsChecked = true;
                }

                if (upToSetNum < 2)
                {
                    pungOneConcealedCheckBox.IsChecked = false;
                    pungOneTerminalsHonorsCheckBox.IsChecked = false;
                    pungOneNormalRadioButton.IsChecked = true;
                }
            } else if (setType == "kong")
            {
                kongFourConcealedCheckBox.IsChecked = false;
                kongFourTerminalsHonorsCheckBox.IsChecked = false;
                kongFourNormalRadioButton.IsChecked = true;

                if (upToSetNum < 4)
                {
                    kongThreeConcealedCheckBox.IsChecked = false;
                    kongThreeTerminalsHonorsCheckBox.IsChecked = false;
                    kongThreeNormalRadioButton.IsChecked = true;
                }

                if (upToSetNum < 3)
                {
                    kongTwoConcealedCheckBox.IsChecked = false;
                    kongTwoTerminalsHonorsCheckBox.IsChecked = false;
                    kongTwoNormalRadioButton.IsChecked = true;
                }

                if (upToSetNum < 2)
                {
                    kongOneConcealedCheckBox.IsChecked = false;
                    kongOneTerminalsHonorsCheckBox.IsChecked = false;
                    kongOneNormalRadioButton.IsChecked = true;
                }
            }        
        }

        /// <summary>
        /// Loop through the double lucky radio buttons 3 times
        /// 1st time: enable all disabled buttons
        /// 2nd time: if one is checked, store it in checkedName
        /// 3rd time: disable all other double luckyu radio buttons
        /// </summary>
        private void EnableDisableDoubleLuckyRadioButtons()
        {
            // create a list of all double lucky radio buttons
            List<RadioButton> AllDoubleLuckyRadioButtons = new List<RadioButton>();
            AllDoubleLuckyRadioButtons.AddRange(PungDoubleLuckyRadioButtons);
            AllDoubleLuckyRadioButtons.AddRange(KongDoubleLuckyRadioButtons);

            // variables to store the name of any checked item, plus first and second run through the loop
            string checkedName = "";
            bool firstRun = true;
            bool secondRun = false;

            restart:
            foreach (RadioButton radioButton in AllDoubleLuckyRadioButtons)
            {
                // first run, we enable all double lucky radio buttons
                if (firstRun == true && 
                    AllDoubleLuckyRadioButtons.IndexOf(radioButton) == AllDoubleLuckyRadioButtons.Count - 1)
                {
                    radioButton.IsEnabled = true;

                    // on the last time through the loop, restart the loop
                    firstRun = false;
                    secondRun = true;
                    goto restart;
                }
                else
                {
                    radioButton.IsEnabled = true;
                }

                // second run, look for any checked double lucky radio buttons
                if (radioButton.IsChecked == true && secondRun == true)
                {
                    // if we find one, store it's name
                    checkedName = radioButton.Name;

                    // then restart the loop
                    secondRun = false;
                    goto restart;
                }

                // third run, disable all double lucky radio buttons except the checked one
                if (firstRun == false && secondRun == false && radioButton.Name != checkedName)
                {
                    radioButton.IsEnabled = false;
                }                 
            }
        }

        /// <summary>
        /// Sets up the check boxes for pungs and kongs
        /// </summary>
        /// <param name="comboBox"></param>
        private void ShowSetCheckBoxes(ComboBox comboBox)
        {

            if (RoundWinner().CurrentWind == game.PrevailingWind)
                VisualStateManager.GoToState(this, "DoubleLuckyWind", true);
            else
                VisualStateManager.GoToState(this, "RegularLuckyWind", true);

            if (comboBox.Name == "pungCountComboBox")
            {
                ResetSetCheckBoxes("pung", comboBox.SelectedIndex);
                switch (comboBox.SelectedIndex)
                {
                    case -1:
                        VisualStateManager.GoToState(this, "NoPungsSelected", true);
                        break;
                    case 0:
                        VisualStateManager.GoToState(this, "NoPungsSelected", true);
                        break;
                    case 1:
                        VisualStateManager.GoToState(this, "OnePungSelected", true);
                        break;
                    case 2:
                        VisualStateManager.GoToState(this, "TwoPungsSelected", true);
                        break;
                    case 3:
                        VisualStateManager.GoToState(this, "ThreePungsSelected", true);
                        break;
                    case 4:
                        VisualStateManager.GoToState(this, "FourPungsSelected", true);
                        break;
                    default:
                        VisualStateManager.GoToState(this, "NoPungsSelected", true);
                        break;
                }
            } else if (comboBox.Name == "kongCountComboBox")
            {
                ResetSetCheckBoxes("kong", comboBox.SelectedIndex);

                switch (comboBox.SelectedIndex)
                {
                    case -1:
                        VisualStateManager.GoToState(this, "NoKongsSelected", true);
                        break;
                    case 0:
                        VisualStateManager.GoToState(this, "NoKongsSelected", true);
                        break;
                    case 1:
                        VisualStateManager.GoToState(this, "OneKongSelected", true);
                        break;
                    case 2:
                        VisualStateManager.GoToState(this, "TwoKongsSelected", true);
                        break;
                    case 3:
                        VisualStateManager.GoToState(this, "ThreeKongsSelected", true);
                        break;
                    case 4:
                        VisualStateManager.GoToState(this, "FourKongsSelected", true);
                        break;
                    default:
                        VisualStateManager.GoToState(this, "NoKongsSelected", true);
                        break;
                }
            }
        }


        /// <summary>
        /// Called whenever we need to set up (or reset) the Rules ListView
        /// Shows or hides certain items in the ListView, depending on checked boxes or ComboBox selections
        /// </summary>
        private void InitializeRules()
        {
            // set up lists of possible rules
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
                        if ((pungCountComboBox.SelectedIndex + kongCountComboBox.SelectedIndex) < 
                            (ConcealedPungsKongs() + 2) &&
                            drawnFromComboBox.SelectedIndex != drawnFromComboBox.Items.Count - 1)
                            rule.ShowInList = true;
                        else
                            rule.ShowInList = false;

                        break;

                    // "Concealed" (fully) - show if all pungs/kongs are concealed and winning tile IS self drawn
                    case 4:
                        if (pungCountComboBox.SelectedIndex + kongCountComboBox.SelectedIndex == ConcealedPungsKongs() && 
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

        private void ResetRulesListViews()
        {
            if (pointsDoublesStackPanel.Visibility == Visibility.Visible)
                pointsDoublesStackPanel.Visibility = Visibility.Collapsed;

            foreach (ListViewItem lvi in commonRulesListView.SelectedItems)
            {
                lvi.IsSelected = false;
            }

            foreach (ListViewItem lvi in uncommonRulesListView.SelectedItems)
            {
                lvi.IsSelected = false;
            }
        }

        #endregion


        #region "SUMMARY" METHODS


        private void AppendSpecialRulesSummary()
        {
            // Add summary info for selected common rule values and doubles

            foreach (Rule rule in commonRulesListView.SelectedItems)
            {
                // make sure it has a score value, add the score info to the summary
                if (rule.Score != null)
                {
                    // as special case, need to first check one chance and worthless
                    if (Worthless() && rule == game.Rules[1])
                        AddToSpecialRulesSummary("Since this hand is worthless, points for " + 
                            rule.Name + 
                            " are not added to the score.");
                    else
                        AddToSpecialRulesSummary(rule.Score + 
                            " is added to the base score because " + 
                            rule.Description + ".");
                }

                // add to summary based on Double value (if 0, do nothing b/c score was not doubled)
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


            // Add summary info for selected uncommon rule values and doubles

            foreach (Rule rule in uncommonRulesListView.SelectedItems)
            {
                // make sure it has a score value, add the score info to the summary
                if (rule.Score != null)
                    AddToSpecialRulesSummary(rule.Score + 
                        " is added to the base score because " + 
                        rule.Description + ".");


                // add to summary based on Double value (if 0, do nothing b/c score was not doubled)
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

            // Add summary info for automatic rules

            foreach (Rule rule in game.Rules)
            {
                if (!rule.ShowInList)
                {
                    switch (game.Rules.IndexOf(rule))
                    {
                        // Worthless hand - "one chance" and "self drawn" points must be subtracted
                        case 5:
                            if (Worthless())
                                AddToSpecialRulesSummary("Because this is a worthless hand, the score is doubled.");
                            break;
                        // Three concealed pungs - apply summary if there are 3 or more concealed pungs or kongs
                        case 8:
                            if (ConcealedPungsKongs() >= 3)
                                AddToSpecialRulesSummary("Score was doubled twice because " + rule.Description + ".");
                            break;

                        // All pungs - apply summary if there are a total of 4 pungs/kongs
                        case 20:
                            if (pungCountComboBox.SelectedIndex + kongCountComboBox.SelectedIndex == 4)
                                AddToSpecialRulesSummary("Score was doubled twice because " + rule.Description + ".");
                            break;

                        // three kongs - apply summary if there are 3 or more kongs
                        case 22:
                            if (kongCountComboBox.SelectedIndex >= 3)
                                AddToSpecialRulesSummary("Score was doubled twice because " + rule.Description + ".");
                            break;
                    }
                }
            }


            // Add summary info for Selfdrawn

            if (SelfDrawn())
            {
                // only apply self drawn points if hand isn't worthless
                if (!Worthless())
                    AddToSpecialRulesSummary(game.Rules[0].Score + 
                        " is added to the base score because " + 
                        game.Rules[0].Description + ".");
                else
                    AddToSpecialRulesSummary("Even though winning tile was self-drawn, " + 
                        game.Rules[0].Score + 
                        " points are not added because the hand was worthless.");
            }

            // finally, add the special rules summary to the current round summary
            if (_specialRulesSummary != null)
                game.CurrentRoundSummary.Insert(game.CurrentRoundSummary.Length, _specialRulesSummary.ToString());
        }

        /// <summary>
        /// Adds text to the special rules summary, which is later added to the current round summary
        /// </summary>
        /// <param name="summary">takes a string to add to the special rules summary</param>
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
        private void AppendPungKongSummary(Player player)
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
        /// Creates the In Progress Tips summary wholesale each time this method is called
        /// </summary>
        private void GenerateInProgressTips()
        {
            inProgressTipsGrid.Visibility = Visibility.Visible;
            _inProgressTips = new StringBuilder();


            if (DealerWon())
                _inProgressTips.Insert(
                    _inProgressTips.Length, 
                    winnerComboBox.SelectedItem + " wins and is also the dealer, so their score will be multiplied by 6."
                    );
            else
                _inProgressTips.Insert(
                    _inProgressTips.Length, 
                    winnerComboBox.SelectedItem + " wins and is not the dealer, so their score will be multiplied by 4."
                    );


            if (SelfDrawn() && !Worthless())
            {
                InsertLineBreaks(_inProgressTips, 2);

                _inProgressTips.Insert(
                    _inProgressTips.Length, 
                    "The winning tile is self-drawn, which will give " + 
                    winnerComboBox.SelectedItem + 
                    " 2 extra points."
                    );
            } else if (PlayerDrawnFrom() != null)
            {
                InsertLineBreaks(_inProgressTips, 2);

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
                        "The winning tile is drawn from " +
                        PlayerDrawnFrom().Name +
                        ". They will pay the winning score multiplied by 4."
                        );
                }              
            }

            if (OneChance() && !Worthless())
            {
                InsertLineBreaks(_inProgressTips, 2);

                _inProgressTips.Insert(
                    _inProgressTips.Length,
                    "The winning tile is a one chance draw, which will give " +
                    winnerComboBox.SelectedItem +
                    " 2 extra points."
                    );
            }

            if (Worthless())
            {
                InsertLineBreaks(_inProgressTips, 2);

                _inProgressTips.Insert(
                    _inProgressTips.Length,
                    "This hand is currently Worthless, because there are no sets or Lucky Pair. " +
                    winnerComboBox.SelectedItem +
                    " will get a Double for this. "
                    );

                InsertLineBreaks(_inProgressTips, 2);

                // in case of worthless, add tip about self drawn and/or one chance points not counting
                if (SelfDrawn() && OneChance())
                    _inProgressTips.Insert(_inProgressTips.Length, "Self-Drawn and One Chance points will not count, because the hand is Worthless.");
                else if (SelfDrawn())
                    _inProgressTips.Insert(_inProgressTips.Length, "Self-Drawn points will not count because the hand is Worthless.");
                else if (OneChance())
                    _inProgressTips.Insert(_inProgressTips.Length, "One Chance points will not count because the hand is Worthless.");
            }

            if (PartiallyConcealed() && !SelfDrawn())
            {
                InsertLineBreaks(_inProgressTips, 2);

                _inProgressTips.Insert(
                    _inProgressTips.Length,
                    "The winning hand is semi-concealed (not fully concealed, since the winning tile was drawn from someone else). This will give " +
                    winnerComboBox.SelectedItem +
                    " 10 extra points."
                    );
            }

            if (FullyConcealed() && SelfDrawn())
            {
                InsertLineBreaks(_inProgressTips, 2);

                _inProgressTips.Insert(
                    _inProgressTips.Length,
                    "The winning hand is completely concealed, including the winning draw tile. This will double " +
                    winnerComboBox.SelectedItem +
                    "'s score."
                    );
            }

            if (LuckyPair())
            {
                InsertLineBreaks(_inProgressTips, 2);

                _inProgressTips.Insert(
                    _inProgressTips.Length,
                    "The winning hand contains a pair of lucky winds, which will give " +
                    winnerComboBox.SelectedItem +
                    " 2 extra points."
                    );
            }

            if (AllSimples())
            {
                InsertLineBreaks(_inProgressTips, 2);

                _inProgressTips.Insert(
                    _inProgressTips.Length,
                    "The winning tile is comprised of all simple tiles, which will double " +
                    winnerComboBox.SelectedItem +
                    "'s score."
                    );
            }

            if (pungCountComboBox.SelectedItem != null)
            {
                ShowInProgressPlayerScores();
            }
            

            inProgressTipsText.Text = _inProgressTips.ToString();        
        }

        /// <summary>
        /// Show each player's score dynamically in the In Progress Tips summary
        /// </summary>
        private void ShowInProgressPlayerScores()
        {
            int scoreAdjustment;

            // get the rounded current  base score for this round
            int currentBaseScore = CurrentBaseScore();

            InsertLineBreaks(_inProgressTips, 2);

            _inProgressTips.Insert(_inProgressTips.Length, "Score adjustments:");
            InsertLineBreaks(_inProgressTips, 1);
            foreach (Player player in game.Players)
            {
                scoreAdjustment = 0;
                scoreAdjustment = PlayerRoundScore(player, currentBaseScore);

                if (scoreAdjustment > 0)
                    _inProgressTips.Insert(_inProgressTips.Length, player.Name + ": +" + scoreAdjustment);
                else
                    _inProgressTips.Insert(_inProgressTips.Length, player.Name + ": " + scoreAdjustment);

                InsertLineBreaks(_inProgressTips, 1);
            }
        }

        private void GenerateRoundSummary()
        {
            int finalBaseScore = CurrentBaseScore();

            // create a StringBuilder instance to store the summary in
            game.CurrentRoundSummary = new StringBuilder();

            // Mahjong hands have a max or "limit" value. Enforce that here
            if (finalBaseScore >= ScoreValues.MAX_ROUND_SCORE)
            {
                game.CurrentRoundSummary.Insert(game.CurrentRoundSummary.Length,
                    "This hand has reached the max limit of " +
                    ScoreValues.MAX_ROUND_SCORE + ".");

                InsertLineBreaks(game.CurrentRoundSummary, 2);
            }

            // show winner's score and info, based on whether they're dealer or not
            if (DealerWon())
            {
                game.CurrentRoundSummary.Insert(game.CurrentRoundSummary.Length, RoundWinner().Name + 
                    " won this round! Since they were also the dealer, they receive " +
                    (finalBaseScore * ScoreValues.X_ISWINNER_ISDEALER) +
                    ": The base score of " +
                    finalBaseScore +
                    " multiplied by 6.");

                InsertLineBreaks(game.CurrentRoundSummary, 2);

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

                InsertLineBreaks(game.CurrentRoundSummary, 2);

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

            InsertLineBreaks(game.CurrentRoundSummary, 2);

            // add round winner's pung/kong info
            AppendPungKongSummary(RoundWinner());

            InsertLineBreaks(game.CurrentRoundSummary, 2);

            // add compiled special rules summary to current round summary
            AppendSpecialRulesSummary();

            // finalize the current round summary by adding it to the List
            game.RoundSummaries.Add(game.CurrentRoundSummary.ToString());
        }


        /// <summary>
        /// Just add a paragraph break into a String Builder
        /// </summary>
        private void InsertLineBreaks(StringBuilder sb, int numBreaks)
        {
            for (var i = 0; i < numBreaks; i++)
            {
                sb.Append(Environment.NewLine);
            }
        }

        #endregion


        #region "SCORE" METHODS

        /// <summary>
        /// Calculates scores for the different sets. We'll run this method twice: Once for Pungs, once for Kongs
        /// </summary>
        /// <param name="TerminalsHonorsSetCheckBoxes"></param>
        /// <param name="ConcealedSetCheckBoxes"></param>
        /// <param name="baseValue"></param>
        private int SetsScore(ComboBox setsComboBox, List<CheckBox> TerminalsHonorsSetCheckBoxes, List<CheckBox> ConcealedSetCheckBoxes, int baseValue)
        {
            int setsNum = setsComboBox.SelectedIndex;

            int finalSetsValue = 0;
            int checkedTerminalsHonors = 0;
            int checkedConcealed = 0;

            if (TerminalsHonorsSetCheckBoxes != null)
            {
                foreach (CheckBox terminalsHonorsCheckBox in TerminalsHonorsSetCheckBoxes)
                {
                    // if it's terminal or honors
                    if (terminalsHonorsCheckBox.IsChecked == true)
                        checkedTerminalsHonors++;

                    // if it's concealed
                    if (ConcealedSetCheckBoxes[TerminalsHonorsSetCheckBoxes.IndexOf(terminalsHonorsCheckBox)].IsChecked == true)
                        checkedConcealed++;
                }
            }

            // loop once for each set
            for (int i = 0; i < setsNum; i++)
            {
                int thisSetValue = baseValue;

                // apply terminals/honors check boxes if applicable
                if (checkedTerminalsHonors > 0)
                {
                    thisSetValue *= 2;
                    checkedTerminalsHonors--;
                }

                //apply concealed check boxes if applicable
                if (checkedConcealed > 0)
                {
                    thisSetValue *= 2;
                    checkedConcealed--;
                }

                finalSetsValue += thisSetValue;
            }
            return finalSetsValue;
        }

        /// <summary>
        /// Calculate the doubles for a group of sets (pungs or kongs) based on which radio buttons are selected, 
        /// and applies doubles to the base score
        /// </summary>
        /// <param name="LuckyRadioButtons"></param>
        /// <param name="DoubleLuckyRadioButtons"></param>
        /// <param name="baseScore"></param>
        /// <returns>base score doubled the calculated number of times</returns>
        private int ApplySetDoubles(List<RadioButton> LuckyRadioButtons, List<RadioButton> DoubleLuckyRadioButtons, int baseScore)
        {
            int setDoubles = 0;

            foreach (RadioButton luckyRadioButton in LuckyRadioButtons)
            {
                if (luckyRadioButton.IsChecked == true)
                    setDoubles++;
            }

            foreach (RadioButton doubleLuckyRadioButton in DoubleLuckyRadioButtons)
            {
                if (doubleLuckyRadioButton.IsChecked == true)
                {
                    // increment twice for double lucky set
                    setDoubles++;
                    setDoubles++;

                    // there can only be one, so no point continuing the loop
                    break;
                }
            }

            return DoubleScore(baseScore, setDoubles);
        }


        /// <summary>
        /// A method that determines the winner, who dealt the winning tile (if anyone),
        /// and who was the dealer, then uses that info to process scores per player
        /// </summary>
        private void SetFinalScores()
        {
            // get the final base score
            int _finalBaseScore = CurrentBaseScore();

            // process the final score for each player
            foreach (Player player in game.Players)
            {
                // process the score for each player and add it to the player's Round Scores list
                player.RoundScores.Add(PlayerRoundScore(player, _finalBaseScore));

                // set each player's total score, -1 from currentround because the roundscores list indexes at 0
                player.TotalScore += player.RoundScores[game.CurrentRound - 1];
            }

            // create the round summary and add it to the round summaries list
            GenerateRoundSummary();
        }



        private int DoubleScore(int score, int timesDoubled)
        {
            for (int i = 0; i < timesDoubled; i++)
            {
                score *= 2;
            }
            return score;
        }

        #endregion


        #region "STATUS" METHODS


        /// <summary>
        /// Takes the current base score an a Player instance and processes that player's score for the round
        /// </summary>
        /// <param name="player">The player we want the round score for</param>
        /// <param name="baseScore">The current base score</param>
        /// <returns></returns>
        private int PlayerRoundScore(Player player, int baseScore)
        {
            if (DealerWon()) // dealer won so score is multiplied 6x
            {

                if (player == RoundWinner()) // AWARD THE WINNER
                {
                    return (baseScore * ScoreValues.X_ISWINNER_ISDEALER); // x6, positive
                }
                else // TAKE FROM LOSERS
                {
                    // split up payment based on who winning tile was drawn from
                    if (SelfDrawn())
                    {
                        return (baseScore * ScoreValues.X_ISLOSER_SELFDRAWN_DEALERWON); // x2, negative
                    }
                    else
                    {
                        if (player == PlayerDrawnFrom())
                        {
                            return (baseScore * ScoreValues.X_ISLOSER_ISDRAWNFROM_DEALERWON); // x6, negative
                        }
                        else
                        {
                            // even though score change is 0, still need to add an entry in the player's RoundScores list
                            return 0;
                        }
                    }
                }
            }
            else // non-dealer won so score is multiplied 4x
            {
                if (player == RoundWinner()) // AWARD THE WINNER
                {
                    return (baseScore * ScoreValues.X_ISWINNER); // x4, postive
                }
                else // TAKE FROM LOSERS
                {
                    // split up payment based on who winning tile was drawn from
                    if (SelfDrawn())
                    {
                        // if self drawn and a losing player was dealer, they pay double
                        if (player.IsDealer)
                        {
                            return (baseScore * ScoreValues.X_ISLOSER_ISDEALER_SELFDRAWN); // x2, negative
                        }
                        else
                        {
                            return (baseScore * ScoreValues.X_ISLOSER_SELFDRAWN); // x1, negative
                        }
                    }
                    else
                    {
                        // if a player was drawn from, they pay for everyone
                        if (player == PlayerDrawnFrom())
                        {
                            return (baseScore * ScoreValues.X_ISLOSER_ISDRAWNFROM); // x4, negative
                        }
                        else
                        {
                            // even though score change is 0, still need to add an entry in the player's RoundScores list
                            return 0;
                        }
                    }
                }
            }
        }


        /// <summary>
        /// Calculates the current base score, based on user selection at the time
        /// </summary>
        /// <returns>the current base score for the round</returns>
        private int CurrentBaseScore()
        {   
            // set base value
            int baseScore = ScoreValues.BASE_ROUND_SCORE;

            // PUNGS AND KONGS
            // check which pung check boxes are checked, and adjust the score
            baseScore += SetsScore(pungCountComboBox, PungTerminalsHonorsCheckBoxes, PungConcealedCheckBoxes, ScoreValues.BASE_PUNG_VALUE);

            // check which kong check boxes are checked, and adjust the score
            baseScore += SetsScore(kongCountComboBox, KongTerminalsHonorsCheckBoxes, KongConcealedCheckBoxes, ScoreValues.BASE_KONG_VALUE);

            // APPLY SCORE VALUES AND DOUBLES
            baseScore = ApplySetDoubles(PungLuckyRadioButtons, PungDoubleLuckyRadioButtons, baseScore);
            baseScore = ApplySetDoubles(KongLuckyRadioButtons, KongDoubleLuckyRadioButtons, baseScore);
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
                baseScore = ScoreValues.MAX_ROUND_SCORE;

            return baseScore;
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


        /// <summary>
        /// Calculate whether or not winning tile was a One Chance draw
        /// </summary>
        /// <returns>true if One Chance, false if not</returns>
        private bool OneChance()
        {
            if (commonRulesListView.SelectedItems.Contains(game.Rules[1]))
                return true;
            else
                return false;
        }


        /// <summary>
        /// Calculate whether or not Lucky Pair is selected
        /// </summary>
        /// <returns>true if Lucky Pair, false if not</returns>
        private bool LuckyPair()
        {
            if (commonRulesListView.SelectedItems.Contains(game.Rules[2]))
                return true;
            else
                return false;
        }


        /// <summary>
        /// Checks whether semi concealed is selected
        /// </summary>
        /// <returns>true if semi concealed, false if not</returns>
        private bool PartiallyConcealed()
        {
            if (commonRulesListView.SelectedItems.Contains(game.Rules[3]))
                return true;
            else
                return false;
        }

        /// <summary>
        /// Checks whether fully concealed is selected
        /// </summary>
        /// <returns>true if fully concealed, false if not</returns>
        private bool FullyConcealed()
        {
            if (commonRulesListView.SelectedItems.Contains(game.Rules[4]))
                return true;
            else
                return false;
        }


        /// <summary>
        /// Calculate whether or not winning hand is all simples
        /// </summary>
        /// <returns>true if all simples hand, false if not</returns>
        private bool AllSimples()
        {
            if (commonRulesListView.SelectedItems.Contains(game.Rules[6]))
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
        /// Returns the count of concealed pungs and kongs, based on selected check boxes
        /// </summary>
        /// <returns></returns>
        private int ConcealedPungsKongs()
        {
            int concealedPungsKongsCount = 0;

            if (PungConcealedCheckBoxes != null)
            {
                foreach (CheckBox concealedPungs in PungConcealedCheckBoxes)
                {
                    if (concealedPungs.IsChecked == true)
                        concealedPungsKongsCount++;
                }
            }
            
            if (KongConcealedCheckBoxes != null)
            {
                foreach (CheckBox concealedKongs in KongConcealedCheckBoxes)
                {
                    if (concealedKongs.IsChecked == true)
                        concealedPungsKongsCount++;
                }
            }

            return concealedPungsKongsCount;
        }

        /// <summary>
        /// Gets the count of terminals/honors pungs and kongs, based on selected check boxes
        /// </summary>
        /// <returns>count of pungs/kongs that are checked as terminals/honors</returns>
        private int TerminalsHonorsPungsKongs()
        {
            int terminalsHonorsPungsKongsCount = 0;

            if (PungTerminalsHonorsCheckBoxes != null)
            {
                foreach (CheckBox terminalsHonorsPungs in PungTerminalsHonorsCheckBoxes)
                {
                    if (terminalsHonorsPungs.IsChecked == true)
                        terminalsHonorsPungsKongsCount++;
                }
            }
           
            if (KongTerminalsHonorsCheckBoxes != null)
            {
                foreach (CheckBox terminalsHonorsKongs in KongTerminalsHonorsCheckBoxes)
                {
                    if (terminalsHonorsKongs.IsChecked == true)
                        terminalsHonorsPungsKongsCount++;
                }
            }
            
            return terminalsHonorsPungsKongsCount;
        }

        #endregion


        #region "RULES" METHODS

        /// <summary>
        /// Loops through common and uncommon selected rules and applies appropriate points values to the current base score
        /// </summary>
        /// <param name="baseScore">The current base score</param>
        /// <returns>int value for the modified base score</returns>
        private int ApplySelectedRuleValues(int baseScore)
        {
            foreach (Rule rule in commonRulesListView.SelectedItems)
            {
                if (rule.Score != null) // if a rule is selected and has a Score value
                {
                    // if worthless and one chance is selected, skip the loop for that iteration
                    if (Worthless() && rule == game.Rules[1])
                        continue;
                    else
                        baseScore += rule.Score.Value;                  
                }
            }

            foreach (Rule rule in uncommonRulesListView.SelectedItems)
            {
                if (rule.Score != null)
                    baseScore += rule.Score.Value;
            }

            return baseScore;
        }


        /// <summary>
        /// Apply appropriate points to the base score for a self drawn hand
        /// </summary>
        /// <param name="baseScore">the current base score</param>
        /// <returns>int value for the modified base score</returns>
        private int ApplySelfDrawnPoints(int baseScore)
        {
            if (SelfDrawn())
            {
                // only apply self drawn points if hand isn't worthless
                if (!Worthless())
                    baseScore += game.Rules[0].Score.Value;
            }

            return baseScore;
        }


        /// <summary>
        /// Loops through common and uncommon selected rules and applies appropriate doubles to the base score
        /// </summary>
        /// <param name="baseScore">the current base score</param>
        /// <returns>int value for the modified base score</returns>
        private int ApplySelectedRuleDoubles(int baseScore)
        {
            foreach (Rule rule in commonRulesListView.SelectedItems)
            {
                // for all selected rules, current score gets doubled as many times per it's Double value
                baseScore = DoubleScore(baseScore, rule.Double);
            }

            foreach (Rule rule in uncommonRulesListView.SelectedItems)
            {
                // for all selected rules, current score gets doubled as many times per it's Double value
                baseScore = DoubleScore(baseScore, rule.Double);
            }

            return baseScore;
        }


        /// <summary>
        /// Go through all Rules and apply doubles for special cases
        ///  where we can detect automatically, without asking the user
        /// </summary>
        /// <param name="baseScore">the current base score</param>
        /// <returns>int value for the modified base score</returns>
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
                                baseScore = DoubleScore(baseScore, rule.Double);
                            break;
                        // Three concealed pungs - apply double if there are 3 or more concealed pungs or kongs
                        case 8:
                            if (ConcealedPungsKongs() >= 3)
                                baseScore = DoubleScore(baseScore, rule.Double);
                            break;

                        // all pungs - apply double if there are a total of 4 pungs/kongs
                        case 20:
                            if (pungCountComboBox.SelectedIndex + kongCountComboBox.SelectedIndex == 4)
                                baseScore = DoubleScore(baseScore, rule.Double);
                            break;

                        // three kongs - apply double if there are 3 or more kongs
                        case 22:
                            if (kongCountComboBox.SelectedIndex >= 3)
                                baseScore = DoubleScore(baseScore, rule.Double);
                            break;
                    }
                }
            }

            return baseScore;
        }

        #endregion


        #region CONTROL EVENTS

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

            // show drawn from combo box
            if (drawnFromComboBox.Visibility == Visibility.Collapsed)
                drawnFromComboBox.Visibility = Visibility.Visible;

            // hide Sets UI
            if (setsScoringStackPanel.Visibility == Visibility.Visible)
                setsScoringStackPanel.Visibility = Visibility.Collapsed;

            // hide score button
            if (doneScoringButton.Visibility == Visibility.Visible)
                doneScoringButton.Visibility = Visibility.Collapsed;

            ResetRulesListViews();

            // re-render the in progress text
            GenerateInProgressTips();
        }

        private void DrawnFromComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // show Sets UI
            if (setsScoringStackPanel.Visibility == Visibility.Collapsed)
                setsScoringStackPanel.Visibility = Visibility.Visible;

            // hide score button
            if (doneScoringButton.Visibility == Visibility.Visible)
                doneScoringButton.Visibility = Visibility.Collapsed;

            ResetRulesListViews();

            InitializePossiblePungs();

            InitializeRules();

            // re-render the in progress text
            GenerateInProgressTips();
        }


        private void pungCountComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;

            InitializePossibleKongs(cb);

            ShowSetCheckBoxes(cb);

            pungCountGrid.Visibility = Visibility.Visible;

            if (kongCountComboBox.Visibility == Visibility.Collapsed)
                kongCountComboBox.Visibility = Visibility.Visible;

            // hide score button
            if (doneScoringButton.Visibility == Visibility.Visible)
                doneScoringButton.Visibility = Visibility.Collapsed;

            // if there are 4 pungs, preset kongs combobox to 0
            if (cb.SelectedIndex == 4)
                kongCountComboBox.SelectedIndex = 0;

            // re-render the in progress text
            GenerateInProgressTips();
        }


        private void kongCountComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            ComboBox cb = sender as ComboBox;

            ShowSetCheckBoxes(cb);

            kongCountGrid.Visibility = Visibility.Visible;

            InitializeRules();

            // show Rules 
            if (pointsDoublesStackPanel.Visibility == Visibility.Collapsed)
                pointsDoublesStackPanel.Visibility = Visibility.Visible;

            // show score button
            if (doneScoringButton.Visibility == Visibility.Collapsed)
                doneScoringButton.Visibility = Visibility.Visible;


            // re-render the in progress text
            GenerateInProgressTips();
        }


        private void SetCheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            // reset the Rules list views, based on changes to the check boxes
            InitializeRules();

            // re-render the in progress text
            GenerateInProgressTips();

            // enable or disable the double lucky radio buttons
            // (only do this if the winner's lucky wind matches the prevailing wind)
            RadioButton rb = sender as RadioButton;
            if (rb != null && 
                (RoundWinner().CurrentWind == game.PrevailingWind) && 
                rb.Name.Contains("DoubleLucky"))
                EnableDisableDoubleLuckyRadioButtons();
        }

        private void SetCheckBox_Checked(object sender, RoutedEventArgs e)
        {
            // reset the Rules list views, based on changes to the check boxes
            InitializeRules();

            // re-render the in progress text
            GenerateInProgressTips();

            // enable or disable the double lucky radio buttons 
            // (only do this if the winner's lucky wind matches the prevailing wind)
            RadioButton rb = sender as RadioButton;
            if (rb != null &&
                (RoundWinner().CurrentWind == game.PrevailingWind) &&
                rb.Name.Contains("DoubleLucky"))
                EnableDisableDoubleLuckyRadioButtons();
        }


        private async void DoneScoringButton_Click(object sender, RoutedEventArgs e)
        {
            // Show a confirmation dialog, to double check since we're permanently committing round scores
            var confirmDialog = new MessageDialog("End this round with "+ 
                RoundWinner().Name + 
                " as the winner, earning " + 
                PlayerRoundScore(RoundWinner(), CurrentBaseScore()) 
                + " points?");

            // Add command and callback for finalizing the round scores and ending the round
            confirmDialog.Commands.Add(new UICommand("Yes", (command) =>
            {
                SetFinalScores();

                EndRound();

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

        private void ScoreManuallyButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(EditScoresManuallyPage), game, new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
        }

        private void RulesListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            // re-render the in progress text
            GenerateInProgressTips();
        }

        #endregion



        /// <summary>
        /// Change dealer and prevailing wind, other tidying up
        /// </summary>
        private void EndRound()
        {
            if (DealerWon())
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

            InitializeSetElementLists();

            base.OnNavigatedTo(e);

        }

        private void HomeButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StartPage), new DrillOutThemeAnimation());
        }
    }
}
