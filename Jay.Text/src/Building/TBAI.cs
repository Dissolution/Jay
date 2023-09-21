namespace Jay.Text.Building;

public delegate void TBAI<in TBuilder, in T>(TBuilder builder, T value, int index)
    where TBuilder : TextWriter;