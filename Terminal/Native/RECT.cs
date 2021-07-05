namespace Jay.Consolas
{
	internal struct RECT
	{
		public int left;
		public int top;
		public int right;
		public int bottom;

		/// <inheritdoc />
		public override string ToString()
		{
			return $"{{[{left},{top}],[{right},{bottom}]}}";
		}
	}
}
