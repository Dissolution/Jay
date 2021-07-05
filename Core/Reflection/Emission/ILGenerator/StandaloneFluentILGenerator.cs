namespace Jay.Reflection.Emission
{
    // public class StandaloneFluentILGenerator: FluentILGenerator<AttachedFluentILGenerator>
    // {
    //     private int _labelCount = 0;
    //     
    //     
    //     /// <inheritdoc />
    //     public override AttachedFluentILGenerator BeginExceptionBlock(out Label label)
    //     {
    //         label = EmitExtensions.CreateLabel(_labelCount++);
    //         return _emitter;
    //     }
    //
    //     /// <inheritdoc />
    //     public override AttachedFluentILGenerator DeclareLocal(Type localType, out LocalBuilder local)
    //     {
    //         local = EmitExtensions.CreateLocal(localType);
    //         return _emitter;
    //     }
    //
    //     /// <inheritdoc />
    //     public override AttachedFluentILGenerator DeclareLocal(Type localType, bool pinned, out LocalBuilder local)
    //     {
    //         local = EmitExtensions.CreateLocal(localType, pinned);
    //         return _emitter;
    //     }
    //
    //     /// <inheritdoc />
    //     public override AttachedFluentILGenerator DefineLabel(out Label label)
    //     {
    //         label = EmitExtensions.CreateLabel(_labelCount++);
    //         return _emitter;
    //     }
    // }
}