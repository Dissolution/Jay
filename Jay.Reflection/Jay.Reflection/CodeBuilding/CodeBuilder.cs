using Jay.Text.Building;

namespace Jay.Reflection.CodeBuilding;

public class CodeBuilder : IndentTextBuilder<CodeBuilder>
{
    public static CodeBuilder New => new();
    
    public override void Write<T>([AllowNull] T value)
    {
        switch (value)
        {
            case Type type:
            {
                string name = type.NameOf();
                base.Write(name);
                break;
            }
            case CBA cba:
            {
                cba(this);
                break;
            }
            default:
                throw new NotImplementedException();
        }
    }
}