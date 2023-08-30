using IMPL.SourceGen.Modifiers;

namespace IMPL.SourceGen.Writers;

internal class BackingPropertySigWriter : IPropertySigWriter
{
    public void Write(PropertySig propertySig, CodeBuilder codeBuilder)
    {
        var fieldName = BackingFieldModifier.GetBackingFieldName(propertySig);
        codeBuilder
           .Append(propertySig.Visibility, "lc")
           .AppendIf(propertySig.Instic == Instic.Instance, " ", " static ")
           //.AppendKeywords(propertySig.Keywords)
           .Append(propertySig.PropertyType).Append(' ')
           .AppendLine(propertySig.Name)
           .BracketBlock(propertyBlock =>
           {
               if (propertySig.GetMethod is not null)
               {
                   propertyBlock.CodeLine($"get => this.{fieldName};");
               }
               if (propertySig.SetMethod is not null)
               {
                   if (propertySig.SetMethod.Keywords.HasFlag(Keywords.Init))
                       propertyBlock.Append("init");
                   else
                       propertyBlock.Append("set");
                   propertyBlock.CodeLine($" => this.{fieldName} = value;");
               }
           }).NewLine();
    }
}
