﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Runtime.Serialization.Json;
using System.Threading.Tasks;
using Windows.Storage;
using Windows.UI.Core;
using Windows.UI.Popups;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
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

            // show the system back button
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
        }


        private async Task GetSavedGamesAsync()
        {
            // create new instance of saved games list
            MasterSavedGamesList = new List<Game>();

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Game>));

            Stream stream = await ApplicationData.Current.RoamingFolder.OpenStreamForReadAsync("mahjong-data.json");

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

            using (Stream stream = await ApplicationData.Current.RoamingFolder.OpenStreamForWriteAsync("mahjong-data.json", CreationCollisionOption.ReplaceExisting))
            {
                // pass game list into the serializer
                serializer.WriteObject(stream, MasterSavedGamesList);
            }
        }


        protected async override void OnNavigatedTo(NavigationEventArgs e)
        {
            try
            {
                await GetSavedGamesAsync();
            }
            catch
            {
                VisualStateManager.GoToState(this, "NoPreviousGames", true);
            }
        }

        

        private void LoadGameButton_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;

            if (button.Parent == gamesInProgressButtons)
                game = GamesInProgressList[gamesInProgressListView.SelectedIndex];
            else
                game = GamesCompletedList[gamesCompletedListView.SelectedIndex];

            game.LoadedFromSave = true;

            Frame.Navigate(typeof(GameResultsPage), game, new Windows.UI.Xaml.Media.Animation.DrillInNavigationTransitionInfo());
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


            // Show a confirmation dialog, to double check since we're permanently deleting save data
            var confirmDialog = new MessageDialog("The save data for the game created on " + 
                currList[currListView.SelectedIndex].DateCreated + " (currently on round " + 
                currList[currListView.SelectedIndex].CurrentRound + 
                ") will be permanently deleted.", "Are you sure?");

            // Add command and callback for moving forward with the deletion
            confirmDialog.Commands.Add(new UICommand("Delete it", async (command) =>
            {
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

                // Show then fade out message
                deletedMessageTextBlock.Opacity = 1;
                deletedMessageTextBlock.Text = "Save data was deleted.";
                FadeOutMessage.Begin();
            }));

            // Add command and callback for canceling the deletion
            confirmDialog.Commands.Add(new UICommand("Nevermind", (command) =>
            {
                // Show then fade out message
                deletedMessageTextBlock.Opacity = 1;
                deletedMessageTextBlock.Text = "Canceled. Nothing was deleted.";
                FadeOutMessage.Begin();
            }));

            // Set the default
            confirmDialog.DefaultCommandIndex = 1;

            // Set the command to be invoked when escape is pressed
            confirmDialog.CancelCommandIndex = 1;

            // Show the message dialog
            await confirmDialog.ShowAsync();      
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
