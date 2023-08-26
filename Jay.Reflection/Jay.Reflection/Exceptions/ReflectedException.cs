using System.Diagnostics;
using Jay.Reflection.Builders;
using Jay.Reflection.Searching;

namespace Jay.Reflection.Exceptions;

public /* static */ partial class ReflectedException
{
    private static readonly Action<Exception, string?> _setExceptionMessage;
    private static readonly Action<Exception, Exception?> _setExceptionInnerException;

    static ReflectedException()
    {
        var exMessageField = MemberSearch.One<Exception, FieldInfo>(
            new(
                "_message",
                Visibility.NonPublic | Visibility.Instance,
                typeof(string)));
        _setExceptionMessage = RuntimeBuilder.EmitDelegate<Action<Exception, string?>>(
            "Exception_set_Message",
            emitter =>
            {
                emitter.Emit(OpCodes.Ldarg_0)
                    .Emit(OpCodes.Ldarg_1)
                    .Emit(OpCodes.Stfld, exMessageField)
                    .Emit(OpCodes.Ret);
            });

        var exInnerExField = MemberSearch.One<Exception, FieldInfo>(
            new(
                "_innerException",
                Visibility.NonPublic | Visibility.Instance,
                typeof(Exception)));
        _setExceptionInnerException = RuntimeBuilder.EmitDelegate<Action<Exception, Exception?>>(
            "Exception_set_InnerException",
            emitter =>
            {
                emitter.Emit(OpCodes.Ldarg_0)
                    .Emit(OpCodes.Ldarg_1)
                    .Emit(OpCodes.Stfld, exInnerExField)
                    .Emit(OpCodes.Ret);
            });
    }

    public static TException Create<TException>(
        InterpolatedCodeBuilder message,
        params object?[] args)
        where TException : Exception
    {
        throw new NotImplementedException();
    }
}

/// <summary>
/// An <see cref="Exception"/> thrown during Jayflect operations
/// </summary>
public partial class ReflectedException : Exception
{
    public new string Message
    {
        get => base.Message;
        init => _setExceptionMessage(this, value);
    }

    public new Exception? InnerException
    {
        get => base.InnerException;
        init => _setExceptionInnerException(this, value);
    }

    public ReflectedException()
        : base()
    {
    }

    public ReflectedException(
        ref InterpolatedCodeBuilder message,
        Exception? innerException = null)
        : base(message.ToStringAndDispose(), innerException)
    {
        Debugger.Break();
    }

    public ReflectedException(
        string? message = null,
        Exception? innerException = null)
        : base(message, innerException)
    {
    }

    public override string ToString()
    {
        return CodeBuilder.Render(this);
    }
}