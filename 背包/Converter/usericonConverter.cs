using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Windows.UI.Xaml.Data;
using 背包.MobileService;

namespace 背包.Converter
{
    class usericonConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, string language)
        {;
            return "https://bagblob.blob.core.windows.net/users/" + value + ".png";
        }
        public object ConvertBack(object value, Type targetType, object parameter, string language)
        {
            return null;
        }
    }
}
