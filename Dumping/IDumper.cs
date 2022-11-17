using Jay.Dumping.Interpolated;

namespace Jay.Dumping;

/// <summary>
/// A utility that can dump values (turn them into a readable representation)
/// </summary>
public interface IDumper
{
    /// <summary>
    /// Can this <see cref="IDumper"/> instance dump values of the given <paramref name="type"/>?
    /// </summary>
    bool CanDump(Type type);
    
    /// <summary>
    /// Dumps an <see cref="object"/> to a <paramref name="dumpHandler"/> with optional <see cref="format"/>
    /// </summary>
    void DumpObjTo(ref DumpStringHandler dumpHandler, object? obj, DumpFormat format = default);
}