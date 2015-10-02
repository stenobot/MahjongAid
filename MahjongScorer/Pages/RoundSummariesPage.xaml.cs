﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
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
    public sealed partial class RoundSummariesPage : Page
    {
        // holder for local Game object
        private Game game;

        SolidColorBrush titleTextBrush = Application.Current.Resources["MahjongHeaderBrush"] as SolidColorBrush;
        FontFamily shuiFont = Application.Current.Resources["ShuiFont"] as FontFamily;
        GridLength titleRowHeight = (GridLength)Application.Current.Resources["RoundSummariesTitleHeight"];


        public RoundSummariesPage()
        {
            this.InitializeComponent();

            // show the system back button
            SystemNavigationManager.GetForCurrentView().AppViewBackButtonVisibility = AppViewBackButtonVisibility.Visible;
        }




        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            if (e.Parameter is Game)
            {
                game = e.Parameter as Game;
                RenderRoundSummaries();
            }

            
            base.OnNavigatedTo(e);
        }

        private void RenderRoundSummaries()
        {
            int roundCount = 1;
            int rowIndex = 0;



            foreach (string summary in game.RoundSummaries)
            {
                // create two grid rows for each summary, for title and body
                RowDefinition rdTitle = new RowDefinition();
                rdTitle.Height = titleRowHeight;
                RowDefinition rdBody = new RowDefinition();
                roundSummariesGrid.RowDefinitions.Insert(rowIndex, rdTitle);
                roundSummariesGrid.RowDefinitions.Insert(rowIndex + 1, rdBody);

                // create new textblocks for title and body
                TextBlock tbTitle = new TextBlock();
                TextBlock tbBody = new TextBlock();

                // set text
                tbTitle.Text = "ROUND " + roundCount;
                tbBody.Text = summary;

                //set styling
                tbTitle.Foreground = titleTextBrush;
                tbTitle.FontFamily = shuiFont;
                tbBody.TextWrapping = TextWrapping.WrapWholeWords;

                // add as children to grid
                roundSummariesGrid.Children.Add(tbTitle);
                roundSummariesGrid.Children.Add(tbBody);

                //set grid rows
                Grid.SetRow(tbTitle, rowIndex);
                Grid.SetRow(tbBody, rowIndex + 1);

                roundCount++;
                rowIndex = rowIndex + 2;
            }
        }

    }
}
