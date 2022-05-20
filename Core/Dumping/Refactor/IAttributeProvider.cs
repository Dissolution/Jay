using System.Reflection;

namespace Jay.Dumping.Refactor;

public interface IAttributeProvider : ICustomAttributeProvider
{
    object[] ICustomAttributeProvider.GetCustomAttributes(Type attributeType, bool inherit)
    {
        object[] customAttributes = GetCustomAttributes(inherit);
        var attributes = new List<Attribute>(customAttributes.Length);
        foreach (Attribute customAttribute in customAttributes)
        {
            if (customAttribute.GetType().Implements(attributeType))
            {
                attributes.Add(customAttribute);
            }
        }

        // ReSharper disable once CoVariantArrayConversion
        return (object[])attributes.ToArray();
    }

    bool ICustomAttributeProvider.IsDefined(Type attributeType, bool inherit)
    {
        return GetCustomAttributes(attributeType, inherit).Length > 0;
    }

    public bool HasAttribute<TAttribute>(bool inherit = true) 
        where TAttribute : Attribute
    {
        foreach (Attribute customAttribute in GetCustomAttributes(inherit))
        {
            if (customAttribute is TAttribute)
                return true;
        }
        return false;
    }

    public TAttribute? GetAttribute<TAttribute>(bool inherit = true)
        where TAttribute : Attribute
    {
        foreach (Attribute customAttribute in GetCustomAttributes(inherit))
        {
            if (customAttribute is TAttribute attribute)
                return attribute;
        }
        return null;
    }

    public IReadOnlyList<Attribute> Attributes { get; }
}