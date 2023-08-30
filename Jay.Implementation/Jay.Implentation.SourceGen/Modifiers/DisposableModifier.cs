using IMPL.SourceGen.MemberCodes;

namespace IMPL.SourceGen.Modifiers;

public class DisposableModifier : IInterfaceImplementationModifier
{
    public bool AppliesTo(TypeSig interfaceType)
    {
        return interfaceType == typeof(IDisposable);
    }

    public void PreRegister(Implementer implementer)
    {
        IMemberCode disposeMethod = new CustomMemberCode(new MemberPos(Instic.Instance, SigType.Method, Visibility.Public),
            (impl, code) =>
            {
                code.AppendLine("public void Dispose()")
                .BracketBlock(methodBlock =>
                {
                    // Do we have anything specifically marked with the Dispose Attribute?
                    var disposeMembers = implementer.GetMembers<MemberSig>().Where(m => m.HasAttribute<DisposeAttribute>()).ToList();
                    // If none, we'll just do events
                    if (disposeMembers.Count == 0)
                    {
                        disposeMembers = implementer.GetMembers<MemberSig>().Where(m => m.SigType == SigType.Event).ToList();
                    }
                    // if still none, we do nothing
                    if (disposeMembers.Count == 0)
                        return;

                    // For each, we'll dispose it
                    foreach (var member in disposeMembers)
                    {
                        if (member is EventSig eventSig)
                        {
                            methodBlock.CodeLine($"this.{eventSig.Name} = null!;");
                        }
                        else
                        {
                            methodBlock.CodeLine($"this.{member.Name}?.Dispose();");
                        }
                    }
                });
            });
        implementer.AddMember(disposeMethod);
    }
}
