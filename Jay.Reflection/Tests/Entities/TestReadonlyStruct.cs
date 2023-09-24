﻿// ReSharper disable UnassignedReadonlyField
// ReSharper disable UnassignedGetOnlyAutoProperty
#pragma warning disable CS0649 // Field is never assigned to, and will always have its default value
#pragma warning disable CS8618 // Non-nullable field must contain a non-null value when exiting constructor. Consider declaring as nullable.
#pragma warning disable CS0169 // Field is never used

namespace Jay.Reflection.Tests.Entities;

public readonly struct TestReadonlyStruct
{
#region Fields
    private readonly int _privateReadonlyIntField;
    internal readonly int _internalReadonlyIntField;
    public readonly int _publicReadonlyIntField;
    
    private readonly string _privateReadonlyStringField;
    internal readonly string _internalReadonlyStringField;
    public readonly string _publicReadonlyStringField;
    
    private readonly object? _privateReadonlyObjectField;
    internal readonly object? _internalReadonlyObjectField;
    public readonly object? _publicReadonlyObjectField;
#endregion
    
    #region Properties
    private int PrivateIntGetInitProperty { get; init; }
    internal int InternalIntGetInitProperty { get; init; }
    public int PublicIntGetInitProperty { get; init; }
    
    private string PrivateStringGetInitProperty { get; init; }
    internal string InternalStringGetInitProperty { get; init; }
    public string PublicStringGetInitProperty { get; init; }
    
    private object? PrivateObjectGetInitProperty { get; init; }
    internal object? InternalObjectGetInitProperty { get; init; }
    public object? PublicObjectGetInitProperty { get; init; }
    
    private int PrivateIntGetProperty { get; }
    internal int InternalIntGetProperty { get; }
    public int PublicIntGetProperty { get; }
    
    private string PrivateStringGetProperty { get; }
    internal string InternalStringGetProperty { get; }
    public string PublicStringGetProperty { get; }
    
    private object? PrivateObjectGetProperty { get; }
    internal object? InternalObjectGetProperty { get; }
    public object? PublicObjectGetProperty { get; }

#endregion
}