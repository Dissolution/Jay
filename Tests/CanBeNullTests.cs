using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Jay.Reflection;

namespace Tests
{
    public class CanBeNullTests
    {
        [Fact]
        public void Null()
        {
            Assert.True(((Type?)null).CanBeNull());
        }

        [Fact]
        public void Nullable()
        {
            Assert.True(typeof(int?).CanBeNull());
            Assert.True(typeof(Nullable<int>).CanBeNull());
            Assert.True(typeof(Nullable<>).CanBeNull());
            Assert.True(typeof(Nullable<>).MakeGenericType<Guid>().CanBeNull());
        }

        [Fact]
        public void ValueTypes()
        {
            Assert.False(typeof(int).CanBeNull());
            Assert.False(typeof(BindingFlags).CanBeNull());
            Assert.False(typeof(Span<byte>).CanBeNull());
            Assert.False(typeof(ReadOnlySpan<>).CanBeNull());
        }

        [Fact]
        public void ClassTypes()
        {
            Assert.True(typeof(object).CanBeNull());
            Assert.True(typeof(string).CanBeNull());
            Assert.True(typeof(CanBeNullTests).CanBeNull());
            Assert.True(typeof(ICloneable).CanBeNull());
            Assert.True(typeof(ISpanFormattable).CanBeNull());
        }

        [Fact]
        public unsafe void Pointers()
        {
            Assert.True(typeof(void*).CanBeNull());
            Assert.True(typeof(char*).CanBeNull());
        }

        [Fact]
        public void Refs()
        {
            Assert.True(typeof(byte).MakeByRefType().CanBeNull());
            Assert.True(typeof(object).MakeByRefType().CanBeNull());
        }
    }
}
