using System.Collections.Concurrent;
using Jay.Reflection.Emitting;
using Jay.Reflection.Exceptions;
using Jay.Reflection.Info;
using Jay.Reflection.Searching;
using Jay.Reflection.Text;
using Jay.Reflection.Validation;

namespace Jay.Reflection.Builders;

public static class RuntimeBuilder
{
    private static readonly ConcurrentDictionary<string, None> _memberNames = new(Environment.ProcessorCount, 0);
    
    public static AssemblyBuilder AssemblyBuilder { get; }

    public static ModuleBuilder ModuleBuilder { get; }

    static RuntimeBuilder()
    {
        var assemblyName = new AssemblyName($"{nameof(RuntimeBuilder)}_Assembly");
        AssemblyBuilder = AssemblyBuilder.DefineDynamicAssembly(assemblyName, AssemblyBuilderAccess.Run);
        ModuleBuilder = AssemblyBuilder.DefineDynamicModule($"{nameof(RuntimeBuilder)}_Module");
    }

    private static string RegisterName(string name)
    {
        int ctr = 1;
        while (!_memberNames.TryAdd(name, default))
        {
            name = $"{name}_{ctr}";
            ctr += 1;
        }
        return name;
    }
    
    private static string GetMemberName(MemberTypes memberType, InterpolatedCode suggestion)
    {
        string sugg = suggestion.ToStringAndDispose();
        string name = MemberNaming.CreateMemberName(memberType, sugg);
        return RegisterName(name);
    }
    
    public static DynamicMethod CreateDynamicMethod(
        DelegateInfo signature,
        InterpolatedCode name = default)
    {
        return new DynamicMethod(
            // ensure we have a valid name
            name: GetMemberName(MemberTypes.Method, name),
            // only valid combination
            attributes: MethodAttributes.Public | MethodAttributes.Static,
            // only valid value
            callingConvention: CallingConventions.Standard,
            // pass through from sig
            returnType: signature.ReturnType,
            parameterTypes: signature.ParameterTypes,
            // we always belong to our module builder
            m: ModuleBuilder,
            // anyone can use this dynamic method
            skipVisibility: true);
    }

    public static RuntimeDelegateBuilder CreateRuntimeDelegateBuilder(
        DelegateInfo delegateSig,
        InterpolatedCode name = default)
    {
        var dynamicMethod = CreateDynamicMethod(delegateSig, name);
        return new RuntimeDelegateBuilder(dynamicMethod, delegateSig);
    }

    public static RuntimeDelegateBuilder CreateRuntimeDelegateBuilder(
        Type delegateType,
        InterpolatedCode name = default) => CreateRuntimeDelegateBuilder(DelegateInfo.For(delegateType), name);

    public static RuntimeDelegateBuilder<TDelegate> CreateRuntimeDelegateBuilder<TDelegate>(
        InterpolatedCode name = default)
        where TDelegate : Delegate
    {
        var dynamicMethod = CreateDynamicMethod(DelegateInfo.For<TDelegate>(), name);
        return new RuntimeDelegateBuilder<TDelegate>(dynamicMethod);
    }


    public static Delegate GenerateDelegate(
        DelegateInfo signature,
        InterpolatedCode name,
        Action<ILGenerator> generateDelegate)
    {
        var method = CreateDynamicMethod(signature, name);
        var generator = method.GetILGenerator();
        generateDelegate(generator);
        return method.CreateDelegate(signature.DelegateType);
    }

    public static TDelegate GenerateDelegate<TDelegate>(
        InterpolatedCode name,
        Action<ILGenerator> generateDelegate)
        where TDelegate : Delegate
    {
        var method = CreateDynamicMethod(DelegateInfo.For<TDelegate>(), name);
        var generator = method.GetILGenerator();
        generateDelegate(generator);
        return method.CreateDelegate<TDelegate>();
    }

    public static Delegate BuildDelegate(
        DelegateInfo signature,
        InterpolatedCode name,
        Action<RuntimeDelegateBuilder> buildDelegate)
    {
        var runtimeDelegateBuilder = CreateRuntimeDelegateBuilder(signature, name);
        buildDelegate(runtimeDelegateBuilder);
        return runtimeDelegateBuilder.CreateDelegate();
    }

