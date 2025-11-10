using AirJointUI.Properties;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Data;
using Avalonia.Markup.Xaml;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AirJointUI.Utils
{
    public class ResxExtension : MarkupExtension
    {
        public string ResourceKey { get; set; }
        public ResxExtension(string resourceKey)
        {
            ResourceKey = resourceKey;
        }
        public override object ProvideValue(IServiceProvider serviceProvider)
        {
            return Resources.ResourceManager.GetString(ResourceKey, I18NExt.Culture);
        }
    }
}
