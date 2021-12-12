using System.Reflection;

namespace Jay.Reflection;

public class Reflect
{
    public const BindingFlags AllFlags = BindingFlags.Public | BindingFlags.NonPublic |
                                         BindingFlags.Static | BindingFlags.Instance |
                                         BindingFlags.IgnoreCase;
        
}