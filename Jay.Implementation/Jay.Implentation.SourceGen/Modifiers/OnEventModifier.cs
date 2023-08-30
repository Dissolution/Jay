using IMPL.SourceGen.MemberCodes;

using Jay.SourceGen.Validation;

using System.Diagnostics;

namespace IMPL.SourceGen.Modifiers;

public class OnEventModifier : IImplModifier
{
    private void WriteOnEventMethodCode(Implementer impl, EventSig eventSig, CodeBuilder code)
    {
        var eventHandler = eventSig.HandlerSig.ThrowIfNull();
        if (eventHandler.ReturnType != typeof(void))
            throw new NotImplementedException();
        var paramTypes = eventHandler.Parameters;

        // If is is a common EventHandler or EventHandler<TArgs>, we have an easy handle
        if (eventHandler.Name == "EventHandler" && 
            paramTypes.Count == 2 &&
            paramTypes[0].Name == "sender" &&
            paramTypes[0].ParameterType == typeof(object))
        {
            var eventArgs = paramTypes[1];
            code.CodeLine($"public void On{eventSig.Name}({eventArgs.ParameterType} {eventArgs.Name})")
        }
        else
        {
            Debugger.Break();
            throw new NotImplementedException();
        }


        code.Code($"public void {methodSig.Name}(")
                .Delimit(", ", methodSig.Parameters, (cb,mp) =>
                {
                    cb.AppendIf(mp.IsParams, "params ")
                      .Append(mp.ParameterType)
                      .Append(' ')
                      .Append(mp.Name)
                      .If(mp.HasDefaultValue, b => b.Code($" = {mp.DefaultValue}"));
                }).AppendLine(')')
                .BracketBlock(methodBlock =>
                {

                })
            );
        return methodCode;
    }

    public void PreRegister(Implementer implementer)
    {
        var eventSigs = implementer.GetMembers<EventSig>().ToList();

        foreach (var eventSig in eventSigs)
        {
            var eventHandlerSig = eventSig.HandlerSig!;

            if (eventHandlerSig.ReturnType != typeof(void))
            {
                Debugger.Break();
                throw new NotImplementedException();
            }

            
            // Create the code for the OnXYZ Method
            IMemberCode methodCode = new CustomMemberCode(
                new MemberPos(Instic.Instance, SigType.Method, Visibility.Protected),
                (impl, code) => WriteOnEventMethodCode(impl, eventSig, code));
          
            // Add it!
            implementer.AddMember(methodCode);
        }
    }
}