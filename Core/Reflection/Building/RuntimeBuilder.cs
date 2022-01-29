using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Reflection.Emit;
using Jay.Dumping;
using Jay.Reflection.Search;
using Jay.Text;

namespace Jay.Reflection.Building;

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

    public static bool IsValidMemberName(string? name)
    {
        if (name is null) return false;
        var len = name.Length;
        if (len == 0) return false;
        char ch = name[0];
        if (!IsValidNameFirstChar(ch)) return false;
        for (var i = 1; i < len; i++)
        {
            if (!IsValidNameChar(ch)) return false;
        }
        return true;
    }

    internal static bool TryAppendName(string? name, TextBuilder builder)
    {
        if (name is null || name.Length == 0)
            return false;
        int start;
        char ch = name[0];
        if (IsValidNameFirstChar(ch))
        {
            builder.Write(ch);
            start = 1;
        }
        else
        {
            builder.Write('_');
            start = 0;
        }

        for (var i = start; i < name.Length; i++)
        {
            ch = name[i];
            if (IsValidNameChar(ch))
            {
                builder.Write(ch);
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

    public static string FixedMemberName(string? name, MemberTypes memberType)
    {
        using var text = new TextBuilder();
        if (!TryAppendName(name, text))
        {
            var ctr = Interlocked.Increment(ref _counter);
            text.Clear()
                .Append(memberType)
                .Append('_')
                .Append(ctr);
        }
        return text.ToString();
    }

    public static string FieldName(string propertyName)
    {
        return string.Create(propertyName.Length + 1, propertyName, (span, name) =>
        {
            span[0] = '_';
            span[1] = char.ToLower(name[0]);
            for (var i = 1; i < name.Length; i++)
            {
                span[i + 1] = name[i];
            }
        });
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

    public static CustomAttributeBuilder GetCustomAttributeBuilder<TAttribute>()
        where TAttribute : Attribute, new()
    {
        var ctor = typeof(TAttribute).GetConstructor(Reflect.InstanceFlags, Type.EmptyTypes);
        if (ctor is null)
            Dump.ThrowException<InvalidOperationException>($"Cannot find an empty {typeof(TAttribute)} constructor.");
        return new CustomAttributeBuilder(ctor, Array.Empty<object>());
    }

    public static CustomAttributeBuilder GetCustomAttributeBuilder<TAttribute>(params object?[] ctorArgs)
        where TAttribute : Attribute
    {
        var ctor = MemberSearch.FindBestConstructor(typeof(TAttribute), Reflect.InstanceFlags, ctorArgs);
        if (ctor is null)
            Dump.ThrowException<InvalidOperationException>($"Cannot find a {typeof(TAttribute)} constructor that matches {ctorArgs}");
        return new CustomAttributeBuilder(ctor, ctorArgs);
    }

    public static CustomAttributeBuilder GetCustomAttributeBuilder(Type attributeType, params object?[] ctorArgs)
    {
        if (!attributeType.Implements<Attribute>())
            Dump.ThrowException<ArgumentException>($"{attributeType} is not an Attribute");
        var ctor = MemberSearch.FindBestConstructor(attributeType, Reflect.InstanceFlags, ctorArgs);
        if (ctor is null)
            Dump.ThrowException<InvalidOperationException>($"Cannot find a {attributeType} constructor that matches {ctorArgs}");
        return new CustomAttributeBuilder(ctor, ctorArgs);
    }
}