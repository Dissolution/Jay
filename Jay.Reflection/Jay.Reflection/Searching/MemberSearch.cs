using System.Diagnostics;

namespace Jay.Reflection.Searching;

public static class MemberSearch
{
    
    
    public static TMember One<TInstance, TMember>(string? memberName, NameMatchOptions nameMatchOptions = NameMatchOptions.Exact)
        where TMember : MemberInfo
    {
        var search = new MemberSearchOptions(memberName, nameMatchOptions);
        
        return One<TMember>(typeof(TInstance), search);
    }
    
    public static TMember One<TInstance, TMember>(MemberSearchOptions search)
        where TMember : MemberInfo
    {
        return One<TMember>(typeof(TInstance), search);
    }

    [return: NotNull]
    public static TMember One<TMember>([AllowNull, NotNull] Type? type, string? memberName,
        NameMatchOptions nameMatchOptions = NameMatchOptions.Exact)
        where TMember : MemberInfo
    {
        var search = new MemberSearchOptions(memberName, nameMatchOptions);
        return One<TMember>(type, search);
    }

    [return: NotNull]
    public static TMember One<TMember>([AllowNull, NotNull] Type? type, MemberSearchOptions search)
        where TMember : MemberInfo
    {
        if (type is null)
            throw new ArgumentNullException(nameof(type));
        var members = Enumerate<TMember>(type, search);
        using var e = members.GetEnumerator();
        if (!e.MoveNext())
            throw new Exception($"Could not find {type} {typeof(TMember)} '{search}'");
        TMember member = e.Current!;
        Debug.Assert(member is not null);
        if (e.MoveNext())
            throw new Exception($"Found too many matches for {type} {typeof(TMember)} '{search}'");
        return member!;
    }
    
    [return: NotNullIfNotNull(nameof(fallback))]
    public static TMember? OneOrDefault<TMember>(Type? type, MemberSearchOptions search, TMember? fallback = default)
        where TMember : MemberInfo
    {
        return Enumerate<TMember>(type, search).OneOrDefault(fallback);
    }
    
    public static IEnumerable<TMember> Enumerate<TMember>(Type? type, MemberSearchOptions search)
        where TMember : MemberInfo
    {
        if (type is null) return Array.Empty<TMember>();
        return type
            .GetMembers(search.Visibility.ToBindingFlags())
            .Where(member => search.Matches(member))
            .OfType<TMember>();
    }
    
