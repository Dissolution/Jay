using IMPL.SourceGen.Modifiers;
using IMPL.SourceGen.Writers;

namespace IMPL.SourceGen.MemberCodes;

public class PropertyConstructorsCode : IMemberCode
{
    public MemberPos Pos { get; } = new(Instic.Instance, SigType.Constructor, Visibility.Public);

    public void Write(Implementer implementer, CodeBuilder codeBuilder)
    {
        var spec = implementer.ImplSpec;
        var implType = spec.ImplType;

        // All properties
        var properties = implementer.GetMembers<PropertySig>().ToList();

        // Write the full constructor
        writePropsCtor(properties);

        // Read-only subset
        var roProps = properties.Where(ps => ps.SetMethod is null).ToList();

        if (roProps.Count != properties.Count)
        {
            writePropsCtor(roProps);
        }

        void writePropsCtor(IReadOnlyList<PropertySig> props)
        {
            codeBuilder.Code($"public {implType}(")
                .Delimit(", ", props, (b, p) =>
                {
                    b.Append(p.PropertyType).Append(' ').Append(p.Name.ToVariableName());
                }).AppendLine(')')
                .BracketBlock(ctorBlock =>
                {
                    // Set properties
                    ctorBlock.LineDelimit(props, (b, p) =>
                    {
                        string thingName;
                        if (implementer.PropertyWriter is BackingPropertySigWriter)
                        {
                            thingName = BackingFieldModifier.GetBackingFieldName(p);
                        }
                        else
                        {
                            thingName = p.Name!;
                        }

                        b.Append("this.").Append(thingName).Append(" = ").Append(p.Name.ToVariableName()).Append(';');
                    });
                }).NewLine();
        }
    }
}
