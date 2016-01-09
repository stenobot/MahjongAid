using Windows.UI.Xaml.Controls;

namespace MahjongScorer
{
    public class SetCheckBox : CheckBox
    {
        private string _setType;
        private string _checkBoxType;

        public string SetType
        {
            get { return _setType; }
            set
            {
                if (_setType != value)
                    _setType = value;
            }
        }

        public string CheckBoxType
        {
            get { return _checkBoxType; }
            set
            {
                if (_checkBoxType != value)
                    _checkBoxType = value;
            }
        }
    }
}
