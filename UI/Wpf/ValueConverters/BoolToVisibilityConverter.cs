using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Jay.UI.Wpf.ValueConverters
{
	/// <summary>
	/// Converts <see cref="bool"/> values to <see cref="Visibility"/> values.
	/// </summary>
	[ValueConversion(typeof(bool), typeof(Visibility))]
	public sealed class BoolToVisibilityConverter : IValueConverter
	{
		private static Visibility GetVisibility(object? parameter)
		{
			//Defaults to Visible
			if (parameter is null)
				return Visibility.Visible;

			//Direct convert?
			if (parameter is Visibility visibility)
				return visibility;

			//Try Parse
			if (Enum.TryParse(parameter.ToString(), true, out Visibility vEnum))
				return vEnum;
			
			throw new ArgumentException("Invalid Visibility specified as the ConverterParameter. Please use 'Visible', 'Collapsed', or 'Hidden'.");
		}

		public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			if (value is bool b)
			{
				//Check if our parameter indicates invert
				if (GetVisibility(parameter) == Visibility.Collapsed)
					b = !b;
				//Convert
				return b ? Visibility.Visible : Visibility.Collapsed;
			}
			else
			{
				//Fail
				return Binding.DoNothing;
			}
		}

		public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
		{
			if (value is Visibility visibility)
			{
				var isVisible = visibility == Visibility.Visible;
				if (GetVisibility(parameter) == Visibility.Collapsed)
					isVisible = !isVisible;
				return isVisible;
			}
			else
			{
				//Fail
				return Binding.DoNothing;
			}
		}
	}
}
