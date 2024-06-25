using System;
using System.Globalization;
using Avalonia.Data.Converters;

namespace Events.Models;

public class TimeSpanTypeConverter : IValueConverter
{
    public static readonly TimeSpanTypeConverter Instance = new TimeSpanTypeConverter();

    public object Convert(object value, Type targetType, object parameter, CultureInfo culture) 
        => (value is TimeSpan timeSpan) 
            ? timeSpan.ToString() 
            : string.Empty;

    public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) 
        => (value is string str && TimeSpan.TryParse(str, out var timeSpan)) 
            ? timeSpan 
            : TimeSpan.Zero;
}
