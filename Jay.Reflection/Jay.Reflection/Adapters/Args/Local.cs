using Jay.Reflection.Emitting;

namespace Jay.Reflection.Adapters.Args;

partial record Arg
{
    partial record Local
    {
        public static implicit operator Local(EmitLocal emitLocal) => new Local(emitLocal) { Type = emitLocal.Type };
        
        public override void EmitLoad<TEmitter>(TEmitter emitter)
        {
            emitter.Ldloc(this.EmitLocal);
        }
        public override void EmitLoadAddress<TEmitter>(TEmitter emitter)
        {
            emitter.Ldloca(this.EmitLocal);
        }
        public override void EmitStore<TEmitter>(TEmitter emitter)
        {
            emitter.Stloc(this.EmitLocal);
        }
        
        public override void WriteCodeTo(CodeBuilder codeBuilder)
        {
            this.EmitLocal.WriteCodeTo(codeBuilder);
        }
    }
}