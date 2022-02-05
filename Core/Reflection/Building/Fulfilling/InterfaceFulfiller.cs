using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Jay.Annotations;
using Jay.Dumping;
using Jay.Reflection.Building.Emission;
using Jay.Reflection.Building.Fulfilling;

namespace Jay.Reflection.Building;

public abstract class InterfaceFulfiller
{
    protected readonly TypeBuilder _typeBuilder;

    public Type InterfaceType { get; }

    protected InterfaceFulfiller(Type interfaceType)
    {
        ArgumentNullException.ThrowIfNull(interfaceType);
        if (!interfaceType.IsInterface)
            Dump.ThrowException<ArgumentException>($"{interfaceType} is not a valid interface type");
        _typeBuilder = RuntimeBuilder.DefineType($"FF_{interfaceType.Name}", TypeAttributes.Public | TypeAttributes.Class);
    }

    protected virtual FieldBuilder DefineField(string name, 
                                               FieldAttributes attributes,
                                               Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        return _typeBuilder.DefineField(RuntimeBuilder.FixedMemberName(name, MemberTypes.Field),
                                        type,
                                        attributes);
    }

    protected virtual PropertyBuilder DefineProperty(string name, 
                                                     PropertyAttributes attributes,
                                                     Type type,
                                                     params Type[]? parameterTypes)
    {
        ArgumentNullException.ThrowIfNull(type);
        return _typeBuilder.DefineProperty(RuntimeBuilder.FixedMemberName(name, MemberTypes.Property),
                                           attributes,
                                           type,
                                           parameterTypes);
    }

    protected virtual EventBuilder DefineEvent(string name,
                                               EventAttributes attributes,
                                               Type type)
    {
        ArgumentNullException.ThrowIfNull(type);
        if (!type.Implements<Delegate>())
            Dump.ThrowException<ArgumentException>($"{type} is not a valid event handler type");
        return _typeBuilder.DefineEvent(RuntimeBuilder.FixedMemberName(name, MemberTypes.Event),
                                        attributes, 
                                        type);
    }

    protected virtual ConstructorBuilder DefineConstructor(MethodAttributes attributes,
                                                           params Type[]? parameterTypes)
    {
        return _typeBuilder.DefineConstructor(attributes, CallingConventions.Standard, parameterTypes);
    }

    protected virtual MethodBuilder DefineMethod(string name,
                                                 MethodAttributes attributes,
                                                 Type returnType,
                                                 params Type[]? parameterTypes)
    {
        return _typeBuilder.DefineMethod(name, attributes, returnType, parameterTypes);
    }

    protected virtual MethodBuilder DefineGetMethod(PropertyBuilder property)
    {
        return _typeBuilder.DefineMethod($"get_{property.Name}",
                                         MethodAttributes.Private | MethodAttributes.Final,
                                         property.PropertyType,
                                         Type.EmptyTypes);
    }

    protected virtual MethodBuilder DefineSetMethod(PropertyBuilder property)
    {
        return _typeBuilder.DefineMethod($"set_{property}",
                                         MethodAttributes.Private | MethodAttributes.Final,
                                         typeof(void),
                                         new Type[1] {property.PropertyType});
    }
}

public class NotifyPropertyChangedFulfiller : InterfaceFulfiller
{
    public NotifyPropertyChangedFulfiller(Type interfaceType) 
        : base(interfaceType)
    {
        if (!interfaceType.Implements<INotifyPropertyChanged>())
            Dump.ThrowException<ArgumentException>($"{interfaceType} is not INotifyPropertyChanged");
    }

    protected virtual EventBuilder DefinePropertyChangedEvent()
    {
        var backingField = _typeBuilder
            .DefineField(nameof(INotifyPropertyChanged.PropertyChanged),
                typeof(PropertyChangedEventHandler),
                FieldAttributes.Private);
        throw new NotImplementedException();
    }



