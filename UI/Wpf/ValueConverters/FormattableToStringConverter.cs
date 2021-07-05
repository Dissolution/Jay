using System;
using System.Diagnostics;
using System.Globalization;
using System.Windows.Data;

namespace Jay.UI.Wpf.ValueConverters
{
	/// <summary>
	/// Converts <see cref="IFormattable"/> values to <see cref="string"/> values using Format strings as a parameter.
	/// </summary>
	[ValueConversion(typeof(IFormattable), typeof(string))]
	public sealed class FormattableToStringConverter : IValueConverter
	{
		public object? Convert(object? value, Type targetType, object? parameter, CultureInfo? culture)
		{
			if (value is IFormattable formattable)
			{
				// Use the parameter as the format string
				string? format = parameter?.ToString();
				if (!string.IsNullOrWhiteSpace(format))
				{
					return formattable.ToString(format, culture);
				}
				else
				{
					// Fallback to default ToString()
					return formattable.ToString();
				}
			}
			else
			{
				//Fail
				return Binding.DoNothing;
			}
		}

		public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo? culture)
		{
			//Fail
			Debugger.Break();
			return Binding.DoNothing;
		}
	}
}
