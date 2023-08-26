namespace Jay.Reflection.Tests.Entities;

public class TestClass
{
#region Fields
    private int _privateIntField;
    protected int _protectedIntField;
    internal int _internalIntField;
    public int _publicIntField;
    
    private string _privateStringField;
    protected string _protectedStringField;
    internal string _internalStringField;
    public string _publicStringField;
    
    private object? _privateObjectField;
    protected object? _protectedObjectField;
    internal object? _internalObjectField;
    public object? _publicObjectField;
    
    
    private readonly int _privateReadonlyIntField;
    protected readonly int _protectedReadonlyIntField;
    internal readonly int _internalReadonlyIntField;
    public readonly int _publicReadonlyIntField;
    
    private readonly string _privateReadonlyStringField;
    protected readonly string _protectedReadonlyStringField;
    internal readonly string _internalReadonlyStringField;
    public readonly string _publicReadonlyStringField;
    
    private readonly object? _privateReadonlyObjectField;
    protected readonly object? _protectedReadonlyObjectField;
    internal readonly object? _internalReadonlyObjectField;
    public readonly object? _publicReadonlyObjectField;
#endregion
    
#region Properties
    private int PrivateIntGetSetProperty { get; set; }
    protected int ProtectedIntGetSetProperty { get; set; }
    internal int InternalIntGetSetProperty { get; set; }
    public int PublicIntGetSetProperty { get; set; }
    
    private string PrivateStringGetSetProperty { get; set; }
    protected string ProtectedStringGetSetProperty { get; set; }
    internal string InternalStringGetSetProperty { get; set; }
    public string PublicStringGetSetProperty { get; set; }
    
    private object? PrivateObjectGetSetProperty { get; set; }
    protected object? ProtectedObjectGetSetProperty { get; set; }
    internal object? InternalObjectGetSetProperty { get; set; }
    public object? PublicObjectGetSetProperty { get; set; }
    
    private int PrivateIntGetInitProperty { get; init; }
    protected int ProtectedIntGetInitProperty { get; init; }
    internal int InternalIntGetInitProperty { get; init; }
    public int PublicIntGetInitProperty { get; init; }
    
    private string PrivateStringGetInitProperty { get; init; }
    protected string ProtectedStringGetInitProperty { get; init; }
    internal string InternalStringGetInitProperty { get; init; }
    public string PublicStringGetInitProperty { get; init; }
    
    private object? PrivateObjectGetInitProperty { get; init; }
    protected object? ProtectedObjectGetInitProperty { get; init; }
    internal object? InternalObjectGetInitProperty { get; init; }
    public object? PublicObjectGetInitProperty { get; init; }
    
    private int PrivateIntGetProperty { get; }
    protected int ProtectedIntGetProperty { get; }
    internal int InternalIntGetProperty { get; }
    public int PublicIntGetProperty { get; }
    
    private string PrivateStringGetProperty { get; }
    protected string ProtectedStringGetProperty { get; }
    internal string InternalStringGetProperty { get; }
    public string PublicStringGetProperty { get; }
    
    private object? PrivateObjectGetProperty { get; }
    protected object? ProtectedObjectGetProperty { get; }
    internal object? InternalObjectGetProperty { get; }
    public object? PublicObjectGetProperty { get; }

#endregion
}