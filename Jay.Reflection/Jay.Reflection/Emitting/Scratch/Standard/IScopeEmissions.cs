using Jay.Reflection.Emitting.Scratch.Simple;

namespace Jay.Reflection.Emitting.Scratch.Standard;

public interface IScopeEmissions<out Self>
    where Self : IEmitter<Self>
{
    /// <summary>
    /// Begins a lexical scope.
    /// </summary>
    /// <exception cref="NotSupportedException">An underlying <see cref="ILGenerator"/> belongs to a <see cref="DynamicMethod"/></exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.beginscope?view=netcore-3.0"/>
    Self BeginScope();

    /// <summary>
    /// Ends a lexical scope.
    /// </summary>
    /// <exception cref="NotSupportedException">An underlying <see cref="ILGenerator"/> belongs to a <see cref="DynamicMethod"/></exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.endscope?view=netcore-3.0"/>
    Self EndScope();

    /// <summary>
    /// Specifies the <see langword="namespace"/> to be used in evaluating locals and watches for the current active lexical scope.
    /// </summary>
    /// <param name="namespace">The namespace to be used in evaluating locals and watches for the current active lexical scope.</param>
    /// <exception cref="ArgumentNullException">If <paramref name="namespace"/> is <see langword="null"/> or has a Length of 0.</exception>
    /// <exception cref="NotSupportedException">An underlying <see cref="ILGenerator"/> belongs to a <see cref="DynamicMethod"/></exception>
    /// <see href="https://docs.microsoft.com/en-us/dotnet/api/system.reflection.emit.ilgenerator.usingnamespace?view=netcore-3.0"/>
    Self UsingNamespace(string @namespace);
}