    protected FieldBuilder EmitEvent(string eventName,
                                     Type eventHandlerType)
    {
        var field = _typeBuilder.DefineField(eventName, eventHandlerType, FieldAttributes.Private);
        var @event = _typeBuilder.DefineEvent(eventName, EventAttributes.None, eventHandlerType);
        var addMethod = _typeBuilder.DefineMethod($"add_{eventName}",
            MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.NewSlot | MethodAttributes.SpecialName,
            CallingConventions.HasThis,
            typeof(void),
            new Type[1] { eventHandlerType });
        addMethod.GetEmitter()
              .DeclareLocal(eventHandlerType, out var addTemp0)
              .DeclareLocal(eventHandlerType, out var addTemp1)
              .DeclareLocal(eventHandlerType, out var addTemp2)
              .Ldarg(0) //this
              .Ldfld(field)
              .Stloc(addTemp0)
              .DefineAndMarkLabel(out var lblAddLoopStart)
              .Ldloc(addTemp0)
              .Stloc(addTemp1)
              .Ldloc(addTemp1)
              .Ldarg(1)
              .Call(MethodCache.DelegateCombineMethod)
              .Castclass(eventHandlerType)
              .Stloc(addTemp2)
              .Ldarg(0)
              .Ldflda(field)
              .Ldloc(addTemp2)
              .Ldloc(addTemp1)
              .Call(MethodCache.InterlockedCompareExchange(eventHandlerType))
              .Stloc(addTemp0)
              .Ldloc(addTemp0)
              .Ldloc(addTemp1)
              .Bne_Un_S(lblAddLoopStart)
              .Ret();
        @event.SetAddOnMethod(addMethod);

        var removeMethod = _typeBuilder.DefineMethod($"remove_{eventName}",
            MethodAttributes.Public | MethodAttributes.Final | MethodAttributes.NewSlot | MethodAttributes.SpecialName,
            CallingConventions.HasThis,
            typeof(void),
            new Type[1] { eventHandlerType });
        removeMethod.GetEmitter()
                    .DeclareLocal(eventHandlerType, out var removeTemp0)
                    .DeclareLocal(eventHandlerType, out var removeTemp1)
                    .DeclareLocal(eventHandlerType, out var removeTemp2)
                    .Ldarg(0)
                    .Ldfld(field)
                    .Stloc_0()
                    .DefineAndMarkLabel(out var lblRemoveLoopStart)
                    .Ldloc_0()
                    .Stloc_1()
                    .Ldloc_1()
                    .Ldarg(1)
                    .Call(MethodCache.DelegateRemoveMethod)
                    .Castclass(eventHandlerType)
                    .Stloc_2()
                    .Ldarg(0)
                    .Ldflda(field)
                    .Ldloc_2()
                    .Ldloc_1()
                    .Call(MethodCache.InterlockedCompareExchange(eventHandlerType))
                    .Stloc_0()
                    .Ldloc_0()
                    .Ldloc_1()
                    .Bne_Un_S(lblRemoveLoopStart)
                    .Ret();
        @event.SetRemoveOnMethod(removeMethod);
        return field;
    }

    protected void EmitCallEvent(IILGeneratorEmitter emitter,
                                 FieldBuilder eventField)
    {
        emitter.Ldarg(0) //this
               .Ldfld(eventField) //this.EventField
               .Dup() // This is for thread-safety
               .Brtrue(out var lblOk)
               .Pop()
               .Br(out var lblNope)
               .MarkLabel(lblOk)
               .Ldarg(0) //this
               .Ldarg(3) // string propertyName
               .Ret();
        throw new NotImplementedException();
    }

    /*
     
       IL_0013: ldarg.0      // this
    IL_0014: ldfld        class [System.ObjectModel]System.ComponentModel.PropertyChangedEventHandler Jay.Reflection.Building.NotifyBase::PropertyChanged
    IL_0019: dup
    IL_001a: brtrue.s     IL_001f
    IL_001c: pop
    IL_001d: br.s         IL_002b
    IL_001f: ldarg.0      // this
    IL_0020: ldarg.3      // propertyName
    IL_0021: newobj       instance void [System.ObjectModel]System.ComponentModel.PropertyChangedEventArgs::.ctor(string)
    IL_0026: callvirt     instance void [System.ObjectModel]System.ComponentModel.PropertyChangedEventHandler::Invoke(object, class [System.ObjectModel]System.ComponentModel.PropertyChangedEventArgs)

     */


    protected virtual MethodBuilder DefineSetValueMethod()
    {
        var method = _typeBuilder.DefineMethod("SetValue",
                                               MethodAttributes.Family | MethodAttributes.Final,
                                               CallingConventions.HasThis);
        var genericTypeParameterBuilders = method.DefineGenericParameters("T");
        var tBuilder = genericTypeParameterBuilders[0];
        method.SetParameters(tBuilder.MakeByRefType(), tBuilder, typeof(string));
        method.DefineParameter(0, ParameterAttributes.None, "field");
        method.DefineParameter(1, ParameterAttributes.None, "value");
        var propertyNameParameter = method.DefineParameter(2, ParameterAttributes.HasDefault, "propertyName");
        propertyNameParameter.SetConstant((string?)null);
        propertyNameParameter.SetCustomAttribute(RuntimeBuilder.GetCustomAttributeBuilder<CallerMemberNameAttribute>());
        // HERE
        throw new NotImplementedException();
    }

