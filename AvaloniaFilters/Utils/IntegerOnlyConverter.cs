using Avalonia.Data.Converters;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AvaloniaFilters
{
    internal class IntegerOnlyConverter : IValueConverter
    {
        public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value != null)
            {
                string str = new string((value?.ToString() ?? "").Where(c => char.IsDigit(c)).ToArray());

                int result;

                if (int.TryParse(str, out result))
                    return result.ToString();
            }

            return value;
        }

        public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
        {
            if (value == null)
                return 0;

            int result;

            if (int.TryParse(value.ToString(), out result))
                return result;

            return 0;
        }
    }
}
