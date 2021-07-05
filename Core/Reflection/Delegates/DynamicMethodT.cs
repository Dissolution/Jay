using System;
using System.Reflection;
using System.Reflection.Emit;
using Jay.Reflection.Emission;
using Jay.Reflection.Runtime;

namespace Jay.Reflection.Delegates
{
    public sealed class DynamicMethod<TDelegate> : MethodSignature
        where TDelegate : Delegate
    {
        private readonly DynamicMethod _dynamicMethod;

        /// <inheritdoc />
        public override Type OwnerType => typeof(DelegateBuilder);

        /// <inheritdoc />
        public override string Name => _dynamicMethod.Name;

        /// <inheritdoc />
        public override ParameterInfo[] Parameters => _dynamicMethod.GetParameters();

        /// <inheritdoc />
        public override Type ReturnType => _dynamicMethod.ReturnType;

        internal DynamicMethod(DynamicMethod dynamicMethod)
        {
            _dynamicMethod = dynamicMethod;
        }

        public ILGenerator GetILGenerator()
        {
            return _dynamicMethod.GetILGenerator();
        }
        
        public FluentILEmitter GetEmitter()
        {
            return new FluentILEmitter(new AttachedFluentILGenerator(_dynamicMethod.GetILGenerator()));
        }

        public AttachedFluentILGenerator GetGenerator()
        {
            return new AttachedFluentILGenerator(_dynamicMethod.GetILGenerator());
        }
        
        public TDelegate CreateDelegate()
        {
            return _dynamicMethod.CreateDelegate<TDelegate>();
        }
    }
}