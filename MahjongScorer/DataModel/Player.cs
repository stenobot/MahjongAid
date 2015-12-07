using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahjongScorer
{
    public class Player
    {
        private string _name;
        private Wind _currentWind;
        private bool _isDealer;
        private bool _isRoundWinner;
        private bool _isGameWinner;
        private int _totalScore;
        private List<int> _roundScores;

      
        
        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                    _name = value;
            }
        }

        public Wind CurrentWind
        {
            get { return _currentWind; }
            set
            {
                if (_currentWind != value)
                    _currentWind = value;
            }
        }

        public bool IsDealer
        {
            get { return _isDealer; }
            set
            {
                if (_isDealer != value)
                    _isDealer = value;
            }
        }

        public bool IsRoundWinner
        {
            get { return _isRoundWinner; }
            set
            {
                if (_isRoundWinner != value)
                    _isRoundWinner = value;
            }
        }

        public bool IsGameWinner
        {
            get { return _isGameWinner; }
            set
            {
                if (_isGameWinner != value)
                    _isGameWinner = value;
            }
        }

        public int TotalScore
        {
            get { return _totalScore; }
            set
            {
                if (_totalScore != value)
                    _totalScore = value;
            }
        }

        public List<int> RoundScores
        {
            get { return _roundScores; }
            set
            {
                if (_roundScores != value)
                    _roundScores = value;
            }
        }
    }
}
