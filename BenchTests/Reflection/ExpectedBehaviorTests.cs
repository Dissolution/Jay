using System.Drawing;
using static InlineIL.IL;
using static InlineIL.IL.Emit;

namespace Jay.BenchTests.Reflection;

public class ExpectedBehaviorTests
{
    [Fact]
    public void BoxedStructTests()
    {
        var value = new Point(3, 3);
        object boxed = (object) value;
        Assert.True(boxed != null);
            
        MutateBoxedPoint(boxed);
        Assert.True(boxed != null);
        Assert.True(boxed is Point);
        Point unboxed = (Point) boxed!;
        Assert.True(unboxed.Equals(value));
    }
        
    [Fact]
    public void BoxedStructReBoxTests()
    {
        var value = new Point(3, 3);
        object boxed = (object) value;
        Assert.True(boxed != null);
            
        MutateBoxedPointAndRebox(boxed);
        Assert.True(boxed != null);
        Assert.True(boxed is Point);
        Point unboxed = (Point) boxed!;
        Assert.True(unboxed.Equals(value));
    }
        
    [Fact]
    public void BoxedStructIlTests()
    {
        var value = new Point(3, 3);
        object boxed = (object) value;
        Assert.True(boxed != null);
            
        MutateBoxedPointIL(boxed);
        Assert.True(boxed != null);
        Assert.True(boxed is Point);
        Point unboxed = (Point) boxed!;
        Assert.True(!unboxed.Equals(value));
        Assert.True(unboxed.X == value.X + 1 &&
                    unboxed.Y == value.Y + 1);
    }

    [Fact]
    public void BoxedStructObjTests()
    {
        Point point = new Point(3, 3);
        object boxed = (object) point;
        ref Point refPoint = ref GetRefPoint(boxed);
        Assert.True(refPoint.X == point.X);
        Assert.True(refPoint.Y == point.Y);
        refPoint.X++;
        refPoint.Y++;
        Assert.True(refPoint.X == point.X + 1);
        Assert.True(refPoint.Y == point.Y + 1);
    }
        
    [Fact]
    public void BoxedStructRefObjTests()
    {
        Point point = new Point(3, 3);
        object boxed = (object) point;
        ref Point refPoint = ref GetRefPoint(ref boxed);
        Assert.True(refPoint.X == point.X);
        Assert.True(refPoint.Y == point.Y);
        refPoint.X++;
        refPoint.Y++;
        Assert.True(refPoint.X == point.X + 1);
        Assert.True(refPoint.Y == point.Y + 1);
    }
        
    private static void MutateBoxedPoint(object? boxedPoint)
    {
        Assert.True(boxedPoint != null);
        if (boxedPoint is Point point)
        {
            Assert.True(point.X != 0 && point.Y != 0);
            point.X += 1;
            point.Y += 1;
        }
        else
        {
            throw new ArgumentException("", nameof(boxedPoint));
        }
    }
        
    private static void MutateBoxedPointAndRebox(object? boxedPoint)
    {
        Assert.True(boxedPoint != null);
        if (boxedPoint is Point point)
        {
            Assert.True(point.X != 0 && point.Y != 0);
            point.X += 1;
            point.Y += 1;
            boxedPoint = point;
        }
        else
        {
            throw new ArgumentException("", nameof(boxedPoint));
        }
    }
        
        
    private static void MutateBoxedPointIL(object? boxedPoint)
    {
        Assert.True(boxedPoint != null);
        ref Point point = ref GetRefPoint(boxedPoint!);
        Assert.True(point.X != 0 && point.Y != 0);
        point.X += 1;
        point.Y += 1;
    }

    private static ref Point GetRefPoint(object boxedPoint)
    {
        Ldarg(nameof(boxedPoint));
        Unbox<Point>();
        return ref ReturnRef<Point>();
    }
        
    private static ref Point GetRefPoint(ref object boxedPoint)
    {
        Ldarg(nameof(boxedPoint));
        Ldind_Ref();
        Unbox<Point>();
        return ref ReturnRef<Point>();
    }

       
}