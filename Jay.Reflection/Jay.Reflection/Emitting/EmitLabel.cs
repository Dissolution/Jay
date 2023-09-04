using Jay.Utilities;
#if NET7_0_OR_GREATER
using System.Numerics;
#endif

namespace Jay.Reflection.Emitting;

public readonly struct EmitLabel :
#if NET7_0_OR_GREATER
        IEqualityOperators<EmitLabel, EmitLabel, bool>, 
        IEqualityOperators<EmitLabel, Label, bool>,
#endif
        IEquatable<EmitLabel>,
        IEquatable<Label>,
        IToCode
{
    public static bool operator ==(EmitLabel left, EmitLabel right) => left.Equals(right);

    public static bool operator !=(EmitLabel left, EmitLabel right) => !left.Equals(right);

    public static bool operator ==(EmitLabel left, Label right) => left.Equals(right);

    public static bool operator !=(EmitLabel left, Label right) => !left.Equals(right);


    public string Name { get; }

    public int Position { get; }

    public bool IsShortForm => Position is >= 0 and <= 127;

    public EmitLabel(string name, int position)
    {
        this.Name = name;
        this.Position = position;
    }

    public EmitLabel(string name, Label label)
    {
        this.Name = name;
        this.Position = label.GetHashCode(); // hack
    }

    public bool Equals(EmitLabel emitLabel)
    {
        return Easy.FastEqual(this.Name, emitLabel.Name) &&
            this.Position == emitLabel.GetHashCode();
    }

    public bool Equals(Label label)
    {
        return this.Position == label.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj switch
        {
            EmitLabel emitLabel => Equals(emitLabel),
            Label label => Equals(label),
            _ => false,
        };
    }

    public override int GetHashCode()
    {
        return Hasher.Combine(Name, Position);
    }

    public void WriteCodeTo(CodeBuilder codeBuilder)
    {
        codeBuilder.Append("0x");
        codeBuilder.Append(this.Position, "X4");
        codeBuilder.Append(" | ");
        codeBuilder.Append(this.Name);
        codeBuilder.Append(':');
    }

    public override string ToString() => CodeBuilder.Render(this);
}