using System.Reflection.Emit;
using System.Runtime.InteropServices;
using Jay.Text;

namespace Jay.Reflection.Emission;

public class GeneratorInstruction : Instruction, IEquatable<GeneratorInstruction>
{
    private static bool Equals(Type[] x, Type[] y)
    {
        var len = x.Length;
        if (y.Length != len) return false;
        for (int i = 0; i < len; i++)
        {
            if (x[i] != y[i]) return false;
        }
        return true;
    }

    private static bool Equals(object?[] x, object?[] y)
    {
        var len = x.Length;
        if (y.Length != len) return false;
        for (int i = 0; i < len; i++)
        {
            if (!OperandEquals(x[i], y[i])) return false;
        }
        return true;
    }

    private static bool OperandEquals(object? x, object? y)
    {
        if (x is null) return y is null;
        if (y is null) return false;
        if (x is Type xType)
            return y is Type yType && xType == yType;
        if (x is Label xLabel)
            return y is Label yLabel && xLabel == yLabel;
        if (x is string xString)
            return y is string yString && xString == yString;
        if (x is LocalBuilder xLocalBuilder)
            return y is LocalBuilder yLocalBuilder && Equals(xLocalBuilder, yLocalBuilder);
        if (x is bool xBool)
            return y is bool yBool && xBool == yBool;
        if (x is MethodInfo xMethodInfo)
            return y is MethodInfo yMethodInfo && xMethodInfo == yMethodInfo;
        if (x is Type[] xTypes)
            return y is Type[] yTypes && Equals(xTypes, yTypes);
        if (x is CallingConvention xCallingConvention)
            return y is CallingConvention yCallingConvention && xCallingConvention == yCallingConvention;
        if (x is FieldInfo xFieldInfo)
            return y is FieldInfo yFieldInfo && xFieldInfo == yFieldInfo;
        // ?
        if (x is object?[] xArray)
            return y is object?[] yArray && Equals(xArray, yArray);

        throw new NotImplementedException();
    }

    public ILGeneratorMethod Method { get; }
    public object? Arg { get; }

    public GeneratorInstruction(ILGeneratorMethod method)
    {
        this.Method = method;
        this.Arg = null;
    }

    public GeneratorInstruction(ILGeneratorMethod method, object? arg)
    {
        this.Method = method;
        this.Arg = arg;
    }

    public GeneratorInstruction(ILGeneratorMethod method, params object?[] args)
    {
        this.Method = method;
        this.Arg = args;
    }

    public bool Equals(GeneratorInstruction? generatorInstruction)
    {
        return generatorInstruction is not null &&
               generatorInstruction.Method == this.Method &&
               OperandEquals(generatorInstruction.Arg, this.Arg);
    }

    public override bool Equals(Instruction? instruction)
    {
        return instruction is GeneratorInstruction generatorInstruction &&
               Equals(generatorInstruction);
    }

    public override bool Equals(object? obj)
    {
        return obj is GeneratorInstruction generatorInstruction &&
               Equals(generatorInstruction);
    }

    public override int GetHashCode()
    {
        return HashCode.Combine(Method, Arg);
    }

    public override string ToString()
    {
        using var text = new TextBuilder();
        text.Append(Method).Append('(').Append(Arg).Append(')');
        return text.ToString();
    }
}