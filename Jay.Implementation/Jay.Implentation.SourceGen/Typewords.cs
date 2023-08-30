namespace IMPL.SourceGen;

public record struct Typewords(Visibility Visibility, Instic Instic, Keywords Keywords, ObjType ObjType)
{
    public static Typewords operator |(Typewords left, Typewords right)
    {
        return new(left.Visibility | right.Visibility, left.Instic | right.Instic, left.Keywords | right.Keywords, left.ObjType | right.ObjType);
    }
}
