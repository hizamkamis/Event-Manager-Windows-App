using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows.Data;
using System.Globalization;
using System.Windows;
using EventManagerPro.Models;

namespace EventManagerPro.Converters
{
    [ValueConversion(typeof(EventModel), typeof(double))]
    public class ProgressPercentageConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            EventModel curEvent = (EventModel)value;
            double current = 0;
            int max = 100;

            switch (parameter.ToString())
            {
                case "Guests":
                    current = (double)curEvent.Guests.Count;
                    max = curEvent.Capacity;
                    break;
            }

            return (current / max) * 100;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return null;
        }
    }
}
