﻿namespace Jay.Reflection.Adapters.Args;

partial record Arg
{
    partial record Parameter
    {
        public static implicit operator Parameter(ParameterInfo parameter) => new Parameter(parameter)
            { Type = parameter.ParameterType };

        public override void EmitLoad<TEmitter>(TEmitter emitter)
        {
            emitter.Ldarg(this.ParameterInfo.Position);
        }
        public override void EmitLoadAddress<TEmitter>(TEmitter emitter)
        {
            emitter.Ldarga(this.ParameterInfo.Position);
        }
        public override void EmitStore<TEmitter>(TEmitter emitter)
        {
            emitter.Starg(this.ParameterInfo.Position);
        }

        public override void WriteCodeTo(CodeBuilder codeBuilder)
        {
            MemberInfoToCode.WriteCodeTo(this.ParameterInfo, codeBuilder);
        }
    }
}