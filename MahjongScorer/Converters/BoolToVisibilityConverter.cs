using System;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Data;

namespace MahjongScorer.Converters
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        /// <summary>
        /// Converts a bool value to a Visibility value. True is visible, false is collapsed
        /// </summary>
        /// <param name="value">The value to convert</param>
        /// <param name="targetType">Required by interface, but not used</param>
        /// <param name="parameter">Required by interface, but not used</param>
        /// <param name="language">Required by interface, but not used</param>
        /// <returns>The converted value</returns>
        public object Convert(object value, Type targetType, object parameter, string language)
        {
            if (value == null)
                return Visibility.Collapsed;

            bool boolVal = (bool)value;
            if (boolVal)
                return Visibility.Visible;
            else
                return Visibility.Collapsed;
        }



        /// <summary>
        /// This method is required by the interface, but is not supported
        /// </summary>
        /// <param name="value">Not supported</param>
        /// <param name="targetType">Not supported</param>
        /// <param name="parameter">Not supported</param>
        /// <param name="language">Not supported</param>
        /// <returns></returns>
        [Obsolete("Not supported")]
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            throw new NotSupportedException("Converting a TTML URI to VTT URI is not supported");
        }
    }
}
