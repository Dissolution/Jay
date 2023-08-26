namespace Jay.Reflection.Text;

public enum Naming
{
    CamelCase,
    PascalCase,
    UpperCase,
    LowerCase,
    Field,
    Property = PascalCase,
    Method = PascalCase,
    Event = PascalCase,
    Type = PascalCase,
}