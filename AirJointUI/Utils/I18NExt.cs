using AirJointUI.Properties;
using Avalonia.Markup.Xaml.MarkupExtensions;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirJointUI.Utils
{
    public class I18NExt
    {
        private static CultureInfo culture;
        public static CultureInfo Culture
        {
            set
            {
                culture = value;
            }
            get
            {
                return culture;
            }
        }
        public static string? Translate(string key, string? fallbackValue = null)
        {
            string tmpstr = Resources.ResourceManager.GetString(key, culture);
            return tmpstr ?? fallbackValue;
        }
    }
}
