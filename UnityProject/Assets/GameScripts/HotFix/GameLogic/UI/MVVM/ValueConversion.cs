using System;
using System.Globalization;

namespace GameLogic.UI.MVVM
{
    public interface IValueConverter
    {
        bool TryConvert(object value, Type targetType, out object result);
        bool TryConvertBack(object value, Type targetType, out object result);
    }

    public sealed class DefaultValueConverter : IValueConverter
    {
        public static readonly DefaultValueConverter Instance = new DefaultValueConverter();

        private DefaultValueConverter() { }

        public bool TryConvert(object value, Type targetType, out object result)
        {
            return TryConvertInternal(value, targetType, out result);
        }

        public bool TryConvertBack(object value, Type targetType, out object result)
        {
            return TryConvertInternal(value, targetType, out result);
        }

        private static bool TryConvertInternal(object value, Type targetType, out object result)
        {
            if (targetType == null)
            {
                result = null;
                return false;
            }

            if (value == null)
            {
                result = CreateDefault(targetType);
                return true;
            }

            var valueType = value.GetType();
            if (targetType.IsAssignableFrom(valueType))
            {
                result = value;
                return true;
            }

            try
            {
                result = Convert.ChangeType(value, targetType, CultureInfo.InvariantCulture);
                return true;
            }
            catch
            {
                result = null;
                return false;
            }
        }

        private static object CreateDefault(Type targetType)
        {
            if (!targetType.IsValueType)
            {
                return null;
            }

            return Activator.CreateInstance(targetType);
        }
    }
}
