using System;
using System.Collections.Generic;
using Windows.UI.Core;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

// The Blank Page item template is documented at http://go.microsoft.com/fwlink/?LinkId=234238

namespace MahjongScorer.Pages
{
    /// <summary>
    /// An empty page that can be used on its own or navigated to within a Frame.
    /// </summary>
    public sealed partial class StartPage : Page
    {
        // holder for local Game object
        private Game game;

        // rules, with their index, whether they're shown in the UI list, and point value for reference

        private Rule selfDrawn; //0, never show, 2pts
        private Rule oneChance; //1, always show, 2pts
        private Rule luckyPair; //2, always show, 2pts
        private Rule partConcealed; //3, sometimes show, 10pts
        private Rule fullConcealed; //4, sometimes show, double
        private Rule worthless; //5, sometimes show, double
        private Rule allSimples; //6, sometimes show, double
        private Rule oneToNineRun; //7, sometimes show, double
        private Rule threeConcealedPungs; //8, never show, 2 doubles
        private Rule oneSuitWithHonors; //9, always show, double
        private Rule oneSuitNoHonors; //10, sometimes show, 4 doubles
        private Rule allHonors; //11, sometimes show, double
        private Rule luckySet; //12, sometimes show, double
        private Rule doubleLuckySet; //13, sometimes show, 2 doubles
        private Rule offTheDeadWall; //14, sometimes show, double
        private Rule robbingAKong; //15, always show, double
        private Rule bottomOfTheSea; //16, always show, double
        private Rule riichi; //17, always show, double
        private Rule allPairs; //18, sometimes show, 2 doubles
        private Rule triplePung; //19, sometimes show, 2 doubles
        private Rule allPungs; //20, never show, 2 doubles
        private Rule littleThreeDragons; //21, sometimes show, 2 doubles
        private Rule threeKongs; //22, never show, 2 doubles
        private Rule limitHand; //23, always show, 500pts
        

        public StartPage()
        {
            this.InitializeComponent();
        }


