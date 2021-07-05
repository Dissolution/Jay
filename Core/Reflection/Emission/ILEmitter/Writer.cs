using System;
using System.Reflection;
using System.Reflection.Emit;
using Jay.Debugging;
using Jay.Debugging.Dumping;

namespace Jay.Reflection.Emission
{
    internal class Writer : IWriter<IFluentILEmitter>
    {
        private readonly FluentILEmitter _ilEmitter;

        public Writer(FluentILEmitter ilEmitter)
        {
            _ilEmitter = ilEmitter ?? throw new ArgumentNullException(nameof(ilEmitter));
        }

        /// <inheritdoc />
        public IFluentILEmitter WriteLine(string? text)
        {
            _ilEmitter._ilGenerator.WriteLine(text);
            return _ilEmitter;
        }

        /// <inheritdoc />
        public IFluentILEmitter WriteLine(FieldInfo field)
        {
            _ilEmitter._ilGenerator.WriteLine(field);
            return _ilEmitter;
        }

        /// <inheritdoc />
        public IFluentILEmitter WriteLine(LocalBuilder local)
        {
            _ilEmitter._ilGenerator.WriteLine(local);
            return _ilEmitter;
        }

        /// <inheritdoc />
        public IFluentILEmitter DumpValue<T>() => DumpValue(typeof(T));

        public IFluentILEmitter DumpValue(Type valueType)
        {
            _ilEmitter._ilGenerator
                      .Dup()
                      .Box(valueType)
                      .Emitter
                      .Call(typeof(Dumper).GetMethod(nameof(Dumper.DumpObject)));
            return _ilEmitter;
        }
        
        public IFluentILEmitter WriteValue(Type valueType)
        {
            var toStringMethod = valueType.GetMethod(nameof(ToString),
                                                     BindingFlags.Public | BindingFlags.Instance,
                                                     null,
                                                     Type.EmptyTypes,
                                                     null)
                                          .ThrowIfNull();
            var consoleWriteLineStringMethod = typeof(Console).GetMethod(nameof(Console.WriteLine),
                                                                         Reflect.StaticFlags,
                                                                         null,
                                                                         new Type[1] {typeof(string)},
                                                                         null)
                                                              .ThrowIfNull();

            _ilEmitter._ilGenerator
                      .Dup()
                      .Call(toStringMethod)
                      .Call(consoleWriteLineStringMethod);
            return _ilEmitter;
        }
    }
}