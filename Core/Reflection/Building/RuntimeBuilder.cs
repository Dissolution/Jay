﻿using System.Reflection;
using System.Reflection.Emit;
using Jay.Dumping;
using Jay.Reflection.Building.Emission;
using Jay.Reflection.Caching;
using Jay.Reflection.Search;

namespace Jay.Reflection.Building;

public static class RuntimeBuilder
{
    private static int _counter = 0;

    public static AssemblyBuilder AssemblyBuilder { get; }
    public static ModuleBuilder ModuleBuilder { get; }

    static RuntimeBuilder()
    {
        var assemblyName = new AssemblyName($"{typeof(RuntimeBuilder).Namespace}.Runtime");
        AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        ModuleBuilder = AssemblyBuilder.DefineDynamicModule("RuntimeModuleBuilder");
    }

    internal static DynamicMethod CreateDynamicMethod(MethodSig methodSig, string? name = null)
    {
        return new DynamicMethod(MemberNaming.CreateMemberName(name),
            MethodAttributes.Public | MethodAttributes.Static,
            CallingConventions.Standard,
            methodSig.ReturnType,
            methodSig.ParameterTypes,
            ModuleBuilder,
            true);
    }
   
    public static RuntimeMethod CreateMethod(MethodSig methodSig, string? name = null)
    {
        return new RuntimeMethod(CreateDynamicMethod(methodSig, name), methodSig);
    }

    public static Delegate CreateDelegate(Type delegateType, Action<RuntimeMethod> buildDelegate)
    {
        return CreateDelegate(delegateType, null, buildDelegate);
    }

    public static Delegate CreateDelegate(Type delegateType, string? name, Action<RuntimeMethod> buildDelegate)
    {
        if (!delegateType.Implements<Delegate>())
            throw new ArgumentException("Must be a delegate", nameof(delegateType));
        var runtimeMethod = CreateMethod(MethodSig.Of(delegateType), name);
        buildDelegate(runtimeMethod);
        return runtimeMethod.CreateDelegate(delegateType);
    }

    public static Delegate CreateDelegate(Type delegateType, Action<IILGeneratorEmitter> emitDelegate)
    {
        return CreateDelegate(delegateType, null, emitDelegate);
    }

    public static Delegate CreateDelegate(Type delegateType, string? name, Action<IILGeneratorEmitter> emitDelegate)
    {
        if (!delegateType.Implements<Delegate>())
            throw new ArgumentException("Must be a delegate", nameof(delegateType));
        var runtimeMethod = CreateMethod(MethodSig.Of(delegateType), name);
        emitDelegate(runtimeMethod.Emitter);
        return runtimeMethod.CreateDelegate(delegateType);
    }
    
    public static RuntimeMethod<TDelegate> CreateMethod<TDelegate>(string? name = null)
        where TDelegate : Delegate
    {
        return new RuntimeMethod<TDelegate>(CreateDynamicMethod(MethodSig.Of<TDelegate>(), name));
    }

    public static TDelegate CreateDelegate<TDelegate>(Action<RuntimeMethod<TDelegate>> buildDelegate)
        where TDelegate : Delegate
    {
        return CreateDelegate<TDelegate>(null, buildDelegate);
    }

    public static TDelegate CreateDelegate<TDelegate>(string? name, Action<RuntimeMethod<TDelegate>> buildDelegate)
        where TDelegate : Delegate
    {
        var runtimeMethod = CreateMethod<TDelegate>(name);
        buildDelegate(runtimeMethod);
        return runtimeMethod.CreateDelegate();
    }

    public static TypeBuilder DefineType(TypeAttributes typeAttributes, string? name = null)
    {
        return ModuleBuilder.DefineType(
            MemberNaming.CreateMemberName(name),
            typeAttributes, 
            typeof(RuntimeBuilder));
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