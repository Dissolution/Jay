using Jay.Reflection.Emitting;

namespace Jay.Reflection.Deconstruction;

public class MethodDeconstruction
{
    public MethodBase Method { get; }
    public MethodBody MethodBody { get; }
    public byte[] IlBytes { get; }
    public IReadOnlyList<ParameterInfo> Parameters { get; }
    public IReadOnlyList<LocalVariableInfo> Locals { get; }
    
    public EmissionStream Emissions { get; internal set; }
    
    internal MethodDeconstruction(
        MethodBase method,
        MethodBody methodBody,
        byte[] ilBytes,
        ParameterInfo[] parameters,
        IReadOnlyList<LocalVariableInfo> locals)
    {
        this.Method = method;
        this.MethodBody = methodBody;
        this.IlBytes = ilBytes;
        this.Parameters = parameters;
        this.Locals = locals;
        this.Emissions = null!; // Gets filled before this class is ever returned
    }
}