    public static Delegate BuildDelegate(
        Type delegateType,
        InterpolatedCode name,
        Action<RuntimeDelegateBuilder> buildDelegate) => BuildDelegate(
        DelegateInfo.For(delegateType),
        name,
        buildDelegate);

    public static TDelegate BuildDelegate<TDelegate>(
        InterpolatedCode name,
        Action<RuntimeDelegateBuilder<TDelegate>> buildDelegate)
        where TDelegate : Delegate
    {
        var runtimeDelegateBuilder = CreateRuntimeDelegateBuilder<TDelegate>(name);
        buildDelegate(runtimeDelegateBuilder);
        return runtimeDelegateBuilder.CreateDelegate();
    }

    public static Delegate EmitDelegate(
        DelegateInfo signature,
        Action<FluentGeneratorEmitter> emitDelegate)
    {
        return EmitDelegate(
            signature,
            default,
            emitDelegate);
    }

    public static Delegate EmitDelegate(
        Type delegateType,
        Action<FluentGeneratorEmitter> emitDelegate)
    {
        return EmitDelegate(
            delegateType,
            default,
            emitDelegate);
    }

    public static Delegate EmitDelegate(
        DelegateInfo signature,
        InterpolatedCode name,
        Action<FluentGeneratorEmitter> emitDelegate)
    {
        var runtimeMethod = CreateRuntimeDelegateBuilder(signature, name);
        emitDelegate(runtimeMethod.Emitter);
        return runtimeMethod.CreateDelegate();
    }

    public static Delegate EmitDelegate(
        Type delegateType,
        InterpolatedCode name,
        Action<FluentGeneratorEmitter> emitDelegate)
    {
        if (!delegateType.Implements<Delegate>())
            throw new ArgumentException("Must be a delegate", nameof(delegateType));
        var runtimeMethod = CreateRuntimeDelegateBuilder(delegateType, name);
        emitDelegate(runtimeMethod.Emitter);
        return runtimeMethod.CreateDelegate();
    }


    public static TDelegate EmitDelegate<TDelegate>(
        Action<FluentGeneratorEmitter> emitDelegate)
        where TDelegate : Delegate
    {
        return EmitDelegate<TDelegate>(default, emitDelegate);
    }

    public static TDelegate EmitDelegate<TDelegate>(
        InterpolatedCode name,
        Action<FluentGeneratorEmitter> emitDelegate)
        where TDelegate : Delegate
    {
        var runtimeMethod = CreateRuntimeDelegateBuilder<TDelegate>(name);
        emitDelegate(runtimeMethod.Emitter);
        return runtimeMethod.CreateDelegate();
    }

    public static TypeBuilder DefineType(
        TypeAttributes typeAttributes,
        InterpolatedCode name = default)
    {
        return ModuleBuilder.DefineType(
            MemberNaming.CreateMemberName(MemberTypes.TypeInfo, name.ToStringAndDispose()),
            typeAttributes,
            typeof(RuntimeBuilder));
    }

    public static CustomAttributeBuilder GetCustomAttributeBuilder<TAttribute>()
        where TAttribute : Attribute, new()
    {
        var ctor = typeof(TAttribute)
            .GetConstructor(
                BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic,
                default,
                Type.EmptyTypes,
                default);
        if (ctor is null)
            throw new ReflectedException($"Cannot find an empty {typeof(TAttribute)} constructor.");
        return new CustomAttributeBuilder(ctor, Array.Empty<object>());
    }

    public static CustomAttributeBuilder GetCustomAttributeBuilder<TAttribute>(params object[] ctorArgs)
        where TAttribute : Attribute
    {
        var search = new MemberSearchOptions
        {
            ParameterTypes = ctorArgs.GetElementTypes(typeof(object)),
        };
        var ctor = MemberSearch.One<TAttribute, ConstructorInfo>(search);
        return new CustomAttributeBuilder(ctor, ctorArgs);
    }

    public static CustomAttributeBuilder GetCustomAttributeBuilder(Type attributeType, params object[] ctorArgs)
    {
        ValidateType.IsAttributeType(attributeType);
        var search = new MemberSearchOptions
        {
            ParameterTypes = ctorArgs.GetElementTypes(typeof(object)),
        };
        var ctor = MemberSearch.One<ConstructorInfo>(attributeType, search);
        return new CustomAttributeBuilder(ctor, ctorArgs);
    }
}