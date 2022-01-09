using System.Reflection.Emit;
using Jay.Reflection.Adapting;
using Jay.Reflection.Emission;
using Jay.Text;

namespace Jay.Reflection;

public static class PropertyInfoExtensions
{
    public static MethodInfo? GetGetter(this PropertyInfo? propertyInfo)
    {
        return propertyInfo?.GetGetMethod(false) ??
               propertyInfo?.GetGetMethod(true);
    }
        
    public static MethodInfo? GetSetter(this PropertyInfo? propertyInfo)
    {
        return propertyInfo?.GetSetMethod(false) ??
               propertyInfo?.GetSetMethod(true);
    }
        
    public static Visibility Visibility(this PropertyInfo? propertyInfo)
    {
        Visibility visibility = Reflection.Visibility.None;
        if (propertyInfo is null)
            return visibility;
        visibility |= propertyInfo.GetGetter().Visibility();
        visibility |= propertyInfo.GetSetter().Visibility();
        return visibility;
    }

    public static bool IsStatic(this PropertyInfo? propertyInfo)
    {
        if (propertyInfo is null)
            return false;
        return propertyInfo.GetGetter().IsStatic() ||
               propertyInfo.GetSetter().IsStatic();
    }

    private static string GetBackingFieldName(PropertyInfo property) => $"<{property.Name}>k__BackingField";

    public static FieldInfo? GetBackingField(this PropertyInfo? propertyInfo)
    {
        if (propertyInfo is null) return null;
        var owner = propertyInfo.DeclaringType;
        if (owner is null) return null;
        var flags = BindingFlags.NonPublic;
        flags |= propertyInfo.IsStatic() ? BindingFlags.Static : BindingFlags.Instance;
        var field = owner.GetField(GetBackingFieldName(propertyInfo), flags);
        if (field is null)
        {
            var getter = propertyInfo.GetGetter();
            if (getter is not null)
            {
                field = getter.GetInstructions()
                              .Where(inst =>
                                  inst.OpCode == OpCodes.Ldfld || inst.OpCode == OpCodes.Ldflda ||
                                  inst.OpCode == OpCodes.Ldsfld || inst.OpCode == OpCodes.Ldsflda)
                              .SelectWhere((Instruction inst, out FieldInfo fld) =>
                              {
                                  if (inst.Arg.Is(out fld) &&
                                      fld.DeclaringType == owner &&
                                      fld.FieldType == propertyInfo.PropertyType)
                                  {
                                      return true;
                                  }

                                  fld = null;
                                  return false;
                              })
                              .OrderBy(fld => Levenshtein.Calculate(fld.Name, propertyInfo.Name))
                              .FirstOrDefault();
            }
        }

        if (field is null)
        {
            var setter = propertyInfo.GetSetter();
            if (setter is not null)
            {
                field = setter.GetInstructions()
                              .Where(inst => inst.OpCode == OpCodes.Stfld || inst.OpCode == OpCodes.Stsfld)
                              .SelectWhere((Instruction inst, out FieldInfo fld) =>
                              {
                                  if (inst.Arg.Is(out fld) &&
                                      fld.DeclaringType == owner &&
                                      fld.FieldType == propertyInfo.PropertyType)
                                  {
                                      return true;
                                  }

                                  fld = null;
                                  return false;
                              })
                              .OrderBy(fld => Levenshtein.Calculate(fld.Name, propertyInfo.Name))
                              .FirstOrDefault();
            }
        }

        return field;
    }

    public static StaticGetter<TValue> CreateStaticGetter<TValue>(this PropertyInfo property)
    {
        ArgumentNullException.ThrowIfNull(property);
        var getter = GetGetter(property);
        Validation.IsStatic(getter);
        return RuntimeBuilder.CreateDelegate<StaticGetter<TValue>>(
            $"get_{property.OwnerType()}.{property.Name}", method =>
            {
                method.Emitter
                      .Call(getter)
                      .Cast(getter.ReturnType, typeof(TValue))
                      .Ret();
            });
    }

    public static StructGetter<TStruct, TValue> CreateStructGetter<TStruct, TValue>(this PropertyInfo property)
        where TStruct : struct
    {
        ArgumentNullException.ThrowIfNull(property);
        var result = property.TryGetInstanceType(out var instanceType);
        result.ThrowIfFailed();
        Validation.IsValue(instanceType);
        var getter = GetGetter(property);
        Validation.IsInstance(getter);
        return RuntimeBuilder.CreateDelegate<StructGetter<TStruct, TValue>>(
            $"get_{instanceType}_{property.Name}", method =>
            {
                method.Emitter
                      .LoadAs(method.Parameters[0], instanceType)
                      .Call(getter)
                      .Cast(getter.ReturnType, typeof(TValue))
                      .Ret();
            });
    }

