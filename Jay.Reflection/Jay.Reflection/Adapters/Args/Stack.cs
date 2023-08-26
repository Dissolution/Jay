namespace Jay.Reflection.Adapters.Args;

partial record Arg
{
    partial record Stack
    {
        public static implicit operator Stack(Type? type) => new Stack() { Type = type ?? typeof(void) };

        public override void EmitLoad<TEmitter>(TEmitter emitter)
        {
            // already loaded
        }
        
        public override void EmitLoadAddress<TEmitter>(TEmitter emitter)
        {
            // have to store it in a local in order to get an address
            emitter
                .DeclareLocal(this.RootType, out var localVarForLoadAddress)
                .Stloc(localVarForLoadAddress)
                .Ldloca(localVarForLoadAddress);
        }
        
        public override void EmitStore<TEmitter>(TEmitter emitter)
        {
            // already on the stack
        }
        
        public override void WriteCodeTo(CodeBuilder codeBuilder)
        {
            MemberInfoToCode.WriteCodeTo(this.Type, codeBuilder);
        }
    }
}
