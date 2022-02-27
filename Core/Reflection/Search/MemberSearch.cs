using System.Diagnostics;
using System.Linq.Expressions;
using System.Reflection;

namespace Jay.Reflection.Search;

public static class MemberSearch
{
    public static Result TryFind<TMember>(Expression memberExpression,
                                          [NotNullWhen(true)] out TMember? member)
        where TMember : MemberInfo
    {
        member = memberExpression.ExtractMember<TMember>();
        if (member is null)
        {
            Debugger.Break();
            return new MissingMemberException();
        }
        return true;
    }

    public static TMember? Find<TMember>(Expression memberExpression)
        where TMember : MemberInfo
    {
        return memberExpression.ExtractMember<TMember>();
    }

    public static bool HasDefaultConstructor(this Type type, [NotNullWhen(true)] out ConstructorInfo? ctor)
    {
        ctor = type.GetConstructor(Reflect.InstanceFlags, Type.EmptyTypes);
        return ctor is not null;
    }

    public static ConstructorInfo? FindBestConstructor(Type type,
                                                       BindingFlags flags,
                                                       params object?[]? args)
    {
        return FindBestConstructor(type, flags, MemberExactness.Exact, args);
    }

    public static ConstructorInfo? FindBestConstructor(Type type, 
                                                       BindingFlags flags, 
                                                       MemberExactness exactness, 
                                                       params object?[]? args)
    {
        Type?[]? argTypes;
        if (args is null)
        {
            argTypes = Type.EmptyTypes;
        }
        else
        {
            argTypes = new Type?[args.Length];
            for (var i = 0; i < args.Length; i++)
            {
                argTypes[i] = args[i]?.GetType();
            }
        }
        return FindBestConstructor(type, flags, exactness, argTypes);
    }


    [Flags]
    public enum MemberExactness
    {
        Exact = 0,
        CanIgnoreInputArgs = 1 << 0,
    }

    private static bool FastMatch(Type? argType, Type paramType)
    {
        if (argType is null)
        {
            return paramType.CanBeNull();
        }

        if (argType == paramType) return true;
        if (argType.Implements(paramType)) return true;
        if (argType == typeof(object) || paramType == typeof(object)) return true;
        return false;
    }

    public static ConstructorInfo? FindBestConstructor(Type type,
                                                       BindingFlags flags,
                                                       params Type?[] argTypes)
    {
        return FindBestConstructor(type, flags, MemberExactness.Exact, argTypes);
    }

    public static ConstructorInfo? FindBestConstructor(Type type,
                                                       BindingFlags flags,
                                                       MemberExactness exactness,
                                                           params Type?[] argTypes)
    {
        return type.GetConstructors(flags)
                   .OrderByDescending(ctor => ctor.GetParameters().Length)
                    .SelectWhere((ConstructorInfo ctor, out (int Distance, ConstructorInfo Constructor) measuredCtor) =>
            {
                int distance = 0;
                var parameters = ctor.GetParameters();
                if (parameters.Length < argTypes.Length &&
                    !exactness.HasFlag(MemberExactness.CanIgnoreInputArgs))
                {
                    measuredCtor = default;
                    return false;
                }

                for (var p = 0; p < parameters.Length; p++)
                {
                    var parameter = parameters[p];
                    if (p < argTypes.Length)
                    {
                        var argType = argTypes[p];
                        if (!FastMatch(argType, parameter.ParameterType))
                        {
                            // We do not match
                            measuredCtor = default;
                            return false;
                        }
                    }
                    else
                    {
                        // Can we just use a default value?
                        if (parameter.HasDefaultValue)
                        {
                            // Not great
                            distance++;
                        }
                        else
                        {
                            // This ctor does not work
                            measuredCtor = default;
                            return false;
                        }
                    }
                }

                // If we got here, we matched everything and have a distance
                measuredCtor = (distance, ctor);
                return true;
            })
            .OrderBy(tuple => tuple.Distance)
            .Select(tuple => tuple.Constructor)
            .FirstOrDefault();
    }
}