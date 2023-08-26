using System.Runtime.InteropServices;
using Jay.Reflection.Validation;

namespace Jay.Reflection.Emitting;

public sealed class GeneratorEmission : Emission
{
    public static GeneratorEmission BeginCatchBlock(Type exceptionType)
    {
        Validate.IsExceptionType(exceptionType);
        return new(exceptionType);
    }
    public static GeneratorEmission BeginExceptFilterBlock() => new();
    public static GeneratorEmission BeginExceptionBlock(EmitLabel emitLabel) => new(emitLabel);
    public static GeneratorEmission EndExceptionBlock() => new();
    public static GeneratorEmission BeginFaultBlock() => new();
    public static GeneratorEmission BeginFinallyBlock() => new();
    public static GeneratorEmission BeginScope() => new();
    public static GeneratorEmission EndScope() => new();
    public static GeneratorEmission UsingNamespace(string nameSpace)
    {
        // A name must contain only alphabetic characters, decimal digits, and underscores,
        // and must begin with an alphabetic character or underscore (_).
        int nsLen = nameSpace.Length;
        if (nsLen == 0)
            throw new ArgumentException("A namespace must begin with an alphabetic character or underscore", nameof(nameSpace));
        char ch = nameSpace[0];
        if (!ch.IsAsciiLetter() && ch != '_')
            throw new ArgumentException("A namespace must begin with an alphabetic character or underscore", nameof(nameSpace));
        for (var i = 1; i < nsLen; i++)
        {
            ch = nameSpace[i];
            if (!ch.IsAsciiLetterOrDigit() && ch != '_')
                throw new ArgumentException("A namespace must contain only alphabetic characters, digits, or underscore", nameof(nameSpace));
        }
        // it's valid!
        return new(nameSpace);
    }
    public static GeneratorEmission DeclareLocal(EmitLocal emitLocal) => new(emitLocal);
    public static GeneratorEmission DefineLabel(EmitLabel emitLabel) => new(emitLabel);
    public static GeneratorEmission MarkLabel(EmitLabel emitLabel) => new(emitLabel);
    public static GeneratorEmission EmitCall(MethodInfo method, params Type[]? optionalParameterTypes)
    {
        return new(nameof(EmitCall), method, optionalParameterTypes);
    }
    public static GeneratorEmission EmitCalli(CallingConvention callingConvention, Type? returnType, Type[]? parameterTypes)
    {
        return new(nameof(EmitCalli), callingConvention, returnType, parameterTypes);
    }
    public static GeneratorEmission EmitCalli(CallingConventions callingConventions, Type? returnType, Type[]? parameterTypes,
        Type[]? optionalParameterTypes)
    {
        return new(nameof(EmitCalli), callingConventions, returnType, parameterTypes, optionalParameterTypes);
    }
    public static GeneratorEmission WriteLine(string text) => new(text);
    public static GeneratorEmission WriteLine(FieldInfo field) => new(field);
    public static GeneratorEmission WriteLine(EmitLocal emitLocal) => new(emitLocal);
    public static GeneratorEmission ThrowException(Type exceptionType) => new(exceptionType);

    public override int Size => 0;

    private GeneratorEmission([CallerMemberName] string? methodName = null)
        : base(methodName!, 0, Array.Empty<object?>()) { }
    
    private GeneratorEmission(object? arg, [CallerMemberName] string? methodName = null)
        : base(methodName!, 0, arg) { }

    private GeneratorEmission(string methodName, params object?[] arguments) 
        : base(methodName, 0, arguments)
    {
    }

    public override void WriteCodeTo(CodeBuilder codeBuilder)
    {
        codeBuilder.Append(Name);

        if (HasArgs)
        {
            codeBuilder.Append('(');
            codeBuilder.DelimitAppend(", ", Arguments);
            codeBuilder.Append(')');
        }
    }
}