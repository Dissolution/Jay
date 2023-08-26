using System.Collections;
using Jay.Text.Building;

namespace Jay.Reflection.CodeBuilding;

public delegate void CBA(CodeBuilder builder);

public delegate void CBA<in T>(CodeBuilder builder, T value);

public delegate void CBAI<in T>(CodeBuilder builder, T value, int index);

public sealed class CodeBuilder : FluentIndentTextBuilder<CodeBuilder>
{
    public static string Render(InterpolatedCodeBuilder code)
    {
        return code.ToStringAndDispose();
    }
    public static string Render<T>(T? value)
    {
        using var code = new CodeBuilder();
        code.Format<T>(value);
        return code.ToString();
    }
    
    internal bool CheckNull<T>([AllowNull, NotNullWhen(false)] T thing)
    {
        if (thing is null)
        {
            this.Append('(').Append(typeof(T)).Append(")null");
            return true;
        }
        return false;
    }
    
    public CodeBuilder()
    {
    }
    public CodeBuilder(int minCapacity) 
        : base(minCapacity)
    {
    }
    
    public override void Format<T>(T? value, string? format = null, IFormatProvider? provider = null) where T : default
    {
        switch (value)
        {
            case null:
            {
                return;
            }
            case CBA codeBuilderAction:
            {
                // Capture our original indents
                var originalIndents = _indents;
                // Replace them with a single indent based upon this position
                _indents = new Stack<string>(1);
                _indents.Push(GetCurrentPositionAsIndent());
                // perform the action
                codeBuilderAction(_builder);
                // restore the indents
                _indents = originalIndents;
                return;
            }
            case string str:
            {
                this.Write(str);
                return;
            }
            case MemberInfo memberInfo:
            {
                MemberInfoToCode.WriteCodeTo(memberInfo, this);
                return;
            }
            case ParameterInfo parameterInfo:
            {
                MemberInfoToCode.WriteCodeTo(parameterInfo, this);
                return;
            }
            case Exception exception:
            {
                ExceptionToCode.WriteCodeTo(exception, this);
                return;
            }
            // ReSharper disable once UseSwitchCasePatternVariable
            case IToCode:
            {
                ((IToCode)value).WriteCodeTo(this);
                return;
            }
            case IFormattable formattable:
            {
                base.Format(formattable, format, provider);
                return;
            }
            case IEnumerable enumerable:
            {
                format ??= ",";
                Delimit(
                    format,
                    enumerable.Cast<object?>(),
                    (w, v) => w.Append<object?>(v, format, provider)
                );
                return;
            }
            default:
            {
                base.Format<T>(value, format, provider);
                return;
            }
        }
    }

}