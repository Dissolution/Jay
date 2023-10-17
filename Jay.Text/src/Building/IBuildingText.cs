namespace Jay.Text.Building;

/// <summary>
/// This implementation is building a <see cref="string"/>
/// </summary>
public interface IBuildingText : IDisposable
{
    /// <summary>
    /// Gets the final <see cref="string"/>, disposes this instance, and returns that <see cref="string"/> 
    /// </summary>
    string ToStringAndDispose();
}