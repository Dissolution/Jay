using System.Collections;
using Jay.Reflection.Text;
using Jay.Text.Building;

namespace Jay.Reflection.CodeBuilding;

public sealed class CodeBuilder : FluentIndentTextBuilder<CodeBuilder>
{
    public static CodeBuilder New => new();
    
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
            case Delegate del:
            {
                // Check for CBA compatability
                CBA? cba = Result.InvokeOrDefault(
                    () => Delegate.CreateDelegate(
                        typeof(CBA),
                        del.Target,
                        del.Method) as CBA);
                if (cba is not null)
                {
                    this.Format<CBA>(cba);
                }
                else
                {
                    base.Format<Delegate>(
                        del,
                        format,
                        provider);
                }
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

    public void Format<T>(T? value, Naming naming)
    {
        if (naming == Naming.Field)
        {
            this.Write('_');
            this.Format<T>(value, Naming.Pascal);
            return;
        }
        
        var written = this.GetWrittenSpan(cb => cb.Format<T>(value));
        switch (naming)
        {
            case Naming.Default:
                return;
            case Naming.Lower:
            {
                written.ForEach((ref char ch) => ch = char.ToLower(ch));
                return;
            }
            case Naming.Upper:
            {
                written.ForEach((ref char ch) => ch = char.ToUpper(ch));
                return;
            }
            case Naming.Camel:
            {
                if (written.Length > 0)
                {
                    written[0] = char.ToUpper(written[0]);
                }
                return;
            }
            case Naming.Pascal:
            {
                if (written.Length > 0)
                {
                    written[0] = char.ToLower(written[0]);
                }
                return;
            }
           default:
               return;
        }
    }

    public CodeBuilder Append<T>(T? value, Naming naming)
    {
        Format<T>(value, naming);
        return this;
    }
    
}