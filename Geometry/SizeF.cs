// Decompiled with JetBrains decompiler
// Type: System.Drawing.SizeF
// Assembly: System.Drawing.Primitives, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D0804761-9704-49D2-9F69-9EE51CF26D7A
// Assembly location: C:\Program Files\dotnet\shared\Microsoft.NETCore.App\5.0.4\System.Drawing.Primitives.dll

using System;
using System.ComponentModel;
using System.Drawing;
using System.Runtime.CompilerServices;

#nullable enable
namespace Jay.Geometry;

[TypeForwardedFrom("System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
[TypeConverter("System.Drawing.SizeFConverter, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
[Serializable]
public struct SizeF : IEquatable<SizeF>
{
  public static readonly SizeF Empty;
  private float width;
  private float height;

  public SizeF(SizeF size)
  {
    this.width = size.width;
    this.height = size.height;
  }

  public SizeF(System.Drawing.PointF pt)
  {
    this.width = pt.X;
    this.height = pt.Y;
  }

  public SizeF(float width, float height)
  {
    this.width = width;
    this.height = height;
  }

  public static SizeF operator +(SizeF sz1, SizeF sz2) => SizeF.Add(sz1, sz2);

  public static SizeF operator -(SizeF sz1, SizeF sz2) => SizeF.Subtract(sz1, sz2);

  public static SizeF operator *(float left, SizeF right) => SizeF.Multiply(right, left);

  public static SizeF operator *(SizeF left, float right) => SizeF.Multiply(left, right);

  public static SizeF operator /(SizeF left, float right) => new SizeF(left.width / right, left.height / right);

  public static bool operator ==(SizeF sz1, SizeF sz2) => (double) sz1.Width == (double) sz2.Width && (double) sz1.Height == (double) sz2.Height;

  public static bool operator !=(SizeF sz1, SizeF sz2) => !(sz1 == sz2);

  public static explicit operator System.Drawing.PointF(SizeF size) => new System.Drawing.PointF(size.Width, size.Height);

  [Browsable(false)]
  public readonly bool IsEmpty => (double) this.width == 0.0 && (double) this.height == 0.0;

  public float Width
  {
    readonly get => this.width;
    set => this.width = value;
  }

  public float Height
  {
    readonly get => this.height;
    set => this.height = value;
  }

  public static SizeF Add(SizeF sz1, SizeF sz2) => new SizeF(sz1.Width + sz2.Width, sz1.Height + sz2.Height);

  public static SizeF Subtract(SizeF sz1, SizeF sz2) => new SizeF(sz1.Width - sz2.Width, sz1.Height - sz2.Height);

  public override readonly bool Equals(object? obj) => obj is SizeF other && this.Equals(other);

  public readonly bool Equals(SizeF other) => this == other;

  public override readonly int GetHashCode() => HashCode.Combine<float, float>(this.Width, this.Height);

  public readonly System.Drawing.PointF ToPointF() => (System.Drawing.PointF) this;

  public readonly Size ToSize() => Size.Truncate(this);

  public override readonly string ToString() => "{Width=" + this.width.ToString() + ", Height=" + this.height.ToString() + "}";

  private static SizeF Multiply(SizeF size, float multiplier) => new SizeF(size.width * multiplier, size.height * multiplier);
}