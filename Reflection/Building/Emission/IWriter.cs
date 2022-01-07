using System.Reflection.Emit;

namespace Jay.Reflection.Emission;

public interface IWriter<TEmitter>
{
    TEmitter Line(string? text);
    TEmitter Field(FieldInfo field);
    TEmitter Local(LocalBuilder local);
}