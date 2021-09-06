using System;
using System.Reflection;
using System.Reflection.Emit;

namespace Jay.Reflection.Emission
{
    public interface IWriter<out TBuilder>
        where TBuilder : IFluentILStream<TBuilder>
    {
        TBuilder WriteLine(string? text);
        TBuilder WriteLine(FieldInfo field);
        TBuilder WriteLine(LocalBuilder local);

        TBuilder DumpValue<T>();
        TBuilder DumpValue(Type valueType);

        TBuilder WriteValue(Type valueType);
    }
}