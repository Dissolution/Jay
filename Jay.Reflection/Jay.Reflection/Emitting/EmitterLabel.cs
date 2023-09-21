using Jay.Utilities;
#if NET7_0_OR_GREATER
using System.Numerics;
#endif

namespace Jay.Reflection.Emitting;

public readonly struct EmitterLabel :
#if NET7_0_OR_GREATER
        IEqualityOperators<EmitterLabel, EmitterLabel, bool>, 
        IEqualityOperators<EmitterLabel, Label, bool>,
#endif
    IEquatable<EmitterLabel>,
    IEquatable<Label>,
    ICodePart
{
    public static bool operator ==(EmitterLabel left, EmitterLabel right) => left.Equals(right);

    public static bool operator !=(EmitterLabel left, EmitterLabel right) => !left.Equals(right);

    public static bool operator ==(EmitterLabel left, Label right) => left.Equals(right);

    public static bool operator !=(EmitterLabel left, Label right) => !left.Equals(right);


    public string Name { get; }

    public int Position { get; }

    public bool IsShortForm => Position is >= 0 and <= 127;

    public EmitterLabel(string name, int position)
    {
        this.Name = name;
        this.Position = position;
    }

    public EmitterLabel(string name, Label label)
    {
        this.Name = name;
        this.Position = label.GetHashCode(); // hack
    }

    public bool Equals(EmitterLabel emitterLabel)
    {
        return Easy.FastEqual(this.Name, emitterLabel.Name) && this.Position == emitterLabel.GetHashCode();
    }

    public bool Equals(Label label)
    {
        return this.Position == label.GetHashCode();
    }

    public override bool Equals(object? obj)
    {
        return obj switch
        {
            EmitterLabel emitLabel => Equals(emitLabel),
            Label label => Equals(label),
            _ => false,
        };
    }

    public override int GetHashCode()
    {
        return Hasher.Combine(Name, Position);
    }

    public void DeclareTo(CodeBuilder codeBuilder)
    {
        codeBuilder
            .Write($"0x{Position:X4} | {Name}:");
    }

    public override string ToString() => CodePart.ToDeclaration(this);
}