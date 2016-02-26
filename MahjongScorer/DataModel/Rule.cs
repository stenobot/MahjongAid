using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MahjongScorer
{
    public class Rule
    {
        public string Name { get; set; }

        public string Description { get; set; }

        public int? Score { get; set; }

        public int Double { get; set; }

        public bool ShowInList { get; set; }

        public bool IsUncommon { get; set; }
    }
}
