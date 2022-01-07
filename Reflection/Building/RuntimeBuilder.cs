using System.Globalization;
using System.Reflection.Emit;
using Jay.Text;

namespace Jay.Reflection;

public static class RuntimeBuilder
{
    private static int _counter = 0;

    public static AssemblyBuilder AssemblyBuilder { get; }
    public static ModuleBuilder ModuleBuilder { get; }

    static RuntimeBuilder()
    {
        AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(new AssemblyName("Jay.Reflection.Building.Dynamic"), AssemblyBuilderAccess.Run);
        ModuleBuilder = AssemblyBuilder.DefineDynamicModule("RuntimeModuleBuilder");
    }

    //https://stackoverflow.com/questions/950616/what-characters-are-allowed-in-c-sharp-class-name
    private static bool IsValidNameFirstChar(char ch)
    {
        var category = char.GetUnicodeCategory(ch);
        return ch == '_' ||
               category == UnicodeCategory.UppercaseLetter ||
               category == UnicodeCategory.LowercaseLetter ||
               category == UnicodeCategory.TitlecaseLetter ||
               category == UnicodeCategory.ModifierLetter ||
               category == UnicodeCategory.OtherLetter;
    }

    private static bool IsValidNameChar(char ch)
    {
        var category = char.GetUnicodeCategory(ch);
        return category == UnicodeCategory.UppercaseLetter ||
               category == UnicodeCategory.LowercaseLetter ||
               category == UnicodeCategory.TitlecaseLetter ||
               category == UnicodeCategory.ModifierLetter ||
               category == UnicodeCategory.OtherLetter ||
               category == UnicodeCategory.LetterNumber ||
               category == UnicodeCategory.NonSpacingMark ||
               category == UnicodeCategory.SpacingCombiningMark ||
               category == UnicodeCategory.DecimalDigitNumber ||
               category == UnicodeCategory.ConnectorPunctuation ||
               category == UnicodeCategory.Format;
    }

    private static bool TryAppendName(string? name, TextBuilder builder)
    {
        if (name is null || name.Length == 0)
            return false;
        int start;
        char ch = name[0];
        if (IsValidNameFirstChar(ch))
        {
            builder.Append(ch);
            start = 1;
        }
        else
        {
            builder.Append('_');
            start = 0;
        }

        for (var i = start; i < name.Length; i++)
        {
            ch = name[i];
            if (IsValidNameChar(ch))
            {
                builder.Append(ch);
            }
        }
        return builder.Length > start;
    }

    public static string CreateMethodName(string? name, DelegateSig delegateSig)
    {
        using var builder = new TextBuilder();
        if (!TryAppendName(name, builder))
        {
            builder.Clear();
            if (delegateSig.IsAction)
            {
                builder.Write("Action_");
            }
            else
            {
                builder.Write("Func_");
            }
            var ctr = Interlocked.Increment(ref _counter);
            builder.Write(ctr);
        }
        return builder.ToString();
    }

    public static string CreateTypeName(string? name, TypeAttributes typeAttributes)
    {
        using var builder = new TextBuilder();
        if (!TryAppendName(name, builder))
        {
            builder.Clear();
            if (typeAttributes.HasAnyFlags(TypeAttributes.Interface))
            {
                builder.Write("Interface_");
            }
            else if (typeAttributes.HasAnyFlags(TypeAttributes.Abstract, TypeAttributes.AnsiClass, TypeAttributes.AutoClass, TypeAttributes.Class, TypeAttributes.UnicodeClass))
            {
                builder.Write("Class_");
            }
            else
            {
                builder.Write("Struct_");
            }
            var ctr = Interlocked.Increment(ref _counter);
            builder.Write(ctr);
        }
        return builder.ToString();
    }

    public static DynamicMethod CreateDynamicMethod(string? name,
                                                    DelegateSig delegateSig)
    {
        return new DynamicMethod(CreateMethodName(name, delegateSig),
            MethodAttributes.Public | MethodAttributes.Static,
            CallingConventions.Standard,
            delegateSig.ReturnType,
            delegateSig.ParameterTypes,
            ModuleBuilder,
            true);
    }

    public static DynamicMethod<TDelegate> CreateDynamicMethod<TDelegate>(string? name)
        where TDelegate : Delegate
    {
        return new DynamicMethod<TDelegate>(CreateDynamicMethod(name, DelegateSig.Of<TDelegate>()));
    }

    public static TDelegate CreateDelegate<TDelegate>(string? name, Action<DynamicMethod<TDelegate>> buildDelegate)
        where TDelegate : Delegate
    {
        var dm = CreateDynamicMethod<TDelegate>(name);
        buildDelegate(dm);
        return dm.CreateDelegate();
    }

    public static TypeBuilder DefineType(string? name, TypeAttributes typeAttributes)
    {
        return ModuleBuilder.DefineType(CreateTypeName(name, typeAttributes),
            typeAttributes, typeof(RuntimeBuilder));
    }
}