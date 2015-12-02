using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahjongScorer
{
    public class Rule
    {
        private string _name;
        private string _description;
        private int? _score;
        private int _double;
        private bool _showInList;
        private bool _isUncommon;

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

        public int Double
        {
            get { return _double; }
            set
            {
                if (_double != value)
                    _double = value;
            }
        }

        public bool ShowInList
        {
            get { return _showInList; }
            set
            {
                if (_showInList != value)
                    _showInList = value;
            }
        }

        public bool IsUncommon
        {
            get { return _isUncommon; }
            set
            {
                if (_isUncommon != value)
                    _isUncommon = value;
            }
        }
    }
}
