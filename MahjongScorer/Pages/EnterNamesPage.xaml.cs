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
        /// Add Player objects for teh four players to the game.Players list
        /// </summary>
        private void InitializePlayers()
        {
            // add four players
            game.Players = new List<Player>();
            game.Players.Add(_playerOne = new Player());
            game.Players.Add(_playerTwo = new Player());
            game.Players.Add(_playerThree = new Player());
            game.Players.Add(_playerFour = new Player());

            // set starting winds and dealer
            _playerOne.CurrentWind = Wind.East;
            _playerOne.IsDealer = true;
            _playerTwo.CurrentWind = Wind.South;
            _playerTwo.IsDealer = false;
            _playerThree.CurrentWind = Wind.West;
            _playerThree.IsDealer = false;
            _playerFour.CurrentWind = Wind.North;
            _playerFour.IsDealer = false;

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
                case 1: game.LimitValue = 1000;
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
