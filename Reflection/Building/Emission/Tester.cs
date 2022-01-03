


// ReSharper disable UnusedMember.Global
// ReSharper disable MemberCanBePrivate.Global
// ReSharper disable IdentifierTypo
// ReSharper disable CommentTypo

namespace Jay.Reflection.Emission;

/* NOTES
 * Emit(OpCodes.Calli, ?) should always throw as they are supposed to use Calli()
 *
 *
 *  <see langword="null"/>
 *
 *
 * https://github.com/Sergio0694/BinaryPack/blob/master/src/BinaryPack/Extensions/System.Reflection.Emit/ILGeneratorExtensions.cs
 */

// public interface ICatchBlockBuilder<T>
//     where T : IFluentGenEmitter<T>
// {
//     ICatchBlockBuilder<T> Catch(Type exceptionType, Action<T> catchBlock);
//
//     ICatchBlockBuilder<T> Catch<TException>(Action<T> catchBlock)
//         where TException : Exception;
// }

public static class Tester
{
    static Tester()
    {
        //var emitter = new OpCodeEmitter();
        IILGeneratorFluentEmitter emitter = default!;


        ClassGetter<object, object> getter = new();

    }
}

// public class OpCodeEmitter : IOpCodeEmitter<OpCodeEmitter>
// {
//     public InstructionStream<Instruction> Instructions { get; }
//
//     public OpCodeEmitter Emit(OpCode opCode)
//     {
//         throw new NotImplementedException();
//     }
//
//     public OpCodeEmitter Emit(OpCode opCode, byte value)
//     {
//         throw new NotImplementedException();
//     }
//
//     public OpCodeEmitter Emit(OpCode opCode, sbyte value)
//     {
//         throw new NotImplementedException();
//     }
//
//     public OpCodeEmitter Emit(OpCode opCode, short value)
//     {
//         throw new NotImplementedException();
//     }
//
//     public OpCodeEmitter Emit(OpCode opCode, int value)
//     {
//         throw new NotImplementedException();
//     }
//
//     public OpCodeEmitter Emit(OpCode opCode, long value)
//     {
//         throw new NotImplementedException();
//     }
//
//     public OpCodeEmitter Emit(OpCode opCode, float value)
//     {
//         throw new NotImplementedException();
//     }
//
//     public OpCodeEmitter Emit(OpCode opCode, double value)
//     {
//         throw new NotImplementedException();
//     }
//
//     public OpCodeEmitter Emit(OpCode opCode, string str)
//     {
//         throw new NotImplementedException();
//     }
//
//     public OpCodeEmitter Emit(OpCode opCode, FieldInfo field)
//     {
//         throw new NotImplementedException();
//     }
//
//     public OpCodeEmitter Emit(OpCode opCode, MethodInfo method)
//     {
//         throw new NotImplementedException();
//     }
//
//     public OpCodeEmitter Emit(OpCode opCode, ConstructorInfo ctor)
//     {
//         throw new NotImplementedException();
//     }
//
//     public OpCodeEmitter Emit(OpCode opCode, SignatureHelper signature)
//     {
//         throw new NotImplementedException();
//     }
//
//     public OpCodeEmitter Emit(OpCode opCode, Type type)
//     {
//         throw new NotImplementedException();
//     }
//
//     public OpCodeEmitter Emit(OpCode opCode, LocalBuilder local)
//     {
//         throw new NotImplementedException();
//     }
//
//     public OpCodeEmitter Emit(OpCode opCode, Label label)
//     {
//         throw new NotImplementedException();
//     }
//
//     public OpCodeEmitter Emit(OpCode opCode, params Label[] labels)
//     {
//         throw new NotImplementedException();
//     }
// }