    public static ClassGetter<TClass, TValue> CreateClassGetter<TClass, TValue>(this PropertyInfo property)
        where TClass : class
    {
        ArgumentNullException.ThrowIfNull(property);
        var result = property.TryGetInstanceType(out var instanceType);
        result.ThrowIfFailed();
        Validation.IsClass(instanceType, nameof(property));
        var getter = GetGetter(property);
        Validation.IsInstance(getter);
        return RuntimeBuilder.CreateDelegate<ClassGetter<TClass, TValue>>(
            $"get_{instanceType}_{property.Name}", method =>
            {
                method.Emitter
                      .LoadAs(method.Parameters[0], instanceType)
                      .Call(getter)
                      .Cast(getter.ReturnType, typeof(TValue))
                      .Ret();
            });
    }

    public static TValue? GetValue<TStruct, TValue>(this PropertyInfo property, ref TStruct instance)
        where TStruct : struct
    {
        var getter = DelegateMemberCache.Instance
                                        .GetOrAdd(property, CreateStructGetter<TStruct, TValue>);
        return getter(ref instance);
    }

    public static TValue? GetValue<TClass, TValue>(this PropertyInfo property,
                                                   TClass? instance)
        where TClass : class
    {
        var getter = DelegateMemberCache.Instance
            .GetOrAdd(property, CreateClassGetter<TClass, TValue>);
        return getter(instance);
    }

    public static TValue? GetStaticValue<TValue>(this PropertyInfo property)
    {
        var getter = DelegateMemberCache.Instance
            .GetOrAdd(property, CreateStaticGetter<TValue>);
        return getter();
    }



    public static StaticSetter<TValue> CreateStaticSetter<TValue>(this PropertyInfo property)
    {
        ArgumentNullException.ThrowIfNull(property);
        var setter = GetSetter(property);
        Validation.IsStatic(setter);
        return RuntimeBuilder.CreateDelegate<StaticSetter<TValue>>(
            $"set_{property.OwnerType()}.{property.Name}", method =>
            {
                method.Emitter
                      .LoadAs(method.Parameters[0], property.PropertyType)
                      .Call(setter)
                      .Ret();
            });
    }

    public static StructSetter<TStruct, TValue> CreateStructSetter<TStruct, TValue>(this PropertyInfo property)
        where TStruct : struct
    {
        ArgumentNullException.ThrowIfNull(property);
        var result = property.TryGetInstanceType(out var instanceType);
        result.ThrowIfFailed();
        Validation.IsValue(instanceType);
        var setter = GetSetter(property);
        Validation.IsInstance(setter);
        return RuntimeBuilder.CreateDelegate<StructSetter<TStruct, TValue>>(
            $"set_{instanceType}_{property.Name}", method =>
            {
                method.Emitter
                      .LoadAs(method.Parameters[0], instanceType)
                      .LoadAs(method.Parameters[1], property.PropertyType)
                      .Call(setter)
                      .Ret();
            });
    }

    public static ClassSetter<TClass, TValue> CreateClassSetter<TClass, TValue>(this PropertyInfo property)
        where TClass : class
    {
        ArgumentNullException.ThrowIfNull(property);
        var result = property.TryGetInstanceType(out var instanceType);
        result.ThrowIfFailed();
        Validation.IsClass(instanceType, nameof(property));
        var setter = GetSetter(property);
        Validation.IsInstance(setter);
        return RuntimeBuilder.CreateDelegate<ClassSetter<TClass, TValue>>(
            $"set_{instanceType}_{property.Name}", method =>
            {
                method.Emitter
                      .LoadAs(method.Parameters[0], instanceType)
                      .LoadAs(method.Parameters[1], property.PropertyType)
                      .Call(setter)
                      .Ret();
            });
    }

    public static void SetValue<TStruct, TValue>(this PropertyInfo property,
                                                 ref TStruct instance,
                                                 TValue? value)
        where TStruct : struct
    {
        var setter = DelegateMemberCache.Instance
                                        .GetOrAdd(property, CreateStructSetter<TStruct, TValue>);
        setter(ref instance, value);
    }


    public static void SetValue<TClass, TValue>(this PropertyInfo property,
                                                   TClass? instance,
                                                   TValue? value)
        where TClass : class
    {
        var setter = DelegateMemberCache.Instance
                                        .GetOrAdd(property, CreateClassSetter<TClass, TValue>);
        setter(instance, value);
    }

    public static void SetStaticValue<TValue>(this PropertyInfo property,
                                        TValue? value)
    {
        var setter = DelegateMemberCache.Instance
                                        .GetOrAdd(property, CreateStaticSetter<TValue>);
        setter(value);
    }
}