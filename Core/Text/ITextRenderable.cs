namespace Jay.Text;

public interface ITextRenderable
{
    void Render(TextBuilder textBuilder, string? format);
}