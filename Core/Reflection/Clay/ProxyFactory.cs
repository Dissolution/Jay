/*
using System;
using System.Linq;
using System.Reflection;
using System.Reflection.Emit;
using System.Threading;

namespace Jay.Reflection.Duck
{
	public class ProxyFactoryLock : ProxyFactory
	{
		protected FieldBuilder _lockField;
		protected MethodInfo _monitorEnterMethod;
		protected MethodInfo _monitorExitMethod;

		public ProxyFactoryLock(Type sourceType, Type destType)
			: base(sourceType, destType)
		{
			_monitorEnterMethod = typeof(Monitor)
				.GetMethod(
					"Enter", 
					BindingFlags.Public | BindingFlags.Static, 
					null, 
					new[] { typeof(object), typeof(bool).MakeByRefType() }, 
					null);

			_monitorExitMethod = typeof(Monitor)
				.GetMethod(
					"Exit", 
					BindingFlags.Public | BindingFlags.Static, 
					null,
					new[] { typeof(object) }, 
					null);
		}

		/// <inheritdoc />
		protected override MethodBuilder ImplementMethod(MethodInfo method)
		{
			var parameters = method.GetParameters();
			var parameterTypes = parameters.Select(p => p.ParameterType).ToArray();

			var baseMethod = _baseField.FieldType.GetMethod(method.Name, parameterTypes);

			var builder = _typeBuilder.DefineMethod(method.Name,
				MethodAttributes.Public | MethodAttributes.Virtual | MethodAttributes.HideBySig,
				method.ReturnType,
				parameterTypes);

			var emit = new AdvancedEmitter(builder.GetILGenerator());
			/*
				* .method public hidebysig instance string 
			DoThing(
			int32 a, 
			bool b
			) cil managed 
			{
			.maxstack 3
			.locals init (
			[0] object V_0,
			[1] bool V_1,
			[2] string V_2
			)#1#
			emit.DeclareLocal<object>(out LocalBuilder v0)
				.DeclareLocal<bool>(out LocalBuilder taken)
				.DeclareLocal(method.ReturnType, out LocalBuilder v2);
			/*
			// [81 3 - 81 4]
			IL_0000: nop          

			// [82 4 - 82 16]
			IL_0001: ldarg.0      // this
			IL_0002: ldfld        object Jay.ConsolePad.TestClassProxy::_lock
			IL_0007: stloc.0      // V_0
			IL_0008: ldc.i4.0     
			IL_0009: stloc.1      // V_1
			#1#

			//store _lock in v0
			emit.Ldarg_0()
				.Ldfld(_lockField)
				.Stloc_0();

			//Store false in taken
			emit.Ldc_I4_0()
				.Stloc_1();
			
			/*
			.try
			{#1#

			emit.BeginExceptionBlock(out _);
			
			/*
      IL_000a: ldloc.0      // V_0
      IL_000b: ldloca.s     V_1
      IL_000d: call         void [mscorlib]System.Threading.Monitor::Enter(object, bool&)
      IL_0012: nop          
	  #1#
			//Monitor.Enter
			emit.Ldloc_0()
				.Ldloca_S(taken)
				.Call(_monitorEnterMethod);

			/*
      // [83 4 - 83 5]
      IL_0013: nop          

      // [84 5 - 84 32]
      IL_0014: ldarg.0      // this
      IL_0015: ldfld        class Jay.ConsolePad.TestClassBase Jay.ConsolePad.TestClassProxy::_base
      IL_001a: ldarg.1      // a
      IL_001b: ldarg.2      // b
      IL_001c: callvirt     instance string Jay.ConsolePad.TestClassBase::DoThing(int32, bool)
      IL_0021: stloc.2      // V_2
      IL_0022: leave.s      IL_002f
	  #1#
			//Call base method on base field
			emit.Ldarg_0()
				.Ldfld(_baseField);
			for (var i = 1; i <= parameters.Length; i++)
			{
				emit.Ldarg(i);
			}
			emit.Callvirt(baseMethod);

			//Stored return variable
			emit.Stloc_2()
				//GOTO: 2f
				.DefineLabel(out Label lbl2f)
				.Leave_S(lbl2f);

			/*
    } // end of .try
    finally
	#1#
			emit.BeginFinallyBlock();
			//il.EmitWriteLine("}FINALLY{");

			/*
    {

      IL_0024: ldloc.1      // V_1
      IL_0025: brfalse.s    IL_002e
      IL_0027: ldloc.0      // V_0
      IL_0028: call         void [mscorlib]System.Threading.Monitor::Exit(object)
      IL_002d: nop          
	  #1#
			//Break if not taken, otherwise monitor.exe
			emit.Ldloc_1()
				.DefineLabel(out Label lbl2e)
				.Brfalse_S(lbl2e)
				.Ldloc_0()
				.Call(_monitorExitMethod);

			/*
      IL_002e: endfinally   
    } // end of finally
	#1#
			emit.MarkLabel(lbl2e)
				.EndExceptionBlock();

			//il.MarkLabel(lbl2e);
			//il.EndExceptionBlock();
			//il.EmitWriteLine("2E, }(end)");

			/*

    // [86 3 - 86 4]
    IL_002f: ldloc.2      // V_2
    IL_0030: ret          

  } // end of method TestClassProxy::DoThing
			 #1#

			emit.MarkLabel(lbl2f)
				.Ldloc_2()
				.Ret();
			

			//il.MarkLabel(lbl2f);
			//il.Emit(OpCodes.Ldloc_2);
			//il.EmitWriteLine("Store return to return");
			//il.Emit(OpCodes.Ret);


			_typeBuilder.DefineMethodOverride(builder, method);
			return builder;
		}

		/// <inheritdoc />
		protected override ConstructorBuilder ImplementConstructor()
		{
			var constructorBuilder = _typeBuilder.DefineConstructor(
				MethodAttributes.Public,
				CallingConventions.Standard,
				new[] { SourceType });
			var il = constructorBuilder.GetILGenerator();

			/*
			 .method public hidebysig specialname rtspecialname instance void 
    .ctor(
      class Jay.ConsolePad.TestClassBase b
    ) cil managed 
  {
    .maxstack 8

    // [74 3 - 74 41]
    IL_0000: ldarg.0      // this
    IL_0001: call         instance void [mscorlib]System.Object::.ctor()
    IL_0006: nop          

    // [75 3 - 75 4]
    IL_0007: nop          

    // [76 4 - 76 25]
    IL_0008: ldarg.0      // this
    IL_0009: newobj       instance void [mscorlib]System.Object::.ctor()
    IL_000e: stfld        object Jay.ConsolePad.TestClassProxy::_lock

    // [77 4 - 77 14]
    IL_0013: ldarg.0      // this
    IL_0014: ldarg.1      // b
    IL_0015: stfld        class Jay.ConsolePad.TestClassBase Jay.ConsolePad.TestClassProxy::_base

    // [78 3 - 78 4]
    IL_001a: ret          

  } // end of method TestClassProxy::.ctor
			 #1#

			il.Emit(OpCodes.Ldarg_0);
			var ctor = typeof(object).GetConstructor(Type.EmptyTypes);
			il.Emit(OpCodes.Call, ctor);
			//il.Emit(OpCodes.Nop);
			//il.Emit(OpCodes.Nop);

			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Newobj, ctor);
			il.Emit(OpCodes.Stfld, _lockField);

			il.Emit(OpCodes.Ldarg_0);
			il.Emit(OpCodes.Ldarg_1);
			il.Emit(OpCodes.Stfld, _baseField);

			il.Emit(OpCodes.Ret);

			//Fin
			return constructorBuilder;
		}

		/// <inheritdoc />
		protected override void AddFields()
		{
			//.field private initonly object _lock
			_lockField = _typeBuilder.DefineField("_lock", typeof(object), FieldAttributes.Private | FieldAttributes.InitOnly);

		}

	}
}
*/
