using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahjongScorer
{
    public class WinCondition
    {
        private string _name;
        private string _description;
        private int? _score;
        private bool _isDouble;


        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                    _name = value;
            }
        }

        public string Description
        {
            get { return _description; }
            set
            {
                if (_description != value)
                    _description = value;
            }
        }

        public int? Score
        {
            get { return _score; }
            set
            {
                if (_score != value)
                    _score = value;
            }
        }

        public bool IsDouble
        {
            get { return _isDouble; }
            set
            {
                if (_isDouble != value)
                    _isDouble = value;
            }
        }
    }
}
