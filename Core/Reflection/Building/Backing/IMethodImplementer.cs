using System.Diagnostics;
using System.Reflection;
using System.Reflection.Emit;
using Jay.Enums;

namespace Jay.Reflection.Building.Backing;

public interface IMethodImplementer
{
    MethodBuilder ImplementMethod(MethodInfo method);
}

internal class MethodImplementer : Implementer, IMethodImplementer
{
    public MethodImplementer(TypeBuilder typeBuilder) : base(typeBuilder)
    {
    }

    public MethodBuilder ImplementMethod(MethodInfo method)
    {
        throw new NotImplementedException();
    }
}

internal class InterfaceDefaultMethodImplementer : Implementer, IMethodImplementer
{
    public InterfaceDefaultMethodImplementer(TypeBuilder typeBuilder) : base(typeBuilder)
    {
    }

    public MethodBuilder ImplementMethod(MethodInfo method)
    {
        // Has an implementation
        Debug.Assert(!method.Attributes.HasAnyFlags(MethodAttributes.Abstract));
        // Implementation is always HasThis!
        
        var methodBuilder = _typeBuilder.DefineMethod(
            method.Name,
            GetMethodImplementationAttributes(method) | MethodAttributes.HideBySig,
            GetCallingConventions(method),
            method.ReturnType,
            method.GetParameterTypes());
        var emitter = methodBuilder.GetILEmitter();
        // Load this
        emitter.Ldarg_0()
            .Castclass(method.DeclaringType!);      // as Interface
        int len = method.GetParameters().Length;
        // Load all the rest of the args
        for (var i = 1; i <= len; i++)
        {
            emitter.Ldarg(i);
        }
        // Call the interface's method (not virtual!)
        emitter.Emit(OpCodes.Call, method)
            .Ret();

        return methodBuilder;
    }
}