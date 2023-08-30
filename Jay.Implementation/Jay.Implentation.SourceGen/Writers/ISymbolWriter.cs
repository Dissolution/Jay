namespace IMPL.SourceGen.Writers;

public interface ICodeSpaceWriter<TValue>
{
    void Write(ICodeSpace codeSpace, TValue value, CodeBuilder codeBuilder);
}

public interface ISymbolWriter<TSymbol> : ICodeSpaceWriter<TSymbol>
    where TSymbol : ISymbol
{

}

public abstract class SymbolWriter<TSymbol> : ISymbolWriter<TSymbol>
    where TSymbol : ISymbol
{

}

public interface IFieldWriter : ISymbolWriter<IFieldSymbol>
{

}

public interface IPropertyWriter : ISymbolWriter<IPropertySymbol>
{

}






public sealed class DefaultFieldWriter : IFieldWriter
{
    public void Write(ICodeSpace codeSpace, IFieldSymbol field, CodeBuilder codeBuilder)
    {
        codeBuilder
            .Append(field.GetVisibility(), Naming.Lower).Append(' ')
            .AppendIf(field.IsStatic, "static ")
            .AppendIf(field.IsConst, "const ")
            .AppendIf(field.IsReadOnly, "readonly ")
            .AppendIf(field.IsAbstract, "abstract ")
            .AppendIf(field.IsOverride, "override ")
            .AppendIf(field.IsRequired, "required ")
            .AppendIf(field.IsVirtual, "virtual ")
            .AppendIf(field.IsVolatile, "volatile ")
            .Append(field.Name)
            .Append(';')
            .NewLine();
    }
}

public class PropertyWriter : IPropertyWriter
{
    protected void WriteToName(IPropertySymbol property, CodeBuilder codeBuilder)
    {
        if (property.IsIndexer)
        {

        }
        else
        {
            codeBuilder
              .Append(property.GetVisibility(), Naming.Lower).Append(' ')
              .AppendIf(property.IsStatic, "static ")
              .AppendIf(property.IsReadOnly, "readonly ")
              .AppendIf(property.IsAbstract, "abstract ")
              .AppendIf(property.IsOverride, "override ")
              .AppendIf(property.IsRequired, "required ")
              .AppendIf(property.IsVirtual, "virtual ")
              .AppendIf(property.IsSealed, "sealed ")
              .Append(property.Name);
        }
    }

    public void Write(ICodeSpace codeSpace, IPropertySymbol property, CodeBuilder codeBuilder)
    {

    }
}


public interface ICodeSpace
{
    List<ISymbol> Symbols { get; }
}
