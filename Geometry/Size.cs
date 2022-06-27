// Decompiled with JetBrains decompiler
// Type: System.Drawing.Size
// Assembly: System.Drawing.Primitives, Version=5.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a
// MVID: D0804761-9704-49D2-9F69-9EE51CF26D7A
// Assembly location: C:\Program Files\dotnet\shared\Microsoft.NETCore.App\5.0.4\System.Drawing.Primitives.dll

using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

#nullable enable
namespace Jay.Geometry;

[TypeForwardedFrom("System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
[TypeConverter("System.Drawing.SizeConverter, System.Drawing, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b03f5f7f11d50a3a")]
[Serializable]
public struct Size : IEquatable<Size>
{
  public static readonly Size Empty;
  private int width;
  private int height;

  public Size(System.Drawing.Point pt)
  {
    this.width = pt.X;
    this.height = pt.Y;
  }

  public Size(int width, int height)
  {
    this.width = width;
    this.height = height;
  }

  public static implicit operator Jay.Geometry.SizeF(Size p) => new Jay.Geometry.SizeF((float) p.Width, (float) p.Height);

  public static Size operator +(Size sz1, Size sz2) => Size.Add(sz1, sz2);

  public static Size operator -(Size sz1, Size sz2) => Size.Subtract(sz1, sz2);

  public static Size operator *(int left, Size right) => Size.Multiply(right, left);

  public static Size operator *(Size left, int right) => Size.Multiply(left, right);

  public static Size operator /(Size left, int right) => new Size(left.width / right, left.height / right);

  public static Jay.Geometry.SizeF operator *(float left, Size right) => Size.Multiply(right, left);

  public static Jay.Geometry.SizeF operator *(Size left, float right) => Size.Multiply(left, right);

  public static Jay.Geometry.SizeF operator /(Size left, float right) => new Jay.Geometry.SizeF((float) left.width / right, (float) left.height / right);

  public static bool operator ==(Size sz1, Size sz2) => sz1.Width == sz2.Width && sz1.Height == sz2.Height;

  public static bool operator !=(Size sz1, Size sz2) => !(sz1 == sz2);

  public static explicit operator System.Drawing.Point(Size size) => new System.Drawing.Point(size.Width, size.Height);

  [Browsable(false)]
  public readonly bool IsEmpty => this.width == 0 && this.height == 0;

  public int Width
  {
    readonly get => this.width;
    set => this.width = value;
  }

  public int Height
  {
    readonly get => this.height;
    set => this.height = value;
  }

  public static Size Add(Size sz1, Size sz2) => new Size(sz1.Width + sz2.Width, sz1.Height + sz2.Height);

  public static Size Ceiling(Jay.Geometry.SizeF value) => new Size((int) System.Math.Ceiling((double) value.Width), (int) System.Math.Ceiling((double) value.Height));

  public static Size Subtract(Size sz1, Size sz2) => new Size(sz1.Width - sz2.Width, sz1.Height - sz2.Height);

  public static Size Truncate(Jay.Geometry.SizeF value) => new Size((int) value.Width, (int) value.Height);

  public static Size Round(Jay.Geometry.SizeF value) => new Size((int) System.Math.Round((double) value.Width), (int) System.Math.Round((double) value.Height));

  public override readonly bool Equals(object? obj) => obj is Size other && this.Equals(other);

  public readonly bool Equals(Size other) => this == other;

  public override readonly int GetHashCode() => HashCode.Combine<int, int>(this.Width, this.Height);

  public override readonly string ToString() => "{Width=" + this.width.ToString() + ", Height=" + this.height.ToString() + "}";

  private static Size Multiply(Size size, int multiplier) => new Size(size.width * multiplier, size.height * multiplier);

  private static Jay.Geometry.SizeF Multiply(Size size, float multiplier) => new Jay.Geometry.SizeF((float) size.width * multiplier, (float) size.height * multiplier);
}