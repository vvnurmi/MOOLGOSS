using System;
using System.Globalization;
using System.Windows.Data;

namespace MOO.Client.GUI
{
    public class GenericBinding<T> : MultiBinding
    {
        private class ValueConverter : IMultiValueConverter
        {
            private Func<T> _convert;

            public ValueConverter(Func<T> convert)
            {
                _convert = convert;
            }

            public object Convert(object[] values, Type targetType, object parameter, CultureInfo culture)
            {
                return _convert();
            }

            public object[] ConvertBack(object value, Type[] targetTypes, object parameter, CultureInfo culture)
            {
                throw new NotImplementedException();
            }
        }

        public GenericBinding(Func<T> getValue, params Binding[] bindings)
        {
            Converter = new ValueConverter(getValue);
            foreach (var b in bindings) Bindings.Add(b);
        }
    }
}
