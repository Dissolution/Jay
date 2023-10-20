namespace Jay.Reflection.CodeBuilding;

public static class CodePart
{
    public static string ToDeclaration<T>([AllowNull] T value)
    {
        throw new NotImplementedException();
    }

    public static string ToCode(ref InterpolatedCode interpolatedCode)
    {
        return interpolatedCode.ToStringAndDispose();
    }
    
    public static string ToCode<T>([AllowNull] T value)
    {
        throw new NotImplementedException();
    }

    public static void DeclareTo<T>([AllowNull] T value, CodeBuilder codeBuilder)
    {
        throw new NotImplementedException();
    }
}