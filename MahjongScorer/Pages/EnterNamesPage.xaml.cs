using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Windows.Storage;
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
    public sealed partial class EnterNamesPage : Page
    {
        // holder for local Game object
        private Game game;

        // instance of each player
        private Player _playerOne;
        private Player _playerTwo;
        private Player _playerThree;
        private Player _playerFour;

        // new list to check Saved Games
        private List<Game> SavedGamesList;

        // public player list and public player names list
        //public List<Player> Players;
        //public List<string> PlayerNames;


        public EnterNamesPage()
        {
            this.InitializeComponent();
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;

            //// show system back button, handle back
            //SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
            //SystemNavigationManager.GetForCurrentView().BackRequested += (s, a) =>
            //{
            //    if (Frame.CanGoBack)
            //    {
            //        Frame.GoBack();
            //        a.Handled = true;
            //    }
            //};

            startingScoreComboBox.SelectedIndex = 0;
            limitHandComboBox.SelectedIndex = 0;
            baseScoreComboBox.SelectedIndex = 1;
        }


        private async Task SaveNewGameAsync()
        {
            // create new instance of saved games list
            SavedGamesList = new List<Game>();

            // read saved games list from Json data
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Game>));

            var stream = await ApplicationData.Current.RoamingFolder.OpenStreamForReadAsync("mahjong-data.json");

            // retrieve the list of game objects add assign to our new List
            SavedGamesList = (List<Game>)serializer.ReadObject(stream);

            // add new game to the saved games list
            SavedGamesList.Add(game);

            // write saved games list back to storage
            using (stream = await ApplicationData.Current.RoamingFolder.OpenStreamForWriteAsync("mahjong-data.json", CreationCollisionOption.ReplaceExisting))
            {
                // pass game list into the serializer
                serializer.WriteObject(stream, SavedGamesList);
            }
        }

        private async Task InitializeSaveData()
        {
            SavedGamesList = new List<Game>();
            SavedGamesList.Add(game);
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Game>));


            using (Stream stream = await ApplicationData.Current.RoamingFolder.OpenStreamForWriteAsync("mahjong-data.json", CreationCollisionOption.ReplaceExisting))
            {
                // pass game list into the serializer
                serializer.WriteObject(stream, SavedGamesList);
            }
        }


        /// <summary>
        /// Add Player objects for the four players to the game.Players list
        /// </summary>
        private void InitializePlayers()
        {
            // add four players
            game.Players = new List<Player>();
            game.Players.Add(_playerOne = new Player());
            game.Players.Add(_playerTwo = new Player());
            game.Players.Add(_playerThree = new Player());
            game.Players.Add(_playerFour = new Player());

            // initialize dealer status for each player
            foreach (Player player in game.Players)
            {
                player.IsDealer = false;
                player.ConsecutiveWinsAsDealer = 0;
            }

            // player one always starts as dealer
            _playerOne.IsDealer = true;

            // set starting winds
            _playerOne.CurrentWind = Wind.East;
            _playerTwo.CurrentWind = Wind.South;
            _playerThree.CurrentWind = Wind.West;
            _playerFour.CurrentWind = Wind.North;
        }

        private void SetStartingValues()
        {
            switch (startingScoreComboBox.SelectedIndex)
            {
                case 1:
                    game.StartingScore = 3000;
                    break;
                case 0:
                default:
                    game.StartingScore = 2000;
                    break;
            }

            switch (limitHandComboBox.SelectedIndex)
            {
                case 1:
                    game.LimitValue = 1000;
                    break;
                case 2: game.LimitValue = 2000;
                    break;
                case 3: game.LimitValue = int.MaxValue;
                    break;
                case 0:
                default:
                    game.LimitValue = 500;
                    break;
            }

            switch (baseScoreComboBox.SelectedIndex)
            {
                case 0:
                    game.BaseValue = 10;
                    break;
                case 1:
                default:
                    game.BaseValue = 20;
                    break;
            }

            switch (reignOfTerrorComboBox.SelectedIndex)
            {
                case 0:
                    game.ReignOfTerrorLimit = 0;
                    break;
                case 1:
                    game.ReignOfTerrorLimit = 6;
                    break;
                case 2:
                    game.ReignOfTerrorLimit = 8;
                    break;
                default:
                    game.ReignOfTerrorLimit = 0;
                    break;
            }


            // need to set the "Limit Hand" rule values here
            if (game.LimitValue != int.MaxValue)
            {
                game.Rules[game.Rules.Count - 1].Score = game.LimitValue;
                game.Rules[game.Rules.Count - 1].Name = "Limit hand (" + game.LimitValue + " points)";
            }
        }


        private void PlayerNameTextBox_TextChanging(TextBox sender, TextBoxTextChangingEventArgs args)
            {
            // set each player's name based on what was typed in the text box
            if (sender == PlayerOneNameTextBox)
                _playerOne.Name = sender.Text.ToUpper();
            else if (sender == PlayerTwoNameTextBox)
                _playerTwo.Name = sender.Text.ToUpper();
            else if (sender == PlayerThreeNameTextBox)
                _playerThree.Name = sender.Text.ToUpper();
            else _playerFour.Name = sender.Text.ToUpper();

            // don't show the start game button unless each textbox has at least one character
            if ((PlayerOneNameTextBox.Text.Count() > 0) &&
                (PlayerTwoNameTextBox.Text.Count() > 0) &&
                (PlayerThreeNameTextBox.Text.Count() > 0) &&
                (PlayerFourNameTextBox.Text.Count() > 0))
                startGameStackPanel.Visibility = Visibility.Visible;
            else
                startGameStackPanel.Visibility = Visibility.Collapsed;
        }


        private void HelpButton_Click(object sender, RoutedEventArgs e)
        {
            Button btn = sender as Button;
            switch (btn.Name)
            {
                case "startingScoreHelpButton":
                    helpOverlayControl.Title = "Starting Score";
                    helpOverlayControl.Body = "Each player starts the game with a certain amount of points. As the game progresses, they'll earn points from other players when they win a round. When they lose a round, they'll have to pay the winner in some circumstances (such as if they discarded the winning tile).";
                    break;
                case "limitValueHelpButton":
                    helpOverlayControl.Title = "Limit Value";
                    helpOverlayControl.Body = "If a player wins a round with a normal hand that has a huge amount of points, it will max out at the limit value chosen here. Or, if a player goes out with a special Limit Hand (such as Thirteen Orphans), they'll earn the limit value. If No Limit is selected here, a normal hand will have no limit and a special Limit Hand will be worth 500.";
                    break;
                case "baseScoreHelpButton":
                    helpOverlayControl.Title = "Base Score";
                    helpOverlayControl.Body = "The base score is the minimum amount of points a player earns when they win a round. for example: If the base score is 20, the winner's base score will be 20 points plus any additional points or doubles for sets and special conditions. The final score gets rounded to the nearest 10, then other players must pay that score depending on who dealt and who discarded the winning tile.";
                    break;
                case "reignOfTerrorHelpButton":
                    helpOverlayControl.Title = "Reign of Terror";
                    helpOverlayControl.Body = "Reign of terror is a special rule that limits the number of rounds a dealer can win in a row. If a player reaches this limit, they'll earn a Limit Hand, and dealer duty will automatically advance to the next player in line.";
                    break;
                default:
                    break;
            }

            if (helpOverlayControl.Visibility != Visibility.Visible)
                FadeInHelpOverlay.Begin();
        }


        private async void StartGameButton_Click(object sender, RoutedEventArgs e)
        {
            startGameButton.Visibility = Visibility.Collapsed;
            startGameProgress.Visibility = Visibility.Visible;

            // set game property for save data display
            game.CurrentDealerName = _playerOne.Name;

            SetStartingValues();

            // set starting values for some Player properties
            foreach (Player player in game.Players)
            {
                player.RoundScores = new List<int>();
                player.TotalScore = game.StartingScore;
                player.IsGameWinner = false;
            }

            // set starting values for some game properties
            game.InProgress = true;
            game.TimesDealerWon = 0;
            game.PrevailingWind = Wind.East;
            game.CurrentRound = 0;

            // create List of prevailing winds to store in game instance
            game.PrevailingWinds = new List<Wind>();
            game.PrevailingWinds.Add(game.PrevailingWind);

            try
            {
                // save new game data
                await SaveNewGameAsync();
            }
            catch
            {
                // initialize a new instance of save data (first run)
                await InitializeSaveData();
            }



            // go to Game Results page and pass it the game object
            Frame.Navigate(typeof(GameResultsPage), game);
        }



        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Game)
            {
                game = e.Parameter as Game;
            }            

            InitializePlayers();

            base.OnNavigatedTo(e);
        }
    }
}
