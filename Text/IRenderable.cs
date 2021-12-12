namespace Jay.Text;

public interface IRenderable
{
    void Render(TextBuilder builder);

    string ToString() => TextBuilder.Build(Render);
}