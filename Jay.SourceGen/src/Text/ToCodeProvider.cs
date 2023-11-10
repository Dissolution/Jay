namespace Jay.SourceGen.Text;

public abstract class ToCodeProvider<T>
{
    protected ToCodeProvider() { }
    
    public abstract bool WriteTo(T? value, CodeBuilder codeBuilder);
}