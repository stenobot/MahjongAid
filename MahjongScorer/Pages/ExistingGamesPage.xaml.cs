using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
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
    public sealed partial class ExistingGamesPage : Page
    {
        Game game;
        List<Game> MasterSavedGamesList;

        // these must be observable collections rather than lists, so we can remove items from the lists and respective listviews
        ObservableCollection<Game> GamesInProgressList = new ObservableCollection<Game>();       
        ObservableCollection<Game> GamesCompletedList = new ObservableCollection<Game>();

        public ExistingGamesPage()
        {
            this.InitializeComponent();
        }

        private void BackButton_Click(object sender, RoutedEventArgs e)
        {
            Frame.Navigate(typeof(StartPage));
        }



        private async Task GetSavedGamesAsync()
        {
            // create new instance of saved games list
            MasterSavedGamesList = new List<Game>();

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Game>));

            Stream stream = await ApplicationData.Current.LocalFolder.OpenStreamForReadAsync("mahjong-data.json");

            // deserialize game data, cast to List of type Game, and assign it to our saved games List
            MasterSavedGamesList = (List<Game>)serializer.ReadObject(stream);

            // split up the saved games by completed or not
            foreach (Game game in MasterSavedGamesList)
            {
                if (game.InProgress)
                    GamesInProgressList.Add(game);
                else
                    GamesCompletedList.Add(game);
            }

            // assign the 2 new lists as itemssource to their respective list views
            gamesInProgressListView.ItemsSource = GamesInProgressList;
            gamesCompletedListView.ItemsSource = GamesCompletedList;
        }

        private async Task WriteSavedGamesAsync()
        {
            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Game>));


            using (Stream stream = await ApplicationData.Current.LocalFolder.OpenStreamForWriteAsync("mahjong-data.json", CreationCollisionOption.ReplaceExisting))
            {
                // pass game list into the serializer
                serializer.WriteObject(stream, MasterSavedGamesList);
            }
        }


        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            await GetSavedGamesAsync();   
        }

        

        private void LoadGameButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button.Parent == gamesInProgressButtons)
                game = GamesInProgressList[gamesInProgressListView.SelectedIndex];
            else
                game = GamesCompletedList[gamesCompletedListView.SelectedIndex];

            Frame.Navigate(typeof(GameResultsPage), game);
        }



        private async void DeleteGameButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            ObservableCollection<Game> currList;
            ListView currListView;
            if (button.Parent == gamesInProgressButtons)
            {
                currList = GamesInProgressList;
                currListView = gamesInProgressListView;
            }
            else
            {
                currList = GamesCompletedList;
                currListView = gamesCompletedListView;
            }

            // first, find the selected game in the master list, then remove it
            foreach (Game game in MasterSavedGamesList)
            {
                if (game == currList[currListView.SelectedIndex])
                {
                    MasterSavedGamesList.Remove(game);
                    break;
                }
            }

            // then rewrite the save data
            await WriteSavedGamesAsync();

            // lastly, remove selected item from the itemssource
            currList.RemoveAt(currListView.SelectedIndex);
        }

        private void GamesInProgressListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (gamesInProgressButtons.Visibility == Visibility.Collapsed)
                gamesInProgressButtons.Visibility = Visibility.Visible;

            if (gamesInProgressListView.SelectedIndex == -1)
                gamesInProgressButtons.Visibility = Visibility.Collapsed;

            gamesCompletedListView.SelectedIndex = -1;
        }

        private void GamesCompletedListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (gamesCompletedButtons.Visibility == Visibility.Collapsed)
                gamesCompletedButtons.Visibility = Visibility.Visible;

            if (gamesCompletedListView.SelectedIndex == -1)
                gamesCompletedButtons.Visibility = Visibility.Collapsed;

            gamesInProgressListView.SelectedIndex = -1;
        }

       
    }
}
