namespace Jay.Reflection.Emitting.Args;

partial record Argument
{
    partial record Local
    {
        public static implicit operator Local(EmitterLocal emitterLocal) => new Local(emitterLocal) { Type = emitterLocal.Type };
        
        public override void EmitLoad<TEmitter>(TEmitter emitter)
        {
            emitter.Ldloc(this.EmitterLocal);
        }
        public override void EmitLoadAddress<TEmitter>(TEmitter emitter)
        {
            emitter.Ldloca(this.EmitterLocal);
        }
        public override void EmitStore<TEmitter>(TEmitter emitter)
        {
            emitter.Stloc(this.EmitterLocal);
        }

        public override string ToString()
        {
            return this.EmitterLocal.ToString();
        }
    }
}