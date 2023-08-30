namespace IMPL.SourceGen.Modifiers;


public class BackingFieldModifier : IImplModifier
{
    public static string GetBackingFieldName(PropertySig propertySig)
    {
        string propName = propertySig.Name!;
        Span<char> buffer = stackalloc char[propName.Length + 1];
        buffer[0] = '_';
        buffer[1] = char.ToLower(propName[0]);
        TextHelper.CopyTo(propName.AsSpan(1), buffer[2..]);
        return buffer.ToString();
    }

    public void PreRegister(Implementer implementer)
    {
        var fields = implementer.GetMembers<FieldSig>().ToList();
        var properties = implementer.GetMembers<PropertySig>().ToList();

        foreach (var property in properties)
        {
            // Do we already have a backing field?
            FieldSig? field = fields
                .Where(f => f.Name!.Contains(property.Name) && f.FieldType == property.PropertyType)
                .OneOrDefault();
            if (field is null)
            {
                field = new()
                {
                    Name = GetBackingFieldName(property),
                    FieldType = property.PropertyType,
                    Instic = property.Instic,
                    Visibility = Visibility.Private,
                    Keywords = default,
                };

                // No setter, field can be readonly
                if (property.SetMethod is null)
                {
                    field.Keywords |= Keywords.Readonly;
                }

                // Add this field
                implementer.AddMember(field);
            }
            else
            {
                // Change its name
                field.Name = GetBackingFieldName(property);
            }
        }

        implementer.PropertyWriter = new BackingPropertySigWriter();
    }
}
