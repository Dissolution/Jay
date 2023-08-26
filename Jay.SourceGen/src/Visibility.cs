namespace Jay.CodeGen;

[EnumToCode(Naming.LowerCase, Delimiter = " ")]
[Flags]
public enum Visibility
{
    None = 0,
        
    Private = 1<<0,
    Protected = 1<<1,
    Internal = 1<<2,
    Public = 1 <<3,
    
    Instance = 1 << 4,
    Static = 1<<5,
}

public static class VisibilityExtensions
{
    public static Visibility ToVisibility(this BindingFlags bindingFlags)
    {
        Visibility vis = default;
        if (bindingFlags.HasFlag<BindingFlags>(BindingFlags.Instance))
            vis |= Visibility.Instance;
        if (bindingFlags.HasFlag<BindingFlags>(BindingFlags.Static))
            vis |= Visibility.Static;
        if (bindingFlags.HasFlag<BindingFlags>(BindingFlags.NonPublic))
            vis |= (Visibility.Private | Visibility.Protected | Visibility.Internal);
        if (bindingFlags.HasFlag<BindingFlags>(BindingFlags.Public))
            vis |= Visibility.Public;
        return vis;
    }

    public static Visibility ToVisibility(this Accessibility accessibility)
    {
        switch (accessibility)
        {
            case Accessibility.Private:
                return Visibility.Private;
            case Accessibility.ProtectedAndInternal:
                return (Visibility.Protected | Visibility.Internal);
            case Accessibility.Protected:
                return Visibility.Protected;
            case Accessibility.Internal:
                return Visibility.Internal;
            case Accessibility.ProtectedOrInternal:
                return (Visibility.Protected | Visibility.Internal);
            case Accessibility.Public:
                return Visibility.Public;
            case Accessibility.NotApplicable:
            default:
                return Visibility.None;
        }
    }

    public static Visibility GetVisibility(this MemberInfo? memberInfo)
    {
        Visibility vis = Visibility.None;
        switch (memberInfo)
        {
            case null:
            {
                // None
                return vis;
            }
            case Type type:
            {
                if (type.IsAbstract && type.IsSealed)
                {
                    vis |= Visibility.Static;
                }
                else
                {
                    vis |= Visibility.Instance;
                }
                if (type.IsPublic)
                    vis |= Visibility.Public;
                if (type.IsNotPublic)
                    vis |= (Visibility.Private | Visibility.Protected | Visibility.Internal);
                if (type.IsNested)
                {
                    if (type.IsNestedPrivate)
                        vis |= Visibility.Private;
                    if (type.IsNestedFamily)
                        vis |= Visibility.Protected;
                    if (type.IsNestedAssembly)
                        vis |= Visibility.Internal;
                    if (type.IsNestedPublic)
                        vis |= Visibility.Public;
                    if (type.IsNestedFamORAssem || type.IsNestedFamORAssem)
                        vis |= (Visibility.Protected | Visibility.Internal);
                }
                return vis;
            }
            case FieldInfo fieldInfo:
            {
                if (fieldInfo.IsPrivate)
                    vis |= Visibility.Private;
                if (fieldInfo.IsFamily)
                    vis |= Visibility.Protected;
                if (fieldInfo.IsAssembly)
                    vis |= Visibility.Internal;
                if (fieldInfo.IsPublic)
                    vis |= Visibility.Public;
                if (fieldInfo.IsFamilyAndAssembly || fieldInfo.IsFamilyOrAssembly)
                    vis |= (Visibility.Protected | Visibility.Internal);
                return vis;
            }
            case MethodBase methodBase:
            {
                if (methodBase.IsPrivate)
                    vis |= Visibility.Private;
                if (methodBase.IsFamily)
                    vis |= Visibility.Protected;
                if (methodBase.IsAssembly)
                    vis |= Visibility.Internal;
                if (methodBase.IsPublic)
                    vis |= Visibility.Public;
                if (methodBase.IsFamilyAndAssembly || methodBase.IsFamilyOrAssembly)
                    vis |= (Visibility.Protected | Visibility.Internal);
                return vis;
            }
            case PropertyInfo propertyInfo:
            {
                vis |= GetVisibility(propertyInfo.GetGetMethod(false) ??
                    propertyInfo.GetGetMethod(true));
                vis |= GetVisibility(propertyInfo.GetSetMethod(false) ??
                    propertyInfo.GetSetMethod(true));
                return vis;
            }
            case EventInfo eventInfo:
            {
                vis |= GetVisibility(eventInfo.GetAddMethod(false) ??
                    eventInfo.GetAddMethod(true));
                vis |= GetVisibility(eventInfo.GetRemoveMethod(false) ??
                    eventInfo.GetRemoveMethod(true));
                vis |= GetVisibility(eventInfo.GetRaiseMethod(false) ??
                    eventInfo.GetRaiseMethod(true));
                return vis;
            }
            default:
                throw new NotImplementedException();
        }
    }

    public static Visibility GetVisibility(this ISymbol? symbol)
    {
        Visibility vis = Visibility.None;
        if (symbol is null) return vis;
        if (symbol.IsStatic)
            vis |= Visibility.Static;
        else
            vis |= Visibility.Instance;
        vis |= ToVisibility(symbol.DeclaredAccessibility);
        return vis;
    }
}