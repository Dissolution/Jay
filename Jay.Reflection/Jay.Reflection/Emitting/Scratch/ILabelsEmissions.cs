namespace Jay.Reflection.Emitting.Scratch;

public interface ILabelsEmissions<out Self>
    where Self : IEmitter<Self>
{
    /// <summary>
    /// Declares a new <see cref="Label"/>.
    /// </summary>
    /// <param name="label">Returns the new <see cref="Label"/> that can be used for branching.</param>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.definelabel"/>
    Self DefineLabel(
        out EmitterLabel label,
        [CallerArgumentExpression(nameof(label))]
        string? labelName = null);

    /// <summary>
    /// Marks the stream's current position with the given <see cref="Label"/>.
    /// </summary>
    /// <param name="label"> <see cref="Label"/> for which to set an index.</param>
    /// <exception cref="ArgumentException">If the <paramref name="label an invalid index.</exception>
    /// <exception cref="ArgumentException">If the <paramref name="label already been marked.</exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.marklabel"/>
    Self MarkLabel(EmitterLabel label);

    /// <summary>
    /// Implements a jump table.
    /// </summary>
    /// <param name="labels">The labels for the jumptable.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="labels"/> is <see langword="null"/> or empty.</exception>
    /// <see href="http://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.opcodes.switch"/>
    Self Switch(params EmitterLabel[] labels);
}