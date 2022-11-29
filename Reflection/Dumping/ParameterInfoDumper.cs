using Jay.Dumping;
using Jay.Dumping.Interpolated;
using Jay.Reflection.Extensions;

namespace Jay.Reflection.Dumping;

public sealed class ParameterInfoDumper : Dumper<ParameterInfo>
{
    protected override void DumpImpl(ref DumpStringHandler stringHandler, [DisallowNull] ParameterInfo parameter, DumpFormat format)
    {
        ParameterAccess access = parameter.GetAccess(out var parameterType);
        switch (access)
        {
            case ParameterAccess.In:
                stringHandler.Write("in ");
                break;
            case ParameterAccess.Ref:
                stringHandler.Write("ref ");
                break;
            case ParameterAccess.Out:
                stringHandler.Write("out ");
                break;
            case ParameterAccess.Default:
            default:
                break;
        }
        stringHandler.Dump(parameterType, format);
        stringHandler.Write(' ');
        stringHandler.Write(parameter.Name ?? "???");
        if (parameter.HasDefaultValue)
        {
            stringHandler.Write(" = ");
            stringHandler.Write(parameter.DefaultValue);
        }
    }
}