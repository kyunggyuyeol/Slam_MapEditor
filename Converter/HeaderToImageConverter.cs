using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media.Imaging;

namespace Slam_MapEditor.Converter
{
    #region HeaderToImageConverter

    /// <summary>
    /// 파일이름의 확장자 규칙에 따라 아이콘 Uri 값으로 변환하는 Converter 클래스
    /// </summary>
    [ValueConversion(typeof(string), typeof(bool))]
    public class HeaderToImageConverter : IValueConverter
    {
        public static HeaderToImageConverter Instance = new HeaderToImageConverter();

        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return null;

            //if ((value as string).Contains(@"\"))
            if ((value as string).Contains(@".map"))
            {
                Uri uri = new Uri("pack://application:,,,/Images/icon/mapProject.png");
                BitmapImage source = new BitmapImage(uri);
                return source;
            }
            else
            {
                Uri uri = new Uri("pack://application:,,,/Images/icon/folder.png");
                BitmapImage source = new BitmapImage(uri);
                return source;
            }
        } 

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotSupportedException("Cannot convert back");
        }
    }

    #endregion // DoubleToIntegerConverter 
}
