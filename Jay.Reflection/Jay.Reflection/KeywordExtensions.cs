namespace Jay.Reflection;

public static class KeywordExtensions
{
    public static Keywords GetKeywords(this MemberInfo? member)
    {
        Keywords keywords = default;
        switch (member)
        {
            case null:
                return keywords;
            case FieldInfo fieldInfo:
            {
                if (fieldInfo.IsInitOnly)
                    keywords.AddFlag(Keywords.Readonly);
                if (fieldInfo.IsLiteral)
                    keywords.AddFlag(Keywords.Const);
                return keywords;
            }
            case MethodBase method:
            {
                if (method.IsAbstract)
                    keywords.AddFlag(Keywords.Abstract);
                if (method.IsFinal)
                    keywords.AddFlag(Keywords.Sealed);
                if (method.IsVirtual)
                    keywords.AddFlag(Keywords.Virtual);
                return keywords;
            }
            case PropertyInfo propertyInfo:
            {
                keywords.AddFlag(GetKeywords(propertyInfo.GetGetter()));
                keywords.AddFlag(GetKeywords(propertyInfo.GetSetter()));
                return keywords;
            }
            case EventInfo eventInfo:
            {
                keywords.AddFlag(GetKeywords(eventInfo.GetAdder()));
                keywords.AddFlag(GetKeywords(eventInfo.GetRemover()));
                keywords.AddFlag(GetKeywords(eventInfo.GetRaiser()));
                return keywords;
            }
            case Type type:
            {
                if (type.IsAbstract)
                    keywords.AddFlag(Keywords.Abstract);
                if (type.IsSealed)
                    keywords.AddFlag(Keywords.Sealed);
                return keywords;
            }
            default:
                return keywords;
        }
    }
}