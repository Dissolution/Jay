﻿using Jay.Reflection.Emitting;


namespace Jay.Reflection.Adapters.Args;

partial record Arg
{
    partial record Field : IToCode
    {
        private void EmitLoadInstance<TEmitter>(TEmitter emitter)
            where TEmitter : FluentEmitter<TEmitter>
        {
            var hasInstance = this.FieldInfo.TryGetInstanceType(out var instanceType);
            if (hasInstance && this.Instance is not null)
            {
                emitter.TryEmitCast(this.Instance, instanceType).ThrowIfError();
            }
        }
        
        public override void EmitLoad<TEmitter>(TEmitter emitter)
        {
            EmitLoadInstance(emitter);
            emitter.Ldfld(this.FieldInfo);
        }
        public override void EmitLoadAddress<TEmitter>(TEmitter emitter)
        {
            EmitLoadInstance(emitter);
            emitter.Ldflda(this.FieldInfo);
        }
        public override void EmitStore<TEmitter>(TEmitter emitter)
        {
            EmitLoadInstance(emitter);
            emitter.Stfld(this.FieldInfo);
        }

        public override void WriteCodeTo(CodeBuilder codeBuilder)
        {
            MemberInfoToCode.WriteCodeTo(this.FieldInfo, codeBuilder);
        }
    }
}