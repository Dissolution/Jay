using System.Reflection;
using System.Reflection.Emit;

namespace Jay.Reflection.Emission.Sandbox
{
    public interface IWriter<TEmitter>
        where TEmitter : IFluentILEmitter<TEmitter>
    {
        TEmitter WriteLine(string? text);
        TEmitter WriteLine(FieldInfo field);
        TEmitter WriteLine(LocalBuilder local);
    }
}