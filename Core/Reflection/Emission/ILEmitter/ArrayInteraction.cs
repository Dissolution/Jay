using System;

namespace Jay.Reflection.Emission
{
    internal sealed class ArrayInteraction : IArrayInteraction<IFluentILEmitter>
    {
        private readonly FluentILEmitter _ilEmitter;

        public ArrayInteraction(FluentILEmitter ilEmitter)
        {
            _ilEmitter = ilEmitter ?? throw new ArgumentNullException(nameof(ilEmitter));
        }

        /// <inheritdoc />
        public IFluentILEmitter GetLength()
        {
            _ilEmitter._ilGenerator.Ldlen();
            return _ilEmitter;
        }

        /// <inheritdoc />
        public IFluentILEmitter LoadElement(Type elementType)
        {
            if (elementType is null)
                throw new ArgumentNullException(nameof(elementType));
            _ilEmitter._ilGenerator.Ldelem(elementType);
            return _ilEmitter;
        }

        /// <inheritdoc />
        public IFluentILEmitter LoadElementAddress(Type elementType)
        {
            if (elementType is null)
                throw new ArgumentNullException(nameof(elementType));
            _ilEmitter._ilGenerator.Ldelema(elementType);
            return _ilEmitter;
        }

        /// <inheritdoc />
        public IFluentILEmitter StoreElement(Type elementType)
        {
            if (elementType is null)
                throw new ArgumentNullException(nameof(elementType));
            _ilEmitter._ilGenerator.Stelem(elementType);
            return _ilEmitter;
        }

        /// <inheritdoc />
        public IFluentILEmitter New(Type elementType)
        {
            if (elementType is null)
                throw new ArgumentNullException(nameof(elementType));
            _ilEmitter._ilGenerator.Newarr(elementType);
            return _ilEmitter;
        }
    }
}