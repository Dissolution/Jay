using System.Drawing;
using System.Runtime.InteropServices;

namespace Jay.Consolas
{
	/// <summary>
	/// Win32 COLORREF, used to specify an RGB color.
	/// https://msdn.microsoft.com/en-us/library/windows/desktop/dd183449(v=vs.85).aspx
	/// </summary>
	[StructLayout(LayoutKind.Explicit, Size = 4)]
	internal struct COLORREF 
	{
		[FieldOffset(0)]
		public byte R;
		[FieldOffset(1)]
		public byte G;
		[FieldOffset(2)]
		public byte B;

		[FieldOffset(0)]
		public uint Value;

		public COLORREF(byte r, byte g, byte b) {
			Value = 0;
			R = r;
			G = g;
			B = b;
		}

		public COLORREF(uint value) {
			R = 0;
			G = 0;
			B = 0;
			Value = value & 0x00FFFFFF;
		}

		public COLORREF(Color color)
		{
			R = G = B = 0;
			Value = (uint)color.R + (((uint)color.G) << 8) + (((uint)color.B) << 16);
		}

		public Color GetColor()
		{
			return System.Drawing.Color.FromArgb((int) (0x000000FFU & Value),
				(int) (0x0000FF00U & Value) >> 8, (int) (0x00FF0000U & Value) >> 16);
		}

		/// <inheritdoc />
		public override string ToString()
		{
			return $"#{Value:x8}";
		}

		public static implicit operator Color(COLORREF colorref)
		{
			return Color.FromArgb(
				(int) (0x000000FFU & colorref.Value),
				(int) (0x0000FF00U & colorref.Value) >> 8, 
				(int) (0x00FF0000U & colorref.Value) >> 16);
		}

		public static implicit operator COLORREF(Color color)
		{
			return new COLORREF(color);
		}
	}
}
