using Jay.Dumping;
using Jay.Dumping.Interpolated;

namespace Jay.Reflection.Dumping;

public sealed class ConstructorInfoDumper : Dumper<ConstructorInfo>
{
    protected override void DumpImpl(ref DumpStringHandler stringHandler, [DisallowNull] ConstructorInfo ctor, DumpFormat format)
    {
        stringHandler.Dump(ctor.DeclaringType!, format);
        stringHandler.Write(".ctor(");
        var parameters = ctor.GetParameters();
        stringHandler.DumpDelimited<ParameterInfo>(", ", parameters);
        stringHandler.Write(')');
    }
}