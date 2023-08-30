namespace IMPL.SourceGen.Writers;

public interface IPropertySigWriter
{
    void Write(PropertySig propertySig, CodeBuilder codeBuilder);
}