    /*
    
    
    
    public static IEnumerable<MemberInfo> FindMembers(Type? type, MemberSearchOptions search)
    {
        if (type is null) return Array.Empty<MemberInfo>();
        return type
            .GetMembers(search.Visibility)
            .Where(member => search.Matches(member));
    }

    public static IEnumerable<MemberInfo> FindMembers<T>(MemberSearchOptions search)
    {
        return typeof(T)
            .GetMembers(search.Visibility)
            .Where(member => search.Matches(member));
    }

    public static TMember FindMember<TMember>(Expression<Func<TMember?>> memberExpression)
        where TMember : MemberInfo
    {
        Exception? exception = null;
        TMember? member = null;
        try
        {
            member = memberExpression.Compile().Invoke();
            if (member is not null)
                return member;
        }
        catch (Exception ex)
        {
            // Ignore, build error below
            exception = ex;
        }
        // Build our error
        var expressions = memberExpression
            .SelfAndDescendants()
            .SkipWhile(expr => expr is not MethodCallExpression)
            .ToList();

        // Method we were calling (likely Type.GetWhatever)
        var methodExpr = (expressions[0] as MethodCallExpression)!;
        var method = methodExpr.Method;
        var methodArgs = expressions.Skip(2).Select(expr =>
        {
            if (expr is ConstantExpression constantExpression)
                return constantExpression.Value;
            throw new NotImplementedException();

        }).ToList();
        var methodParams = method.GetParameters();
        if (methodArgs.Count != methodParams.Length)
            Debugger.Break();

        throw new ReflectedException($"Could not find {typeof(TMember)}: {method.OwnerType()}.{method.Name}({methodParams})", exception);
    }

    public static TMember FindMember<TMember>(Expression<Func<object?>> memberExpression)
        where TMember : MemberInfo
    {
        var member = memberExpression.ExtractMember<TMember>();
        if (member is not null)
            return member;
        var desc = memberExpression.SelfAndDescendants().ToList();
        var values = desc
            .SelectMany(expr => expr.ExtractValues<TMember>())
            .ToList();
        throw new NotImplementedException();
    }
    
    public static TMember FindMember<T, TMember>(Expression<Func<T, object?>> memberExpression)
        where TMember : MemberInfo
    {
        throw new NotImplementedException();
    }

    public static FieldInfo FindField(Type type, MemberSearchOptions fieldSearch)
    {
        return FindFields(type, fieldSearch).OneOrDefault() ??
               throw new ReflectedException($"Could not find on {type} a Field that matches '{fieldSearch}'");
    }

    public static FieldInfo FindField<T>(MemberSearchOptions fieldSearch) => FindField(typeof(T), fieldSearch);

    public static IEnumerable<FieldInfo> FindFields(Type type, MemberSearchOptions search)
    {
        return type.GetFields(search.Visibility)
            .Where(field => search.Matches(field));
    }

    public static PropertyInfo FindProperty(Type type, MemberSearchOptions propertySearch)
    {
        return FindProperties(type, propertySearch).OneOrDefault() ??
               throw new ReflectedException($"Could not find on {type} a Property that matches '{propertySearch}'");
    }

    public static PropertyInfo FindProperty<T>(MemberSearchOptions propertySearch) => FindProperty(typeof(T), propertySearch);

    public static IEnumerable<PropertyInfo> FindProperties(Type type, MemberSearchOptions search)
    {
        return type.GetProperties(search.Visibility)
            .Where(property => search.Matches(property));
    }

    public static EventInfo FindEvent(Type type, MemberSearchOptions eventSearch)
    {
        return FindEvents(type, eventSearch).OneOrDefault() ??
               throw new ReflectedException($"Could not find on {type} an Event that matches '{eventSearch}'");
    }

    public static EventInfo FindEvent<T>(MemberSearchOptions eventSearch) => FindEvent(typeof(T), eventSearch);

    public static IEnumerable<EventInfo> FindEvents(Type type, MemberSearchOptions search)
    {
        return type.GetEvents(search.Visibility)
            .Where(vent => search.Matches(vent));
    }

    public static ConstructorInfo? GetConstructor(Type instanceType, params object[] arguments)
    {
        return GetConstructor(instanceType, Array.ConvertAll(arguments, arg => arg.GetType()));
    }

    public static ConstructorInfo FindConstructor(Type instanceType, params object[] arguments)
    {
        return FindConstructor(instanceType, Array.ConvertAll(arguments, arg => arg.GetType()));
    }

    public static ConstructorInfo FindConstructor<TInstance>(params object[] arguments) => FindConstructor(typeof(TInstance), arguments);


    public static ConstructorInfo? GetConstructor<TInstance>(params Type[] argTypes)
    {
        ConstructorInfo? constructor = typeof(TInstance)
            .GetConstructors(Visibility.Instance)
            .FirstOrDefault(ctor => Easy.SeqEqual<Type>(ctor.GetParameterTypes(), argTypes));
        return constructor;
    }
    
    public static ConstructorInfo? GetConstructor(Type instanceType, params Type[] argTypes)
    {
        ConstructorInfo? constructor = instanceType
            .GetConstructors(Visibility.Instance)
            .FirstOrDefault(ctor => Easy.SeqEqual<Type>(ctor.GetParameterTypes(), argTypes));
        return constructor;
    }
    
    public static ConstructorInfo FindConstructor(Type instanceType, params Type[] argTypes)
    {
        ConstructorInfo? constructor = instanceType
            .GetConstructors(Visibility.Instance)
            .FirstOrDefault(ctor => Easy.SeqEqual<Type>(ctor.GetParameterTypes(), argTypes));
        if (constructor is not null)
            return constructor;
        throw new ReflectedException($"Could not find constructor that matched {instanceType}({argTypes})");
    }

    public static ConstructorInfo FindConstructor<TInstance>(params Type[] parameterTypes) => FindConstructor(typeof(TInstance), parameterTypes);

    public static ConstructorInfo FindConstructor(Type type, MemberSearchOptions ctorSearch)
    {
        return FindConstructors(type, ctorSearch).OneOrDefault() ??
               throw new ReflectedException($"Could not find on {type} a Constructor that matches '{ctorSearch}'");
    }
    
    public static IEnumerable<ConstructorInfo> FindConstructors(Type type, MemberSearchOptions search)
    {
        return type.GetConstructors(search.Visibility)
            .Where(ctor => search.Matches(ctor));
    }

    public static MethodInfo? GetMethod(Type type, MemberSearchOptions methodSearch)
    {
        return FindMethods(type, methodSearch).OneOrDefault();
    }
    
    public static MethodInfo FindMethod(Type type, MemberSearchOptions methodSearch)
    {
        return FindMethods(type, methodSearch).OneOrDefault() ??
               throw new ReflectedException($"Could not find on {type} a Method that matches '{methodSearch}'");
    }

    public static MethodInfo FindMethod<T>(MemberSearchOptions methodSearch) => FindMethod(typeof(T), methodSearch);
    
    public static IEnumerable<MethodInfo> FindMethods(Type type, MemberSearchOptions search)
    {
        return type.GetMethods(search.Visibility)
            .Where(ctor => search.Matches(ctor));
    }
    */
}