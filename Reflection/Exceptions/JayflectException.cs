using Jay.Collections;
using Jay.Dumping.Interpolated;
using Jay.Reflection.Building;

namespace Jay.Reflection.Exceptions;

/// <summary>
/// An <see cref="Exception"/> thrown during Jayflect operations
/// </summary>
public class JayflectException : Exception
{
    private static readonly Action<Exception, string?> _setExceptionMessage;
    private static readonly Action<Exception, Exception?> _setExceptionInnerException;

    static JayflectException()
    {
        var exMessageField = Searching.MemberSearch.FindField<Exception>(new("_message", Visibility.NonPublic | Visibility.Instance, typeof(string)));
        _setExceptionMessage = RuntimeBuilder.CreateDelegate<Action<Exception, string?>>(
            "Exception_set_Message",
            emitter => emitter
                .Ldarg(0)
                .Ldarg(1)
                .Stfld(exMessageField)
                .Ret());

        var exInnerExField = Searching.MemberSearch.FindField<Exception>(new("_innerException", Visibility.NonPublic | Visibility.Instance, typeof(Exception)));
        _setExceptionInnerException = RuntimeBuilder.CreateDelegate<Action<Exception, Exception?>>(
            "Exception_set_InnerException",
            emitter => emitter
                .Ldarg(0)
                .Ldarg(1)
                .Stfld(exInnerExField)
                .Ret());
    }

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

    private DictionaryAdapter<string, object?>? _data = null;
    
    public new IDictionary<string, object?> Data => _data ??= new(base.Data);

    public JayflectException()
        : base()
    {

    }
    
    public JayflectException(
        ref DumpStringHandler message, 
        Exception? innerException = null)
        : base(message.ToStringAndDispose(), innerException)
    {

    }
    
    public JayflectException(
        string? message = null,
        Exception? innerException = null)
        : base(message, innerException)
    {

    }

    public override string ToString()
    {
        if (base.Data.Count == 0)
        {
            return base.ToString();
        }
        
        var dumper = new DumpStringHandler();
        dumper.Write(base.ToString());
        foreach (var pair in Data)
        {
            dumper.Write(Environment.NewLine);
            dumper.Write(pair.Key);
            dumper.Write(": ");
            dumper.Dump(pair.Value);
        }
        return dumper.ToStringAndDispose();
    }
}