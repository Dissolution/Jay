namespace Jay.CodeGen.Extensions;

public static class ArrayExtensions
{
    public static Type?[] GetElementTypes(this object?[] objectArray)
    {
        int count = objectArray.Length;
        var elementTypes = new Type?[count];
        for (var i = 0; i < count; i++)
        {
            elementTypes[i] = objectArray[i]?.GetType() ;
        }
        return elementTypes;
    }
    
    public static Type[] GetElementTypes(this object?[] objectArray, Type fallbackType)
    {
        int count = objectArray.Length;
        var elementTypes = new Type[count];
        for (var i = 0; i < count; i++)
        {
            elementTypes[i] = objectArray[i]?.GetType() ?? fallbackType;
        }
        return elementTypes;
    }
}