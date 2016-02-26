using System;
using System.Collections.Generic;
using System.Text;

namespace MahjongScorer
{
    public class Game
    {
        public DateTime DateCreated { get; set; }

        public Guid Guid { get; set; }

        public int CurrentRound { get; set; }

        public int TimesDealerWon { get; set; }

        public bool InProgress { get; set; }

        public int BaseValue { get; set; }

        public int LimitValue { get; set; }

        public int StartingScore { get; set; }

        public int ReignOfTerrorLimit { get; set; }

        public Wind PrevailingWind { get; set; }

        public List<Wind> PrevailingWinds { get; set; }

        public StringBuilder CurrentRoundSummary { get; set; }

        public List<string> RoundSummaries { get; set; }

        public List<Player> Players { get; set; }

        public List<Rule> Rules { get; set; }

        public string CurrentDealerName { get; set; }

        public string WinnerName { get; set; }

        public bool LoadedFromSave { get; set; }
    }
}
