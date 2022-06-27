// Decompiled with JetBrains decompiler
// Type: System.Drawing.PointF
// Assembly: System.Drawing.Primitives, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D0804761-9704-49D2-9F69-9EE51CF26D7A
// Assembly location: C:\Program Files\dotnet\shared\Microsoft.NETCore.App\5.0.4\System.Drawing.Primitives.dll

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;


#nullable enable
namespace Jay.Geometry;

[TypeForwardedFrom("System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
[Serializable]
public struct PointF : IEquatable<PointF>
{
  public static readonly PointF Empty;
  private float x;
  private float y;

  public PointF(float x, float y)
  {
    this.x = x;
    this.y = y;
  }

  [Browsable(false)]
  public readonly bool IsEmpty => (double) this.x == 0.0 && (double) this.y == 0.0;

  public float X
  {
    readonly get => this.x;
    set => this.x = value;
  }

  public float Y
  {
    readonly get => this.y;
    set => this.y = value;
  }

  public static PointF operator +(PointF pt, Size sz) => PointF.Add(pt, sz);

  public static PointF operator -(PointF pt, Size sz) => PointF.Subtract(pt, sz);

  public static PointF operator +(PointF pt, SizeF sz) => PointF.Add(pt, sz);

  public static PointF operator -(PointF pt, SizeF sz) => PointF.Subtract(pt, sz);

  public static bool operator ==(PointF left, PointF right) => (double) left.X == (double) right.X && (double) left.Y == (double) right.Y;

  public static bool operator !=(PointF left, PointF right) => !(left == right);

  public static PointF Add(PointF pt, Size sz) => new PointF(pt.X + (float) sz.Width, pt.Y + (float) sz.Height);

  public static PointF Subtract(PointF pt, Size sz) => new PointF(pt.X - (float) sz.Width, pt.Y - (float) sz.Height);

  public static PointF Add(PointF pt, SizeF sz) => new PointF(pt.X + sz.Width, pt.Y + sz.Height);

  public static PointF Subtract(PointF pt, SizeF sz) => new PointF(pt.X - sz.Width, pt.Y - sz.Height);

  public override readonly bool Equals(object? obj) => obj is PointF other && this.Equals(other);

  public readonly bool Equals(PointF other) => this == other;

  public override readonly int GetHashCode()
  {
    float num = this.X;
    int hashCode1 = num.GetHashCode();
    num = this.Y;
    int hashCode2 = num.GetHashCode();
    return HashCode.Combine<int, int>(hashCode1, hashCode2);
  }

  public override readonly string ToString() => "{X=" + this.x.ToString() + ", Y=" + this.y.ToString() + "}";
}