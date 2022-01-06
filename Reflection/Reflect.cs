using System.Reflection;

namespace Jay.Reflection;

public class Reflect
{
    public const BindingFlags AllFlags = BindingFlags.Public | BindingFlags.NonPublic |
                                         BindingFlags.Static | BindingFlags.Instance |
                                         BindingFlags.IgnoreCase;

    public const BindingFlags PublicFlags = BindingFlags.Public | 
                                         BindingFlags.Static | BindingFlags.Instance |
                                         BindingFlags.IgnoreCase;

    public const BindingFlags NonPublicFlags = BindingFlags.NonPublic |
                                         BindingFlags.Static | BindingFlags.Instance |
                                         BindingFlags.IgnoreCase;

    public const BindingFlags StaticFlags = BindingFlags.Public | BindingFlags.NonPublic |
                                         BindingFlags.Static |
                                         BindingFlags.IgnoreCase;

    public const BindingFlags InstanceFlags = BindingFlags.Public | BindingFlags.NonPublic |
                                         BindingFlags.Instance |
                                         BindingFlags.IgnoreCase;

}