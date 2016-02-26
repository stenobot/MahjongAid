using System.Collections.Generic;

namespace MahjongScorer
{
    public class Player
    {

        public string Name { get; set; }

        public Wind CurrentWind { get; set; }

        public bool IsDealer { get; set; }

        public int ConsecutiveWinsAsDealer { get; set; }

        public bool IsGameWinner { get; set; }

        public int TotalScore { get; set; }

        public List<int> RoundScores { get; set; }
    }
}
