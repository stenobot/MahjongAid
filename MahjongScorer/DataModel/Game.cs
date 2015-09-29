﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahjongScorer
{
    public class Game
    {
        private DateTime _dateCreated;
        private Guid _guid;

        private int _currentRound;
        private int _currentRoundScore;
        private int _timesDealerWon;
        private bool _inProgress;
        private Wind _prevailingWind;

        private RuleSet _ruleSet;

        private StringBuilder _currentRoundSummary;
        private List<string> _roundSummaries;
        
        private List<Player> _players;

        private List<WinCondition> _winConditions;

        // using these only for save data lists
        private string _currentDealerName;
        private string _winnerName;


        public DateTime DateCreated
        {
            get { return _dateCreated; }
            set
            {
                if (_dateCreated != value)
                    _dateCreated = value;
            }
        }

        public Guid Guid
        {
            get { return _guid; }
            set
            {
                if (_guid != value)
                    _guid = value;
            }
        }

        public int CurrentRound
        {
            get { return _currentRound; }
            set
            {
                if (_currentRound != value)
                    _currentRound = value;
            }
        }

        public int CurrentRoundScore
        {
            get { return _currentRoundScore; }
            set
            {
                if (_currentRoundScore != value)
                    _currentRoundScore = value;
            }
        }


        public int TimesDealerWon
        {
            get { return _timesDealerWon; }
            set
            {
                if (_timesDealerWon != value)
                    _timesDealerWon = value;
            }
        }

        public bool InProgress
        {
            get { return _inProgress; }
            set
            {
                if (_inProgress != value)
                    _inProgress = value;
            }
        }

        public Wind PrevailingWind
        {
            get { return _prevailingWind; }
            set
            {
                if (_prevailingWind != value)
                    _prevailingWind = value;
            }
        }

        public RuleSet RuleSet
        {
            get { return _ruleSet; }
            set
            {
                if (_ruleSet != value)
                    _ruleSet = value;
            }
        }

        public StringBuilder CurrentRoundSummary
        {
            get { return _currentRoundSummary; }
            set
            {
                if (_currentRoundSummary != value)
                    _currentRoundSummary = value;
            }
        }

        public List<string> RoundSummaries
        {
            get { return _roundSummaries; }
            set
            {
                if (_roundSummaries != value)
                    _roundSummaries = value;
            }
        }

        public List<Player> Players
        {
            get { return _players; }
            set
            {
                if (_players != value)
                    _players = value;
            }
        }

        public List<WinCondition> WinConditions
        {
            get { return _winConditions; }
            set
            {
                if (_winConditions != value)
                    _winConditions = value;
            }
        }

        public string CurrentDealerName
        {
            get { return _currentDealerName; }
            set
            {
                if (_currentDealerName != value)
                    _currentDealerName = value;
            }
        }

        public string WinnerName
        {
            get { return _winnerName; }
            set
            {
                if (_winnerName != value)
                    _winnerName = value;
            }
        }
    }
}
