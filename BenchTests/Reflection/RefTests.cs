using System.Drawing;

namespace Jay.BenchTests.Reflection
{
    public class RefTests
    {
        [Fact]
        public void RefBoxUnboxTest()
        {
            Point point = new Point(3, 3);
            object boxed = (object)point;
            Point unboxed = (Point)boxed;
            Assert.True(unboxed.X == point.X);
            Assert.True(unboxed.Y == point.Y);
            unboxed.X++;
            unboxed.Y++;
            Assert.True(unboxed.X == point.X + 1);
            Assert.True(unboxed.Y == point.Y + 1);
        }
    }
}