        /// <summary>
        /// A method that creates a list of the Rules
        /// and sets the properties for each Rule
        /// </summary>
        private void InitializeWinConditions()
        {
            // initialize the list of Rules
            game.Rules = new List<Rule>();

            //add rules to the list, and set up their properties

            // SELF_DRAWN - 2 POINTS
            game.Rules.Add(selfDrawn = new Rule());
            // never show in list, because this gets explicitly set earlier by user
            selfDrawn.ShowInList = false;
            selfDrawn.Double = 0;
            selfDrawn.Score = ScoreValues.SELF_DRAWN_SCORE;
            selfDrawn.Name = "Self-Drawn (" + selfDrawn.Score.ToString() + " points)"; 
            selfDrawn.Description = "the winning tile was self-drawn";

            // ONE CHANCE - 2 POINTS
            game.Rules.Add(oneChance = new Rule());
            // always show in list
            oneChance.ShowInList = true;
            oneChance.Double = 0;
            oneChance.Score = ScoreValues.ONE_CHANCE_SCORE;
            oneChance.Name = "One chance (" + oneChance.Score.ToString() + " points)";
            oneChance.Description = "it was only possible to win with one tile";

            // LUCKY PAIR - 2 POINTS
            game.Rules.Add(luckyPair = new Rule());
            // always show in list
            luckyPair.ShowInList = true;
            luckyPair.Double = 0;
            luckyPair.Score = ScoreValues.LUCKY_PAIR_SCORE;
            luckyPair.Name = "Lucky pair (" + luckyPair.Score.ToString() + " points)";
            luckyPair.Description = "the winning pair is prevailing wind, lucky wind, or dragons";

            // PARTIALLY CONCEALED - 10 POINTS
            game.Rules.Add(partConcealed = new Rule());
            // only show in list if all pungs/kongs are concealed and winning tile was NOT self drawn
            partConcealed.Double = 0;

            partConcealed.ShowInList = true;

            partConcealed.Score = ScoreValues.CONCEALED_SCORE;
            partConcealed.Name = "Concealed hand (" + partConcealed.Score.ToString() + " points)";
            partConcealed.Description = "the entire hand is concealed except for the winning tile";

            // FULLY CONCEALED - DOUBLE
            game.Rules.Add(fullConcealed = new Rule());
            // only show in list if all pungs/kongs are concealed and winning tile WAS self drawn
            fullConcealed.Double = 1;
            fullConcealed.Name = "Concealed hand (double)";
            fullConcealed.Description = "the entire hand is concealed";

            // WORTHLESS - DOUBLE
            game.Rules.Add(worthless = new Rule());
            // only show in list if no pungs/kongs
            worthless.Double = 1;
            worthless.Name = "Worthless (double)";           
            worthless.Description = "there are no pungs or kongs, and no lucky pair";

            // ALL SIMPLES - DOUBLE
            game.Rules.Add(allSimples = new Rule());
            // only show in list if pungs/kongs have no terminals/honors
            allSimples.Double = 1;
            allSimples.Name = "All simples (double)";
            allSimples.Description = "there are no winds, terminals, or dragons";

            // ONE TO NINE RUN - DOUBLE
            game.Rules.Add(oneToNineRun = new Rule());
            // only show in list if there are 1 or less pungs/kongs
            oneToNineRun.Double = 1;
            oneToNineRun.Name = "1-9 run (double)";
            oneToNineRun.Description = "there are 3 chows with 1-9 in same suit";

            // THREE CONCEALED PUNGS - 2 DOUBLES
            game.Rules.Add(threeConcealedPungs = new Rule());
            // never show in list, because we can always calculate this
            threeConcealedPungs.ShowInList = false;
            threeConcealedPungs.Double = 2;
            threeConcealedPungs.Name = "3 concealed pungs (double)";
            threeConcealedPungs.Description = "there are three concealed pungs";

            // ONE SUIT WITH HONORS - DOUBLE
            game.Rules.Add(oneSuitWithHonors = new Rule());
            // always show in list
            oneSuitWithHonors.ShowInList = true;
            oneSuitWithHonors.Double = 1;
            oneSuitWithHonors.Name = "One suit with honors (double)";           
            oneSuitWithHonors.Description = "all tiles are winds, dragons, and just one suit";

            // ONE SUIT NO HONORS - 4 DOUBLES
            game.Rules.Add(oneSuitNoHonors = new Rule());
            // only show in list if there are no honors pungs/kongs
            oneSuitNoHonors.Double = 4;
            oneSuitNoHonors.Name = "One suit, no honors (4 doubles)";
            oneSuitNoHonors.Description = "all sets and the pair are in the same suit";

            // ALL HONORS - DOUBLE (opposite of all simples)
            game.Rules.Add(allHonors = new Rule());
            // only show in list if every set has a terminal or honor
            allHonors.Double = 1;
            allHonors.Name = "All terminals or honors (double)";
            allHonors.Description = "there's a terminal or honor in every set or chow";

            // LUCKY SET - DOUBLE
            game.Rules.Add(luckySet = new Rule());
            // only show in list if at least one pung or kong has terminals/honors checked
            luckySet.Double = 1;
            luckySet.Name = "Lucky set (double)";
            luckySet.Description = "a pung or kong has the prevailing or lucky wind, or dragons";

            // DOUBLE LUCKY SET - 2 DOUBLES
            game.Rules.Add(doubleLuckySet = new Rule());
            // only show in list if at least one pung or kong has terminals/honors checked
            doubleLuckySet.Double = 2;
            doubleLuckySet.Name = "Double lucky set (2 doubles)";
            doubleLuckySet.Description = "a pung or kong is lucky wind, and the winner's lucky wind is also prevailing";

            // OFF THE DEAD WALL - DOUBLE
            game.Rules.Add(offTheDeadWall = new Rule());
            // only show in list if there's at least one kong
            offTheDeadWall.Double = 1;
            offTheDeadWall.Name = "Off the dead wall (double)";
            offTheDeadWall.Description = "the winning tile came off the dead wall";

            // ROBBING A KONG - DOUBLE
            game.Rules.Add(robbingAKong = new Rule());
            // always show in list
            robbingAKong.ShowInList = true;
            robbingAKong.Double = 1;
            robbingAKong.Name = "Robbing a kong (double)";
            robbingAKong.Description = "the winning tile would have completed someone else's kong";

            // BOTTOM OF THE SEA - DOUBLE
            game.Rules.Add(bottomOfTheSea = new Rule());
            // always show in list
            bottomOfTheSea.ShowInList = true;
            bottomOfTheSea.Double = 1;
            bottomOfTheSea.Name = "Bottom of the sea (double)";
            bottomOfTheSea.Description = "the winning tile was the very last tile before the dead wall";

            // RIICHI - DOUBLE
            game.Rules.Add(riichi = new Rule());
            // always show in list
            riichi.ShowInList = true;
            riichi.Double = 1;
            riichi.Name = "Riichi (double)";
            riichi.Description = "the winner laid their hand ";

            // ALL PAIRS - 2 DOUBLES
            game.Rules.Add(allPairs = new Rule());
            // only show in the list if there are no pungs or kongs
            allPairs.Double = 2;
            allPairs.Name = "All pairs (2 doubles)";
            allPairs.Description = "the winning hand consists of 7 different pairs";

            // TRIPLE PUNG - 2 DOUBLES
            game.Rules.Add(triplePung = new Rule());
            // only show in the list if there are at least 3 pungs
            triplePung.Double = 2;
            triplePung.Name = "Triple pung (2 doubles)";
            triplePung.Description = "there are three pungs or kongs of the same number in different suits";

            // ALL PUNGS - 2 DOUBLES
            game.Rules.Add(allPungs = new Rule());
            // never show in the list
            allPungs.ShowInList = false;
            allPungs.Double = 2;
            allPungs.Name = "All pungs (2 doubles)";
            allPungs.Description = "there are four pungs or kongs";

            // LITTLE THREE DRAGONS - 2 DOUBLES
            game.Rules.Add(littleThreeDragons = new Rule());
            // only show in list if there are at least 2 pungs or kongs that are terminals/honors
            littleThreeDragons.Double = 2;
            littleThreeDragons.Name = "Little three dragons (2 doubles)";
            littleThreeDragons.Description = "there are 2 sets and one pair of dragons";

            // THREE KONGS - 2 DOUBLES
            game.Rules.Add(threeKongs = new Rule());
            // never show in the list
            threeKongs.ShowInList = false;
            threeKongs.Double = 2;
            threeKongs.Name = "Three kongs (2 doubles)";
            threeKongs.Description = "there are three kongs";

            // LIMIT HAND
            game.Rules.Add(limitHand = new Rule());
            // always show in the list
            limitHand.ShowInList = true;
            limitHand.Double = 0;
            limitHand.Score = 500;
            limitHand.Name = "Limit hand (500 points)";
            limitHand.Description = "the winner had a true limit hand (Thirteen Orphans, Nine Gates, etc.)";
        }


        private Game CreateNewGame()
        {
            // create a new instance of a Game object
            game = new Game();

            // set the date created property
            game.DateCreated = DateTime.Now;
            
            // generate a unique identifier (for save data purposes)
            game.Guid = Guid.NewGuid();

            // initialize all the possible win conditions and add them to the game object
            InitializeWinConditions();

            // initialize the list of round summaries strings
            game.RoundSummaries = new List<string>();

            return game;
        }

        private void NewGameButton_Click(object sender, RoutedEventArgs e)
        {
            // create a new game object and save it 
            CreateNewGame();

            // right now doing this just in case, so there's always save data stored (first run)
           // await InitializeSaveDataAsync();

            

            // pass game object to the next page
            Frame.Navigate(typeof(EnterNamesPage), game, new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
        }


        private void PreviousGamesButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ExistingGamesPage), game, new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
        }

        private void learnToPlayButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LearnToPlayPage), new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
        }

        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            // This page is always at the top of our in-app back stack.
            // Once it is reached there is no further back so we can always disable the title bar back UI when navigated here.
            // If you want to you can always to the Frame.CanGoBack check for all your pages and act accordingly.
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Collapsed;
        }
    }
}
