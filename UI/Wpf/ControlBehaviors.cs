using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;

namespace Jay.UI.Wpf
{
	/// <summary>
	/// Utility class for controlling common <see cref="Control"/> behaviors.
	/// </summary>
	public static class ControlBehaviors
	{
		private static bool _selectAllTextOnTextBoxFocus = false;
		private static bool _selectAllTextOnTextBoxFocusRegistered = false;
		private static bool _triggerLeaveEventsOnButtonFocus = false;
		private static bool _triggerLeaveEventsOnButtonFocusRegisterd = false;

		/// <summary>
		/// Should all text in a <see cref="TextBox"/> or <see cref="PasswordBox"/> be selected when it is focussed on?
		/// </summary>
		public static bool SelectAllTextOnTextBoxFocus
		{
			get => _selectAllTextOnTextBoxFocus;
			set
			{
				_selectAllTextOnTextBoxFocus = value;
				if (value && !_selectAllTextOnTextBoxFocusRegistered)
					RegisterSelectAllTextOnTextBoxFocus();
			}
		}

		/// <summary>
		/// Should button focus trigger leave events?
		/// </summary>
		public static bool TriggerLeaveEventsOnButtonFocus
		{
			get => _triggerLeaveEventsOnButtonFocus;
			set
			{
				_triggerLeaveEventsOnButtonFocus = value;
				if (value && !_triggerLeaveEventsOnButtonFocusRegisterd)
					RegisterButtonLeaveFocus();
			}
		}

		private static void SelectivelyIgnoreMouseButton(object sender, MouseButtonEventArgs e)
		{
			//Check if we're turned off
			if (!_selectAllTextOnTextBoxFocus)
				return;

			//Find the true parent
			var parent = e.OriginalSource as DependencyObject;
			while (parent != null && !(parent is TextBox) && !(parent is PasswordBox))
				parent = VisualTreeHelper.GetParent(parent);

			//Check if it exists and it's a control
			if (!(parent is UIElement control))
				return;

			//Focused via keyboard?
			if (control.IsKeyboardFocusWithin)
				return;

			//If it isn't, give it focus and be done
			control.Focus();
			e.Handled = true;
		}

		private static void SelectAllText(object sender, RoutedEventArgs e)
		{
			//Check if we're turned off
			if (!_selectAllTextOnTextBoxFocus)
				return;

			//Textbox?
			if (sender is TextBox textBox)
				textBox.SelectAll();
			//PasswordBox?
			else if (sender is PasswordBox passwordBox)
				passwordBox.SelectAll();
		}

		private static void ClickEventHandler(object sender, RoutedEventArgs e)
		{
			if (!_triggerLeaveEventsOnButtonFocus)
				return;

			if (sender is Button button)
			{
				if (button.IsDefault)
					button.Focus();
			}
		}

		private static void RegisterSelectAllTextOnTextBoxFocus()
		{
			var textBoxType = typeof(TextBox);
			var passwordBoxType = typeof(PasswordBox);
			//Make sure we ignore certain textbox focuses
			EventManager.RegisterClassHandler(textBoxType, UIElement.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(SelectivelyIgnoreMouseButton));
			EventManager.RegisterClassHandler(passwordBoxType, UIElement.PreviewMouseLeftButtonDownEvent, new MouseButtonEventHandler(SelectivelyIgnoreMouseButton));
			//Register keyboard focus to select all
			EventManager.RegisterClassHandler(textBoxType, UIElement.GotKeyboardFocusEvent, new RoutedEventHandler(SelectAllText));
			EventManager.RegisterClassHandler(passwordBoxType, UIElement.GotKeyboardFocusEvent, new RoutedEventHandler(SelectAllText));
			//Register double click to also select all
			EventManager.RegisterClassHandler(textBoxType, Control.MouseDoubleClickEvent, new RoutedEventHandler(SelectAllText));
			EventManager.RegisterClassHandler(passwordBoxType, Control.MouseDoubleClickEvent, new RoutedEventHandler(SelectAllText));
			_selectAllTextOnTextBoxFocusRegistered = true;
		}

		private static void RegisterButtonLeaveFocus()
		{
			EventManager.RegisterClassHandler(typeof(Button), ButtonBase.ClickEvent, new RoutedEventHandler(ClickEventHandler));
			_triggerLeaveEventsOnButtonFocusRegisterd = true;
		}
	}
}
