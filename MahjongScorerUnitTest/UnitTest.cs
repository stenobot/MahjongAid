using MahjongScorer;
using Microsoft.VisualStudio.TestPlatform.UnitTestFramework;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Runtime.Serialization.Json;
using System.IO;
using Windows.Storage;

namespace MahjongScorerUnitTest
{
    [TestClass]
    public class UnitTest1
    {
        List<Game> SavedGamesList;

        /// <summary>
        /// Tests that the round scores for each player add up to zero in each game
        /// </summary>
        [TestMethod]
        public void TestRoundScoreValues()
        {
            GetSavedGamesAsync();

            foreach (Game game in SavedGamesList)
            {
                for (int i = 0; i < game.Players[0].RoundScores.Count; i++)
                {
                    int p1Score = game.Players[0].RoundScores[i];
                    int p2Score = game.Players[1].RoundScores[i];
                    int p3Score = game.Players[2].RoundScores[i];
                    int p4Score = game.Players[3].RoundScores[i];

                    Assert.AreEqual(p1Score + p2Score + p3Score + p4Score, 0);
                }
            }
        }


        private async Task GetSavedGamesAsync()
        {
            // create new instance of saved games list
            SavedGamesList = new List<Game>();

            DataContractJsonSerializer serializer = new DataContractJsonSerializer(typeof(List<Game>));

            Stream stream = await ApplicationData.Current.RoamingFolder.OpenStreamForReadAsync("mahjong-data.json");

            // deserialize game data, cast to List of type Game, and assign it to our saved games List
            SavedGamesList = (List<Game>)serializer.ReadObject(stream);
        }
    }
}