    protected virtual PropertyInfo DefineNotifyProperty(string name,
                                                        PropertyAttributes attributes,
                                                        Type type,
                                                        params Type[]? parameterTypes)
    {
        name = RuntimeBuilder.FixedMemberName(name, MemberTypes.Property);
        var field = base.DefineField(RuntimeBuilder.FieldName(name), FieldAttributes.Private, type);
        var property = base.DefineProperty(name, attributes, type, parameterTypes);
        var getMethod = DefineGetMethod(property)
                        .GetEmitter()
                        .Ldarg(0) //this
                        .Ldfld(field)
                        .Ret();
        var equalityMethods = MethodCache.GetEqualityMethods(property.PropertyType);
        var setMethod = DefineSetMethod(property)
                        .GetEmitter()
                        .Call(equalityMethods.GetDefaultMethod)
                        .Ldarg(0)
                        .Ldfld(field)
                        .Ldarg(1)
                        .Call(equalityMethods.EqualsMethod)
                        .Brfalse(out var lblSet)
                        .Ret()
                        .MarkLabel(lblSet)
                        .Ldarg(0)
                        .Ldarg(1)
                        .Stfld(field)
                        .Ret();
                                                 


        throw new NotImplementedException();
    }
}


public abstract class NotifyBase : INotifyPropertyChanged
{
    public event PropertyChangedEventHandler? PropertyChanged;

    [MethodImpl(MethodImplOptions.AggressiveInlining | MethodImplOptions.AggressiveOptimization)]
    protected bool SetValue<T>(ref T field, T value, [CallerMemberName] string? propertyName = null)
    {
        if (!EqualityComparer<T>.Default.Equals(field, value))
        {
            this.PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        return false;
    }

    /* .method family hidebysig instance bool
       SetValue<T>(
       !!0/*T* /& 'field',
       !!0/*T* / 'value',
       [opt] string propertyName
       ) cil managed
       {
       .custom instance void System.Runtime.CompilerServices.NullableContextAttribute::.ctor([in] unsigned int8)
       = (01 00 01 00 00 ) // .....
       // unsigned int8(1) // 0x01
       .param [3] = nullref
       .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor([in] unsigned int8)
       = (01 00 02 00 00 ) // .....
       // unsigned int8(2) // 0x02
       .custom instance void [System.Runtime]System.Runtime.CompilerServices.CallerMemberNameAttribute::.ctor()
       = (01 00 00 00 )
       .param type [1] /*T* /
       .custom instance void System.Runtime.CompilerServices.NullableAttribute::.ctor([in] unsigned int8)
       = (01 00 02 00 00 ) // .....
       // unsigned int8(2) // 0x02
       .maxstack 8
       
       // [131 9 - 131 63]
       IL_0000: call         class [System.Collections]System.Collections.Generic.EqualityComparer`1<!0/*T* /> class [System.Collections]System.Collections.Generic.EqualityComparer`1<!!0/*T* />::get_Default()
       IL_0005: ldarg.1      // 'field'
       IL_0006: ldobj        !!0/*T* /
       IL_000b: ldarg.2      // 'value'
       IL_000c: callvirt     instance bool class [System.Collections]System.Collections.Generic.EqualityComparer`1<!!0/*T* />::Equals(!0/*T* /, !0/*T* /)
       IL_0011: brtrue.s     IL_002b
       
       // [133 13 - 133 92]
       IL_0013: ldarg.0      // this
       IL_0014: ldfld        class [System.ObjectModel]System.ComponentModel.PropertyChangedEventHandler Jay.Reflection.Building.NotifyBase::PropertyChanged
       IL_0019: dup
       IL_001a: brtrue.s     IL_001f
       IL_001c: pop
       IL_001d: br.s         IL_002b
       IL_001f: ldarg.0      // this
       IL_0020: ldarg.3      // propertyName
       IL_0021: newobj       instance void [System.ObjectModel]System.ComponentModel.PropertyChangedEventArgs::.ctor(string)
       IL_0026: callvirt     instance void [System.ObjectModel]System.ComponentModel.PropertyChangedEventHandler::Invoke(object, class [System.ObjectModel]System.ComponentModel.PropertyChangedEventArgs)
       
       // [135 9 - 135 22]
       IL_002b: ldc.i4.0
       IL_002c: ret
       
       } // end of method NotifyBase::SetValue
     *
     */
}

public class Entity : NotifyBase
{
    private int _id;

    public int Id
    {
        get => _id;
        set => SetValue(ref _id, value);
    }
    
}