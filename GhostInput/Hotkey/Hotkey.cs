using System.Windows.Input;

namespace Jay.GhostInput.Hotkey
{
	/// <summary>
	/// Represents a <see cref="System.Windows.Input.KeyModifier"/> and <see cref="System.Windows.Input.Key"/> combination.
	/// </summary>
	public struct Hotkey
	{
		public ModifierKeys KeyModifier { get; }
		public Key Key { get; }

		public Hotkey(ModifierKeys keyModifier, Key key)
		{
			KeyModifier = keyModifier;
			Key = key;
		}
	}
}