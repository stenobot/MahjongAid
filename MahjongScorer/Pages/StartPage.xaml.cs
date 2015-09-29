using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.Storage;
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
    public sealed partial class StartPage : Page
    {
        // holder for local Game object
        private Game game;

        // win conditions
        private WinCondition oneChance;
        private WinCondition luckyPair;
        private WinCondition concealed;
        private WinCondition worthless;
        private WinCondition allSimples;
        private WinCondition oneToNineRun;
        //private WinCondition threeConcealedPungs;
        private WinCondition allOneSuit;
        //private WinCondition allHonors;
        private WinCondition oneSuitNoHonors;

        public StartPage()
        {
            this.InitializeComponent();
        }


        /// <summary>
        /// A method that creates a list of the Win Conditions
        /// and sets appr. values for each Win Condition
        /// </summary>
        private void InitializeWinConditions()
        {
            // initialize the list of Win Conditions
            game.WinConditions = new List<WinCondition>();

            // add each Win Condition to the list
            game.WinConditions.Add(oneChance = new WinCondition());
            game.WinConditions.Add(luckyPair = new WinCondition());
            game.WinConditions.Add(concealed = new WinCondition());
            game.WinConditions.Add(worthless = new WinCondition());
            game.WinConditions.Add(allSimples = new WinCondition());
            game.WinConditions.Add(oneToNineRun = new WinCondition());
            //game.WinConditions.Add(threeConcealedPungs = new WinCondition());
            game.WinConditions.Add(allOneSuit = new WinCondition());
            //game.WinConditions.Add(allHonors = new WinCondition());
            game.WinConditions.Add(oneSuitNoHonors = new WinCondition());

            // define applicable properties for each Win Condition
            oneChance.Score = ScoreValues.ONE_CHANCE_SCORE;
            oneChance.Name = "One Chance (" + oneChance.Score.ToString() + " points)";
            oneChance.Description = "Only possible to win with one tile";

            luckyPair.Score = ScoreValues.LUCKY_PAIR_SCORE;
            luckyPair.Name = "Lucky pair (" + luckyPair.Score.ToString() + " points)";
            luckyPair.Description = "Winning pair is the prevailing wind or player's lucky wind";

            // this one can be points or a double, but it must be set to double or score points later, before any of the other doubles
            concealed.Name = "Concealed hand (points varies)";
            concealed.Description = "If final tile is self-drawn, it's a double. Otherwise it's 10 points.";

            // no score set on these b/c they're doubles
            worthless.Name = "Worthless (double)";
            worthless.IsDouble = true;
            worthless.Description = "No pungs, no lucky pair";

            allSimples.Name = "All simples (double)";
            allSimples.IsDouble = true;
            allSimples.Description = "No winds, terminals, or dragons";

            oneToNineRun.Name = "1-9 run (double)";
            oneToNineRun.IsDouble = true;
            oneToNineRun.Description = "3 chows, 1-9 of same suit (circles or bamboo)";

            //threeConcealedPungs.Name = "3 concealed pungs (double)";
            //threeConcealedPungs.IsDouble = true;
            //threeConcealedPungs.Description = "3 pungs, all fully concealed";

            allOneSuit.Name = "All one suit (2 doubles)";
            allOneSuit.IsDouble = true;
            allOneSuit.Description = "All sets are the same suit.";

            //allHonors.Name = "All terminals or honors (2 doubles)";
            //allHonors.IsDouble = true;
            //allHonors.Description = "A terminal or honor in every set";

            oneSuitNoHonors.Name = "One suit, no honors (4 doubles)";
            oneSuitNoHonors.IsDouble = true;
            oneSuitNoHonors.Description = "All sets and the pair are in the same suit";
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
            Frame.Navigate(typeof(EnterNamesPage), game);
        }


        private void PreviousGamesButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(ExistingGamesPage), game);
        }

        private void learnToPlayButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(LearnToPlayPage));
        }
    }
}
