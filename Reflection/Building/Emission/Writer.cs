using System.Reflection.Emit;

namespace Jay.Reflection.Emission;

internal class Writer : IWriter<IILGeneratorEmitter>
{
    protected readonly GenEmitter _emitter;

    public Writer(GenEmitter emitter)
    {
        _emitter = emitter;
    }

    public IILGeneratorEmitter Line(string? text)
    {
        _emitter._ilGenerator.EmitWriteLine(text ?? string.Empty);
        var inst = new GeneratorInstruction(ILGeneratorMethod.WriteLine, text ?? "");
        _emitter.Instructions.AddLast(inst);
        return (_emitter as IILGeneratorEmitter)!;
    }

    public IILGeneratorEmitter Field(FieldInfo field)
    {
        _emitter._ilGenerator.EmitWriteLine(field);
        var inst = new GeneratorInstruction(ILGeneratorMethod.WriteLine, field);
        _emitter.Instructions.AddLast(inst);
        return (_emitter as IILGeneratorEmitter)!;
    }

    public IILGeneratorEmitter Local(LocalBuilder local)
    {
        _emitter._ilGenerator.EmitWriteLine(local);
        var inst = new GeneratorInstruction(ILGeneratorMethod.WriteLine, local);
        _emitter.Instructions.AddLast(inst);
        return (_emitter as IILGeneratorEmitter)!;
    }
}