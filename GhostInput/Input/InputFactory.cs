namespace Jay.GhostInput.Input
{
	internal static class InputFactory
	{
		public static MouseInputFactory Mouse { get; } = new MouseInputFactory();
		public static KeyboardInputFactory Keyboard { get; } = new KeyboardInputFactory();
	}
}


