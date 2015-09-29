using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahjongScorer
{
    public class Set
    {
        private int _index; 
        private string _name;
        private bool _isTerminalOrHonors;
        private bool _isConcealed;

        public int Index
        {
            get { return _index; }
            set
            {
                if (_index != value)
                    _index = value;
            }
        }

        public string Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                    _name = value;
            }
        }

        public bool IsTerminalOrHonors
        {
            get { return _isTerminalOrHonors; }
            set
            {
                if (_isTerminalOrHonors != value)
                    _isTerminalOrHonors = value;
            }
        }

        public bool IsConcealed
        {
            get { return _isConcealed; }
            set
            {
                if (_isConcealed != value)
                    _isConcealed = value;
            }
        }
    }
}
