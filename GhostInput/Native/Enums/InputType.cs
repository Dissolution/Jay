namespace Jay.GhostInput.Native.Enums
{
	/// <summary>
	/// Specifies the type of the input event. 
	/// </summary>
	internal enum InputType : uint
	{
		/// <summary>
		/// INPUT_MOUSE = 0x00
		/// (This event is a mouse event Use the <c>mi</c> struture of the union.)
		/// </summary>
		Mouse = 0x00,

		/// <summary>
		/// INPUT_KEYBOARD = 0x01
		/// (This event is a keyboard event. Use the <c>ki</c> structure of the union.)
		/// </summary>
		Keyboard = 0x01,

		/// <summary>
		/// INPUT_HARDWARE = 0x02 
		/// (The event is a hardware event. Use the <c>hi</c> structure of the union.)
		/// </summary>
		Hardware = 0x02,
	}
}
